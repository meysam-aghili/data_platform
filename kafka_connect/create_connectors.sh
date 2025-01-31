#!/bin/bash

input_file="$1"

if [ "$input_file" == "d" ]; then
	curl -X DELETE "http://localhost:8083/connectors/source-postgres-akrana-sales"
	curl -X DELETE "http://localhost:8083/connectors/source-postgres-iot-sales"
	curl -X DELETE "http://localhost:8083/connectors/source-postgres-log-sales"
fi

curl -X POST -H "Content-Type: application/json" --data @./kafka_connect/connector_akrana.json http://localhost:8083/connectors
curl -X POST -H "Content-Type: application/json" --data @./kafka_connect/connector_iot.json http://localhost:8083/connectors
curl -X POST -H "Content-Type: application/json" --data @./kafka_connect/connector_log.json http://localhost:8083/connectors
