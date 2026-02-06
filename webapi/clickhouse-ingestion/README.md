## Overview

This assignment implements a small **traffic ingestion and reporting service** on top of **ClickHouse**.  
The service exposes HTTP endpoints to **write**, **read (daily report)**, and (optionally) **update** traffic events.

- **Project Files**:
  - Clickhouse implementations: [https://github.com/meysam-aghili/resume/tree/main/clickhouse](https://github.com/meysam-aghili/resume/tree/main/clickhouse)
  - FastApi implementations: [https://github.com/meysam-aghili/resume/tree/main/webapi/clickhouse-ingestion](https://github.com/meysam-aghili/resume/tree/main/webapi/clickhouse-ingestion)
- **Tech stack**: FastAPI, ClickHouse, Docker.
- **Core entities**: one `traffic` fact table in ClickHouse, keyed by `request_id`, partitioned by `created_at` in daily format.

## ClickHouse Table Design & Performance
Table DDL (see `clickhouse/utils/snapp.sql`) is:

```sql
CREATE TABLE IF NOT EXISTS snapp.traffic ON CLUSTER cluster_nesaj
(
    request_id UInt64 NOT NULL COMMENT 'id of the request',
    user_id    UInt32 NOT NULL COMMENT 'id of the user',
    created_at DateTime NOT NULL COMMENT 'datetime of the visit',
    page_url   String NOT NULL COMMENT 'visited webapp url'
) 
ENGINE = ReplicatedReplacingMergeTree('/clickhouse/tables/{shard}/snapp_traffic', '{replica}', request_id)
PARTITION BY toDate(created_at)
ORDER BY (user_id, created_at)
SETTINGS index_granularity = 8192
COMMENT 'traffic log for page views';
```

### Engine choice: `ReplicatedReplacingMergeTree`
- **Why MergeTree**: optimized for **append-heavy analytical workloads** with large volumes of immutable events.
- **Why Replacing**: `request_id` is used as the deduplication key, so **updates are modeled as new rows** with the same `request_id`. During background merges, older versions are removed and the latest one is kept.
  - This gives us an efficient strategy for the bonus **update** endpoint in a system where in-place UPDATE is expensive.
- **Why Replicated**: the table is replicated across nodes in the `cluster_nesaj` cluster, giving:
  - **High availability** (node failures do not take the table down).
  - **Horizontal read scalability** (queries can be routed via `chproxy` to multiple replicas).

### Partitioning & primary key for performance
- **Partitioning**: `PARTITION BY toDate(created_at)`
  - Typical analytics filters are time-based; partitioning by day allows ClickHouse to **skip entire partitions** for queries outside the date range.
  - Daily partitions align with the **“last 24 hours”** query pattern.
- **Primary key / ORDER BY**: `(user_id, created_at)`
  - The critical query is **by `user_id` over a recent time window**, grouping by `page_url`.  
  - Ordering the data by `(user_id, created_at)` means ClickHouse can:
    - Efficiently locate all rows for a given user using sparse primary key indexes.
    - Read data for the last 24 hours in a **mostly sequential** fashion, which is cache- and IO-friendly.
- **Index granularity**: `8192`
  - Default value suitable for large append-only tables; it balances index size vs. read performance.

## API Design
- **Write request**
  - **Endpoint**: `POST /api/traffic/{request_id}`
  - **Body**:
    - `user_id: int`
    - `created_at: datetime`
    - `page_url: string`
  - **Behavior**: Inserts one row into `snapp.traffic`. The `created_at` field is validated so it cannot be in the future (guards against bad data arriving late and confusing analytics).
- **Read request – daily report**
  - **Endpoint**: `GET /api/traffic/daily/{user_id}`
  - **Response**: JSON object mapping `page_url -> count` for the last 24 hours.
  - **Query pattern**: `WHERE user_id = ? AND created_at >= now() - INTERVAL 1 DAY GROUP BY page_url`.
- **Health check**
  - `GET /health` returns `{"status": "ok"}` and is used for container / k8s health probes.

## Application Architecture
- **FastAPI application** (`webapi/clickhouse-ingestion`):
  - `main.py`: creates the `FastAPI` app, configures lifespan hooks, and attaches routers.
  - `routes.py`: defines the traffic endpoints for write/read (and can be extended with update).
  - `models.py`: Pydantic models for input validation and response schema.
  - `db.py`: wraps `clickhouse-connect` client initialization.
  - `settings.py`: centralizes configuration using Pydantic `BaseSettings` and `.env` files.
  - `tests.py`: unit tests for the endpoints with ClickHouse client mocked.
- **ClickHouse layer** (`clickhouse` folder):
  - `docker-compose.yml`: multi-node ClickHouse cluster + Keeper + `chproxy`.
  - `config/*.xml`: ClickHouse and Keeper configuration (cluster, replication, users).
  - `utils/snapp.sql`: DDL for `snapp.traffic` table and basic smoke tests.

## Dockerization & CICD
gitlab-ci :
```yml
stages:
  - build
  - test
  - deploy

before_script:
  - source .env
  - docker login -u "$CI_REGISTRY_USER" -p "$CI_REGISTRY_PASSWORD" "$CI_REGISTRY"

build api:
  stage: build
  script:
    - docker build -t "$CI_REGISTRY_IMAGE/api:$API_VERSION" ./webapi/clickhouse-ingestion
    - docker push "$CI_REGISTRY_IMAGE/api:$API_VERSION"

test api:
  stage: test
  script:
    - docker run --rm --pull always --env-file .env $CI_REGISTRY_IMAGE/api:$API_VERSION bash -c "pytest -v /app/src"

deploy clickhouse:
  stage: deploy
  only:
    - main
  script:
    - ./parse_compose_envs.sh clickhouse/docker-compose.yml .env
    - docker stack deploy -c clickhouse/docker-compose.prod.yml --with-registry-auth clickhouse

deploy api:
  stage: deploy
  only:
    - main
  script:
    - ./parse_compose_envs.sh webapi/clickhouse-ingestion/docker-compose.yml .env
    - docker stack deploy -c webapi/clickhouse-ingestion/docker-compose.prod.yml --with-registry-auth clickhouse-ingestion-api
```

## Performance Considerations
### Write Path Optimization
  - Batch Inserts: Collect multiple events in memory or a buffer before sending to ClickHouse to reduce network overhead and increase insert throughput.
  - Asynchronous Inserts: Use FastAPI’s async endpoints combined with async ClickHouse clients to avoid blocking the web server during writes.
  - Write Queue / Buffer: For heavy traffic spikes, consider a lightweight queue (e.g., Kafka, RabbitMQ) to decouple ingestion from ClickHouse and provide retry/fault-tolerance.
  - Deduplication Strategy: Rely on ReplacingMergeTree to handle duplicate request_ids asynchronously; avoids slowing down inserts with real-time deduplication.
### Read Path Optimization
  - Materialized Views / Pre-Aggregation: Precompute daily summaries per user/page to minimize SELECT FINAL usage and speed up reporting queries.
### Cluster and Scaling
  - Web API is stateless → can scale horizontally.
  - All writes and heavy analytical reads hit the same node - multi instance with load balancer
### Monitoring & Observability
  - Metrics: Use Prometheus + Grafana to monitor insert rates, query latency, ClickHouse merge queue, and system resource usage.
  - Logging & Alerts: Track failed inserts, high latency queries, and unusual traffic patterns.
### Alternative technologies
  - Kafka + ClickHouse sink for buffering and high-throughput ingestion.
  - Batch ETL via Spark / Airflow for analytics or backfill.
