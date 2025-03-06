ARG SPARK_VERSION=3.5.1

FROM registry-git.digikala.com/bi/rhodry/ibmjava:8-sdk AS maven

RUN apt update && apt -y upgrade && apt install -y maven curl
RUN mkdir -p /dependencies/jars
WORKDIR /dependencies
RUN \
    curl \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-sql-kafka-0-10_2.12/3.5.1/spark-sql-kafka-0-10_2.12-3.5.1.pom \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-avro_2.12/3.5.1/spark-avro_2.12-3.5.1.pom \
    --remote-name https://repo1.maven.org/maven2/io/delta/delta-spark_2.12/3.2.0/delta-spark_2.12-3.2.0.pom \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-connect_2.12/3.5.1/spark-connect_2.12-3.5.1.pom \
    --remote-name https://repo1.maven.org/maven2/io/delta/delta-storage/3.2.0/delta-storage-3.2.0.pom && \

    mvn dependency:copy-dependencies -f spark-sql-kafka-0-10_2.12-3.5.1.pom -DoutputDirectory=jars && \
    mvn dependency:copy-dependencies -f spark-avro_2.12-3.5.1.pom -DoutputDirectory=jars && \
    mvn dependency:copy-dependencies -f spark-connect_2.12-3.5.1.pom -DoutputDirectory=jars && \
    mvn dependency:copy-dependencies -f delta-spark_2.12-3.2.0.pom -DoutputDirectory=jars && \ 
    mvn dependency:copy-dependencies -f delta-storage-3.2.0.pom -DoutputDirectory=jars

FROM registry-git.digikala.com/bi/spark/bitnami/spark:${SPARK_VERSION}

USER root
RUN install_packages curl

USER 1001
WORKDIR ${SPARK_HOME}/jars
RUN curl \
    --remote-name https://repo1.maven.org/maven2/net/sourceforge/jtds/jtds/1.3.1/jtds-1.3.1.jar \
    --remote-name https://repo1.maven.org/maven2/com/microsoft/sqlserver/mssql-jdbc/12.6.1.jre11/mssql-jdbc-12.6.1.jre11.jar \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-sql-kafka-0-10_2.12/3.5.1/spark-sql-kafka-0-10_2.12-3.5.1.jar \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-avro_2.12/3.5.1/spark-avro_2.12-3.5.1.jar \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-connect_2.12/3.5.1/spark-connect_2.12-3.5.1.jar \

    --remote-name https://repo1.maven.org/maven2/io/delta/delta-spark_2.12/3.2.0/delta-spark_2.12-3.2.0.jar \
    --remote-name https://repo1.maven.org/maven2/io/delta/delta-storage/3.2.0/delta-storage-3.2.0.jar 

COPY --from=maven /dependencies/jars/*.jar ${SPARK_HOME}/jars/
WORKDIR ${SPARK_HOME}