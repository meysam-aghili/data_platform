-- Active: 1739199614877@@172.20.65.42@8122@nesaj

SELECT * FROM system.clusters;

DROP DATABASE IF EXISTS nesaj ON CLUSTER 'cluster_nesaj';
CREATE DATABASE nesaj ON CLUSTER 'cluster_nesaj';

Drop TABLE if EXISTS nesaj.sales_orders ON CLUSTER 'cluster_nesaj';
CREATE TABLE IF NOT EXISTS nesaj.sales_orders ON CLUSTER 'cluster_nesaj'(
    id UInt32,
    customer_id UInt32,
    order_date UInt32
) ENGINE = ReplicatedMergeTree('/clickhouse/tables/{database}/sales_orders', '{replica}')
PARTITION BY toYYYYMM(order_date)
ORDER BY (customer_id, id);

select * from nesaj.sales_orders;

SELECT
	disk_name,
    part_type,
    path,
    formatReadableQuantity(rows) AS rows,
    formatReadableSize(data_uncompressed_bytes) AS data_uncompressed_bytes,
    formatReadableSize(data_compressed_bytes) AS data_compressed_bytes,
    formatReadableSize(primary_key_bytes_in_memory) AS primary_key_bytes_in_memory,
    marks,
    formatReadableSize(bytes_on_disk) AS bytes_on_disk,
	*
FROM system.parts
WHERE (table = 'my_table')
