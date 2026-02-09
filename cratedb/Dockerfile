FROM crate:5.10.1
RUN dnf install -y wget && \
    wget https://repo1.maven.org/maven2/io/crate/crate-jmx-exporter/1.2.0/crate-jmx-exporter-1.2.0.jar && \
    mkdir /jmxdir && cp crate-jmx-exporter-1.2.0.jar /jmxdir/