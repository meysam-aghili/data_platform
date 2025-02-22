include .env
include ./kafka/kafka_connect/.env
include ./superset/.env


all: build deploy

build-kafka:
	docker build -t confluentinc/cp-kafka-prod:${CONFLUENT_VERSION} --build-arg CONFLUENT_VERSION=${CONFLUENT_VERSION} ./kafka/

build-kafka-connect:
	docker build -t confluentinc/cp-kafka-connect-prod:${CONFLUENT_VERSION} \
		--build-arg CONFLUENT_VERSION=${CONFLUENT_VERSION} \
		--build-arg DEBEZIUM_VERSION=${DEBEZIUM_VERSION} \
		--build-arg JDBC_CONNECTOR_VERSION=${JDBC_CONNECTOR_VERSION} \
		./kafka_connect/

build: build-kafka build-kafka-connect

create-network:
	docker network create --driver overlay --scope swarm --attachable platform || echo "network already exists. skipping."

deploy-kafka:
	./compose_envs.sh docker-compose-kafka.yml
	docker stack deploy platform-kafka --compose-file docker-compose-kafka.prod.yml
	rm docker-compose-kafka.prod.yml

deploy-db:
	./compose_envs.sh docker-compose-db.yml
	docker stack deploy platform-db --compose-file docker-compose-db.prod.yml
	rm docker-compose-db.prod.yml

deploy-viz:
	./compose_envs.sh docker-compose-visualizer.yml
	docker stack deploy platform-visualizer --compose-file docker-compose-visualizer.prod.yml
	rm docker-compose-visualizer.prod.yml

deploy-monitoring:
	./compose_envs.sh docker-compose-monitoring.yml
	docker stack deploy platform-monitoring --compose-file docker-compose-monitoring.prod.yml
	rm docker-compose-monitoring.prod.yml

deploy: create-network deploy-db deploy-kafka deploy-viz deploy-monitoring deploy-clickhouse

remove-all-volumes:
	docker volume rm $$(docker volume ls -q) && docker volume prune -f

create-kafka-connectors:
	./kafka_connect/create_connectors.sh d

init-pg-tables:
	python ./postgres/generate_data.py -delete -initdb
generate-data-pg-tables:
	python ./postgres/generate_data.py -gen

create-secrets:
	printf "postgres" | docker secret create postgres_password -
	printf "grafana" | docker secret create grafana_password -
	printf "minio" | docker secret create minio_password -
	printf "elasticsearch" | docker secret create elasticsearch_password -
	cat ./docker/configs/certs/domain.key | docker secret create registry_ssl_key -
	cat ./docker/configs/certs/domain.crt | docker secret create registry_ssl_crt -


##########################

deploy-docker:
	./deploy-stack.sh docker

deploy-clickhouse:
	./deploy-stack.sh clickhouse

deploy-grafana:
	docker build -t localregistry.com/grafana/grafana-prod:11.5.0 ./grafana/
	./deploy-stack.sh grafana

deploy-metabase:
	docker build -t localregistry.com/metabase/metabase-prod:v0.53.2.2 ./metabase/
	./deploy-stack.sh metabase

deploy-postgres:
	./deploy-stack.sh postgres

deploy-prometheus:
	./deploy-stack.sh prometheus

deploy-superset:
	docker build -t localregistry.com/apache/superset-prod:4.1.1 ./superset/
	./deploy-stack.sh superset