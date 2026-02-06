-- Active: 1730722459444@@172.20.65.42@8123@nesaj

SELECT * FROM system.clusters;

DROP DATABASE IF EXISTS nesaj ON CLUSTER 'cluster_nesaj';
CREATE DATABASE nesaj ON CLUSTER 'cluster_nesaj';

--------------------------------------------------------------------------------------------------------

Drop TABLE if EXISTS nesaj.orders ON CLUSTER 'cluster_nesaj';
CREATE TABLE IF NOT EXISTS nesaj.orders ON CLUSTER 'cluster_nesaj'
(
    id Int32,
    customer_id Int32,
    order_date TIMESTAMP
) ENGINE = ReplicatedReplacingMergeTree('/clickhouse/tables/{database}/orders', '{replica}')
PARTITION BY toYYYYMM(order_date)
ORDER BY (customer_id, id);

--------------------------

Drop TABLE if EXISTS nesaj.products ON CLUSTER 'cluster_nesaj';
CREATE TABLE IF NOT EXISTS nesaj.products ON CLUSTER 'cluster_nesaj'
(
    id Int32,
    name String,
    description String
) ENGINE = ReplicatedReplacingMergeTree('/clickhouse/tables/{database}/products', '{replica}')
ORDER BY (id);

--------------------------

Drop TABLE if EXISTS nesaj.customers ON CLUSTER 'cluster_nesaj';
CREATE TABLE IF NOT EXISTS nesaj.customers ON CLUSTER 'cluster_nesaj'
(
    id Int32,
    first_name String,
    last_name String,
    email String,
    address String
) ENGINE = ReplicatedReplacingMergeTree('/clickhouse/tables/{database}/customers', '{replica}')
ORDER BY (id);

--------------------------

Drop TABLE if EXISTS nesaj.order_items ON CLUSTER 'cluster_nesaj';
CREATE TABLE IF NOT EXISTS nesaj.order_items ON CLUSTER 'cluster_nesaj'
(
    id Int32,
    order_id Int32,
    product_id Int32,
    quantity Int32,
    unit_price Int32,
    price Int32
) ENGINE = ReplicatedReplacingMergeTree('/clickhouse/tables/{database}/order_items', '{replica}')
ORDER BY (product_id,order_id,id);

select * from nesaj.products;
select * from nesaj.orders;
select * from nesaj.customers;
select * from nesaj.order_items;

select count() from nesaj.customers;


DROP TABLE IF EXISTS sales ON CLUSTER 'cluster_nesaj';
CREATE TABLE IF NOT EXISTS sales ON CLUSTER 'cluster_nesaj'(
    order_item_id Int32,
    order_id Int32,
    customer_id Int32,
    product_id Int32,
    quantity Int32,
    unit_price Int32,
    price Int32,
    order_date TIMESTAMP,
    customer_name String,
    product_name String,
) ENGINE = ReplicatedReplacingMergeTree('/clickhouse/tables/{database}/sales', '{replica}')
PARTITION BY toYYYYMM(order_date)
ORDER BY (product_id, customer_id, order_id, order_item_id);

CREATE MATERIALIZED VIEW sales_mv ON CLUSTER 'cluster_nesaj' TO sales AS
select
    oi.id as order_item_id
    ,oi.order_id
    ,o.customer_id
    ,oi.product_id
    ,oi.quantity
    ,oi.unit_price
    ,oi.price
    ,o.order_date
    ,concat(c.first_name, c.last_name) as customer_name
    ,p.name as product_name
from order_items oi
join orders o on o.id = oi.order_id
join customers c on c.id = o.customer_id
join products p on p.id = oi.product_id;

INSERT into sales
select
    oi.id as order_item_id
    ,oi.order_id
    ,o.customer_id
    ,oi.product_id
    ,oi.quantity
    ,oi.unit_price
    ,oi.price
    ,o.order_date
    ,concat(c.first_name, c.last_name) as customer_name
    ,p.name as product_name
from order_items oi
join orders o on o.id = oi.order_id
join customers c on c.id = o.customer_id
join products p on p.id = oi.product_id;


--------------------------------------------------------------------------------------------------------------

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

SET send_logs_level='trace'
SET stream_like_engine_allow_direct_select = 1