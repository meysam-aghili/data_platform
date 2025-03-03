
create-network:
	docker network create --driver overlay --scope swarm --attachable platform || echo "network already exists. skipping."

remove-all-volumes:
	docker volume rm $$(docker volume ls -q) && docker volume prune -f

create-kafka-connectors:
	./kafka/kafka_connect/create_connectors.sh d

init-pg-tables:
	python ./postgres/utils/generate_data.py -delete -initdb
generate-data-pg-tables:
	python ./postgres/utils/generate_data.py -gen

create-secrets:
	printf "postgres" | docker secret create postgres_password -
	printf "grafana" | docker secret create grafana_password -
	printf "minio" | docker secret create minio_password -
	printf "elasticsearch" | docker secret create elasticsearch_password -
	cat ./docker/configs/certs/domain.key | docker secret create registry_ssl_key -
	cat ./docker/configs/certs/domain.crt | docker secret create registry_ssl_crt -
	printf "influxdb" | docker secret create influxdb_password -
	printf "FMZ6bCX2QekeXOGPfctfRATHAFzkWaFBx-rbq1nhTyYvsdJ-DrClTI_1l2Vc_bjqJwmGYYO3dWdc5lfIxnFQYA==" | docker secret create influxdb_admin_token -

##########################

deploy-docker:
	./deploy-stack.sh docker

deploy-clickhouse:
	./deploy-stack.sh clickhouse

deploy-grafana:
	docker build -t localregistry.com/grafana/grafana-prod:11.5.0 ./grafana/
	docker push localregistry.com/grafana/grafana-prod:11.5.0
	./deploy-stack.sh grafana

deploy-metabase:
	docker build -t localregistry.com/metabase/metabase-prod:v0.53.2.2 ./metabase/
	docker push localregistry.com/metabase/metabase-prod:v0.53.2.2
	./deploy-stack.sh metabase

deploy-postgres:
	./deploy-stack.sh postgres

deploy-prometheus:
	./deploy-stack.sh prometheus

deploy-superset:
	docker build -t localregistry.com/apache/superset-prod:4.1.1 ./superset/
	docker push localregistry.com/apache/superset-prod:4.1.1
	./deploy-stack.sh superset

deploy-kafka:
	docker build -t localregistry.com/confluentinc/cp-kafka-prod:7.8.0 ./kafka/
	docker build -t localregistry.com/confluentinc/cp-kafka-connect-prod:7.8.0 ./kafka/kafka_connect/
	docker push localregistry.com/confluentinc/cp-kafka-prod:7.8.0
	docker push localregistry.com/confluentinc/cp-kafka-connect-prod:7.8.0
	./deploy-stack.sh kafka

deploy-influxdb:
	./deploy-stack.sh influxdb

deploy-cratedb:
	docker build -t localregistry.com/crate-prod:5.10.1 ./cratedb/
	docker push localregistry.com/crate-prod:5.10.1
	./deploy-stack.sh cratedb
