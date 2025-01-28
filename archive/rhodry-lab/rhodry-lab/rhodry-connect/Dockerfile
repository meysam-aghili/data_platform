ARG CONFLUENT_VERSION

FROM registry-git.digikala.com/bi/rhodry/ibmjava:8-sdk AS rhodry
ARG RHODRY_CONNECT_VERSION
ARG DEBEZIUM_VERSION
ENV VERSION=${RHODRY_CONNECT_VERSION}
ENV DBZ_VERSION=${DEBEZIUM_VERSION}

RUN apt update && apt -y upgrade && apt install -y maven
WORKDIR /rhodry
COPY . .
RUN mkdir dependencies && mvn dependency:copy-dependencies -DoutputDirectory=dependencies -DdebeziumVersion=${DBZ_VERSION}
RUN mvn clean package -DskipTests=true -Djacoco.skip=true -Djar.finalName=rhodry-${VERSION} -DrhodryConnectVersion=${VERSION} -DdebeziumVersion=${DBZ_VERSION}

# WORKDIR /dependencies
# RUN \
#     mkdir jars; \
#     wget https://repo1.maven.org/maven2/com/fasterxml/jackson/core/jackson-databind/2.13.5/jackson-databind-2.13.5.pom; \
#     wget https://repo1.maven.org/maven2/com/fasterxml/jackson/datatype/jackson-datatype-hibernate5/2.13.5/jackson-datatype-hibernate5-2.13.5.pom; \
#     mvn dependency:copy-dependencies -f jackson-databind-2.13.5.pom -DoutputDirectory=jars; \
#     mvn dependency:copy-dependencies -f jackson-datatype-hibernate5-2.13.5.pom -DoutputDirectory=jars; \
#     cd jars; \
#     wget https://repo1.maven.org/maven2/com/fasterxml/jackson/core/jackson-databind/2.13.5/jackson-databind-2.13.5.jar; \
#     wget https://repo1.maven.org/maven2/com/fasterxml/jackson/datatype/jackson-datatype-hibernate5/2.13.5/jackson-datatype-hibernate5-2.13.5.jar;

FROM registry-git.digikala.com/bi/rhodry/confluentinc/cp-kafka-connect:${CONFLUENT_VERSION} AS connect

RUN wget https://repo.maven.apache.org/maven2/io/prometheus/jmx/jmx_prometheus_javaagent/1.0.0/jmx_prometheus_javaagent-1.0.0.jar

ARG \
    CONFLUENT_VERSION \
    RHODRY_CONNECT_VERSION \
    DEBEZIUM_VERSION \
    JDBC_CONNECTOR_VERSION \
    JSON_SCHEMA_CONVERTER_VERSION \
    ELASTICSEARCH_SOURCE_CONNECTOR_VERSION
ENV VERSION=${RHODRY_CONNECT_VERSION}
ENV DBZ_VERSION=${DEBEZIUM_VERSION}

WORKDIR /tmp

RUN \
    wget https://repo1.maven.org/maven2/com/microsoft/sqlserver/mssql-jdbc/12.6.1.jre11/mssql-jdbc-12.6.1.jre11.jar; \
    mkdir /usr/share/java/rhodry; \
    mv mssql-jdbc-12.6.1.jre11.jar /usr/share/java/rhodry/; \
    mkdir /usr/share/java/debezium; \
    debezium_connectors=("jdbc" "mongodb" "mysql" "postgres" "sqlserver"); \
    for connector in "${debezium_connectors[@]}"; \
    do \
        wget https://repo1.maven.org/maven2/io/debezium/debezium-connector-${connector}/${DBZ_VERSION}/debezium-connector-${connector}-${DBZ_VERSION}-plugin.tar.gz; \
        tar -xf ./debezium-connector-${connector}-${DBZ_VERSION}-plugin.tar.gz; \
        mkdir /usr/share/java/debezium/debezium-connector-${connector}; \
        mv ./debezium-connector-${connector}/*.jar /usr/share/java/debezium/debezium-connector-${connector}/; \
    done; \
    confluent-hub install --no-prompt confluentinc/kafka-connect-jdbc:${JDBC_CONNECTOR_VERSION}; \
    confluent-hub install --no-prompt confluentinc/kafka-connect-json-schema-converter:${JSON_SCHEMA_CONVERTER_VERSION}; \
    confluent-hub install --no-prompt dariobalinzo/kafka-connect-elasticsearch-source:${ELASTICSEARCH_SOURCE_CONNECTOR_VERSION}; \
    confluent-hub install --no-prompt confluentinc/kafka-connect-datagen:latest;

COPY --from=rhodry /rhodry/dependencies/*.jar /usr/share/java/rhodry/
COPY --from=rhodry /rhodry/target/rhodry-${VERSION}.jar /usr/share/java/rhodry/
# COPY --from=rhodry /dependencies/jars/*.jar /usr/share/java/debezium/

WORKDIR /home/appuser
