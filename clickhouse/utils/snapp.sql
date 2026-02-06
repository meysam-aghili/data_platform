CREATE DATABASE snapp ON CLUSTER cluster_nesaj;

--DROP TABLE snapp.traffic ON CLUSTER cluster_nesaj
CREATE TABLE IF NOT EXISTS snapp.traffic ON CLUSTER cluster_nesaj
(
    request_id UInt64 NOT NULL COMMENT 'if of the request',
    user_id UInt32 NOT NULL COMMENT 'id of the user',
    created_at DateTime NOT NULL COMMENT 'datetime of the visit',
    page_url String NOT NULL COMMENT 'visited webapp url',
) 
ENGINE = ReplicatedReplacingMergeTree('/clickhouse/tables/{shard}/snapp_traffic', '{replica}', request_id)
PARTITION BY toDate(created_at)
ORDER BY (user_id,created_at)
SETTINGS index_granularity = 8192
COMMENT 'traffic log for page views';

insert into snapp.traffic values(1, 1, now(), '/');

select * from snapp.traffic;

select * FROM system.clusters;
OPTIMIZE TABLE myThirdReplacingMT FINAL CLEANUP;
  
  


