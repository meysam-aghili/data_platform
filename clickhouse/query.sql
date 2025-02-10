-- Active: 1739199614877@@172.20.65.42@8122@default

SELECT *
FROM system.clusters

DROP DATABASE IF EXISTS test ON CLUSTER 'cluster_nesaj';
CREATE DATABASE test ON CLUSTER 'cluster_nesaj';

Drop TABLE if EXISTS test.my_table ON CLUSTER 'cluster_nesaj';
CREATE TABLE IF NOT EXISTS test.my_table ON CLUSTER 'cluster_nesaj'(
    id UInt32,
    name String,
    timestamp DateTime
) ENGINE = ReplicatedMergeTree('/clickhouse/tables/{database}/my_table', '{replica}')
PARTITION BY toYYYYMM(timestamp)
ORDER BY (id, timestamp)
SETTINGS storage_policy = 's3_main';


INSERT into test.my_table VALUES(1, 'meysam', now())

select * from test.my_table


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
