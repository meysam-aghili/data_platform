-- Active: 1739199614877@@172.20.65.42@8010@default
CREATE DATABASE test2;

Drop TABLE if EXISTS test.my_table;
CREATE TABLE IF NOT EXISTS test.my_table on cluster cluster_nesaj(
    id UInt32,
    name String,
    timestamp DateTime
) ENGINE = MergeTree()
PARTITION BY toYYYYMM(timestamp)
ORDER BY (id, timestamp)
SETTINGS storage_policy = 's3_main';


INSERT into test.my_table VALUES(1, 'meysam', now())



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
