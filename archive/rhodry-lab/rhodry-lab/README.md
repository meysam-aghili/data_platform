![rhodry](/docs/images/rhodry-512.png "Rhodry Logo")

> BI's wyrd is Rhodry's wyrd.


# Rhodry

BI's Kafka infrastructure.


## Images

Images and their aliases.

| Image                           | Alias / Base Image
|---------------------------------|-----------------------------------------|
| `rhodry/zookeeper:latest`       | `zookeeper:7.1.1`                       |
| `rhodry/kafka:latest`           | `confluentinc/cp-server:7.1.1`          |
| `rhodry/rest:latest`            | `confluentinc/cp-kafka-rest:7.1.1`      |
| `rhodry/schema-registry:latest` | `confluentinc/cp-schema-registry:7.1.1` |
| `rhodry/connect:*`              | `debezium/connect:1.9` *                |

The connect image is built from its corresponding image with additional plugins.


## Topology
![topology](/docs/images/topology.png "Kafka and Kafka Connect WIP Cluster Topology")


## References

* [Confluent's Kafka env reference](https://github.com/confluentinc/cp-demo/blob/7.1.0-post/docker-compose.yml)
* [Confluent's JDBC Sink connector reference](https://docs.confluent.io/kafka-connect-jdbc/current/sink-connector/sink_config_options.html)
* [Confluent's Connect REST API reference](https://docs.confluent.io/platform/current/connect/references/restapi.html)
* [Debezium's env reference](https://github.com/debezium/docker-images/blob/main/connect-base/0.3/README.md)
* [Debezium's tutorial on CDC](https://debezium.io/documentation/reference/stable/tutorial.html)
* [Debezium's tutorial on CDC and Sink](https://debezium.io/blog/2017/09/25/streaming-to-another-database/)
* [Debezium's unwrap messages example](https://github.com/debezium/debezium-examples/tree/main/unwrap-smt)
* [Debezium's unwrap messages docs](https://debezium.io/documentation/reference/stable/transformations/event-flattening.html)
* [Confluent's guidelines on Kafka cluster hardware setup](https://docs.confluent.io/platform/current/kafka/deployment.html)
* [Confluent's guidelines on Kafka cluster architecture](https://docs.confluent.io/platform/current/multi-dc-deployments/multi-region-architectures.html)
