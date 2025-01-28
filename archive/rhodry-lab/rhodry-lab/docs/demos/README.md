# Rhodry POC

## Setup

This demo is intended as a brief showcase of what Apache Kafka is capable of, and 
a proof of the concept that we can employ it to keep an ODS in sync with the changes
of supernova.

For this demo, we'll be fetch data from supernova, push it to an upstream MySQL database,
and keep a downstream SQL Server database in sync with it using Kafka Connect connectors.

For the source connector, we'll be using Debezium's release of Kafka Connect. For syncing
the downstream SQL Server with the contents of our Kafka topic, we'll use the Confluent's
official JDBC sink connector.

## Topology
![topology](/demos/demo-1/docs/topology.png "PoC Setup")

## Getting Started
1. Make sure the rhodry is up and running; running the CI pipeline attached to
this git repository redeploys it if it's not already running.
2. Spin up a MySQL and SQL Server test database. There's a docker config in this repository
(see [here](/testdbs)). 
3. Create a database called `supernova` on the MySQL database, and another called `ods`
on the SQL Server.
4. Register the connectors using REST interface provided by Kafka Connect (in our setup deploy).
In our setup, Debezium's REST API is exposed at port `10004`.
5. Push data to the upstream MySQL database and watch the magic happen!
