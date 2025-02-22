FROM localregistry.com/grafana/grafana:11.5.0
USER root
RUN apk add unzip; \
    wget https://github.com/grafana/clickhouse-datasource/releases/download/v4.8.1/grafana-clickhouse-datasource-4.8.1.linux_amd64.zip; \
    unzip grafana-clickhouse-datasource-4.8.1.linux_amd64.zip; \
    cp -r grafana-clickhouse-datasource /var/lib/grafana/plugins;
USER grafana