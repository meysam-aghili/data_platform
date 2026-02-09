
FROM ibmjava:8-sdk AS maven
RUN apt update && apt -y upgrade && apt install -y maven curl
RUN mkdir -p /dependencies/jars
WORKDIR /dependencies
ENV MAVEN_PACKAGE_VERSION=2.12
ENV SPARK_VERSION=3.5.0
RUN \
    curl \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-sql-kafka-0-10_${MAVEN_PACKAGE_VERSION}/${SPARK_VERSION}/spark-sql-kafka-0-10_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.pom \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-avro_${MAVEN_PACKAGE_VERSION}/${SPARK_VERSION}/spark-avro_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.pom \
    --remote-name https://repo1.maven.org/maven2/io/delta/delta-spark_${MAVEN_PACKAGE_VERSION}/3.2.0/delta-spark_${MAVEN_PACKAGE_VERSION}-3.2.0.pom \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-connect_${MAVEN_PACKAGE_VERSION}/${SPARK_VERSION}/spark-connect_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.pom \
    --remote-name https://repo1.maven.org/maven2/io/delta/delta-storage/3.2.0/delta-storage-3.2.0.pom && \
    mvn dependency:copy-dependencies -f spark-sql-kafka-0-10_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.pom -DoutputDirectory=jars && \
    mvn dependency:copy-dependencies -f spark-avro_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.pom -DoutputDirectory=jars && \
    mvn dependency:copy-dependencies -f spark-connect_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.pom -DoutputDirectory=jars && \
    mvn dependency:copy-dependencies -f delta-spark_${MAVEN_PACKAGE_VERSION}-3.2.0.pom -DoutputDirectory=jars && \ 
    mvn dependency:copy-dependencies -f delta-storage-3.2.0.pom -DoutputDirectory=jars

FROM bitnami/spark:3.5.0
USER root
RUN install_packages curl
USER 1001
WORKDIR ${SPARK_HOME}/jars
ENV MAVEN_PACKAGE_VERSION=2.12
ENV SPARK_VERSION=3.5.0
RUN curl \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-sql-kafka-0-10_${MAVEN_PACKAGE_VERSION}/${SPARK_VERSION}/spark-sql-kafka-0-10_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.jar \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-avro_${MAVEN_PACKAGE_VERSION}/${SPARK_VERSION}/spark-avro_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.jar \
    --remote-name https://repo1.maven.org/maven2/org/apache/spark/spark-connect_${MAVEN_PACKAGE_VERSION}/${SPARK_VERSION}/spark-connect_${MAVEN_PACKAGE_VERSION}-${SPARK_VERSION}.jar \
    --remote-name https://repo1.maven.org/maven2/io/delta/delta-spark_${MAVEN_PACKAGE_VERSION}/3.2.0/delta-spark_${MAVEN_PACKAGE_VERSION}-3.2.0.jar \
    --remote-name https://repo1.maven.org/maven2/io/delta/delta-storage/3.2.0/delta-storage-3.2.0.jar \
    --remote-name https://repo1.maven.org/maven2/net/sourceforge/jtds/jtds/1.3.1/jtds-1.3.1.jar \
    --remote-name https://repo1.maven.org/maven2/com/microsoft/sqlserver/mssql-jdbc/12.6.1.jre11/mssql-jdbc-12.6.1.jre11.jar \
    --remote-name https://jdbc.postgresql.org/download/postgresql-42.7.5.jar
COPY --from=maven /dependencies/jars/*.jar ${SPARK_HOME}/jars/
WORKDIR ${SPARK_HOME}