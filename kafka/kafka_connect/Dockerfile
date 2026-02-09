FROM alpine:3.21.3 AS alpine
RUN apk add curl unzip && \
    cd /tmp && \
    curl -LO https://github.com/ClickHouse/clickhouse-kafka-connect/releases/download/v1.2.6/clickhouse-kafka-connect-v1.2.6.zip && \ 
    unzip clickhouse-kafka-connect-v1.2.6.zip;

FROM confluentinc/cp-kafka-connect:7.8.0
ENV DBZ_VERSION=3.0.0.Final
RUN wget https://repo.maven.apache.org/maven2/io/prometheus/jmx/jmx_prometheus_javaagent/1.0.0/jmx_prometheus_javaagent-1.0.0.jar
WORKDIR /tmp
RUN mkdir /usr/share/java/debezium; \
    debezium_connectors=("jdbc" "mongodb" "mysql" "postgres"); \
    for connector in "${debezium_connectors[@]}"; \
    do \
        wget https://repo1.maven.org/maven2/io/debezium/debezium-connector-${connector}/${DBZ_VERSION}/debezium-connector-${connector}-${DBZ_VERSION}-plugin.tar.gz; \
        tar -xf ./debezium-connector-${connector}-${DBZ_VERSION}-plugin.tar.gz; \
        mkdir /usr/share/java/debezium/debezium-connector-${connector}; \
		mv ./debezium-connector-${connector}/*.jar /usr/share/java/debezium/debezium-connector-${connector}/; \
    done; \
    confluent-hub install --no-prompt confluentinc/kafka-connect-jdbc:10.8.0 && \
    confluent-hub install --no-prompt confluentinc/kafka-connect-json-schema-converter:latest; \
    mkdir -p /usr/share/java/clickhouse;
COPY --from=alpine /tmp/clickhouse-kafka-connect-v1.2.6 /usr/share/java/clickhouse/
WORKDIR /home/appuser
COPY ./config/ /home/appuser/