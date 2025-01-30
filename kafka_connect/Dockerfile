ARG CONFLUENT_VERSION
FROM confluentinc/cp-kafka-connect:${CONFLUENT_VERSION}
ARG DEBEZIUM_VERSION
ARG JDBC_CONNECTOR_VERSION
ENV DBZ_VERSION=${DEBEZIUM_VERSION}
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
    confluent-hub install --no-prompt confluentinc/kafka-connect-jdbc:${JDBC_CONNECTOR_VERSION} && \
    confluent-hub install --no-prompt confluentinc/kafka-connect-json-schema-converter:latest;
WORKDIR /home/appuser