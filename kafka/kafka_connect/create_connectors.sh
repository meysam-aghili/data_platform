#!/bin/bash

input_file="$1"

if [ "$input_file" == "d" ]; then
	curl -X DELETE "http://localhost:8083/connectors/source-postgres-akrana-sales"
	curl -X DELETE "http://localhost:8083/connectors/source-postgres-iot-sensors"
	curl -X DELETE "http://localhost:8083/connectors/source-postgres-log-logs"
	curl -X DELETE "http://localhost:8083/connectors/sink-clickhouse"
fi

curl -X POST -H "Content-Type: application/json" --data @./kafka/kafka_connect/connectors/source_pg_akrana.json http://localhost:8083/connectors
curl -X POST -H "Content-Type: application/json" --data @./kafka/kafka_connect/connectors/source_pg_iot.json http://localhost:8083/connectors
curl -X POST -H "Content-Type: application/json" --data @./kafka/kafka_connect/connectors/source_pg_log.json http://localhost:8083/connectors
curl -X POST -H "Content-Type: application/json" --data @./kafka/kafka_connect/connectors/sink_clickhouse.json http://localhost:8083/connectors
