ifneq (,$(wildcard ./.env))
    include .env
    export
endif

ifneq (,$(wildcard ./kafka_connect/.env))
    include ./kafka_connect/.env
    export
endif

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

deploy-clickhouse:
	./compose_envs.sh docker-compose-clickhouse.yml
	docker stack deploy platform-clickhouse --compose-file docker-compose-clickhouse.prod.yml
	rm docker-compose-clickhouse.prod.yml

deploy: create-network deploy-db deploy-kafka deploy-viz deploy-monitoring deploy-clickhouse

remove-all-volumes:
	docker volume rm $$(docker volume ls -q) && docker volume prune -f

create-kafka-connectors:
	./kafka_connect/create_connectors.sh d

setup-pg-kafka:
	python ./postgres/generate_data.py -initdb
	create-kafka-connectors

create-secrets:
	printf "postgres" | docker secret create postgres_password -
	printf "grafana" | docker secret create grafana_password -
	$ printf "minio" | docker secret create minio_password -
	printf "elasticsearch" | docker secret create elasticsearch_password -

docker-swarm:
	docker swarm init