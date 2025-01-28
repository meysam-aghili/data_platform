# Journal | Upgrading Production Cluster Kafka Version

Here's the journal for the second attempt at updating the production cluster.
We'll forego KRaft and try to update the Zoo-Keeper based cluster as is, from Kafka v3.1 (Confluent v7.1) to v3.7 (Confluent v7.7).

*P.S. Confluent has just published the v7.8 of its services; compatible to Kafka v3.8.*
*So if we're going to go through the trouble, it's better if we jump straight to that*
*version, even though this experiment was done on v7.7.*

We started with a clone of the production cluster and modified it minimally; removing unnecessary
services and reducing the number of nodes from 18 to 5.

## Starting Setup

Here's the setup that we started with; mimicing production setup on Confluent v7.1.

```yml
version: '3.7'

x-network: &network
  networks:
    - rhodry

x-deploy-on-front: &deploy-on-front
  deploy:
    replicas: 1
    placement:
      constraints: [node.hostname==rhodrylab-01]

x-zookeeper-envs: &zookeeper-envs
  ZOO_TICK_TIME: 2000
  ### ZOO_SERVERS REDUCED TO 1
  ZOO_SERVERS: server.1=zookeeper-01:12888:13888;12181

x-kafka-envs: &kafka-envs
  ### ZOO_SERVERS REDUCED TO 1
  KAFKA_ZOOKEEPER_CONNECT: zookeeper-01:12181/kafka
  # KAFKA_METRIC_REPORTERS: io.confluent.metrics.reporter.ConfluentMetricsReporter
  KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 3
  KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS: 0
  KAFKA_CONFLUENT_LICENSE_TOPIC_REPLICATION_FACTOR: 1
  KAFKA_CONFLUENT_BALANCER_TOPIC_REPLICATION_FACTOR: 3
  KAFKA_TRANSACTION_STATE_LOG_MIN_ISR: 1
  KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR: 3
  KAFKA_JMX_PORT: 9101
  KAFKA_CONFLUENT_SCHEMA_REGISTRY_URL: http://schema-registry:8081
  # CONFLUENT_METRICS_REPORTER_BOOTSTRAP_SERVERS: kafka-01:29091,kafka-02:29092,kafka-03:29093,kafka-04:29094,kafka-05:29095,kafka-06:29096,kafka-07:29097,kafka-08:29098,kafka-09:29099,kafka-10:29190,kafka-11:29191,kafka-12:29192,kafka-13:29193,kafka-14:29194,kafka-15:29195,kafka-16:29196,kafka-17:29197,kafka-18:29198
  # CONFLUENT_METRICS_REPORTER_TOPIC_REPLICAS: 1
  # CONFLUENT_METRICS_ENABLE: 'true'
  CONFLUENT_SUPPORT_CUSTOMER_ID: 'anonymous'
  KAFKA_FETCH_MAX_BYTES: ${MAXIMUM_BYTES_ALLOWED_PER_MESSAGE}
  KAFKA_REPLICA_FETCH_MAX_BYTES: ${MAXIMUM_BYTES_ALLOWED_PER_MESSAGE}
  KAFKA_MESSAGE_MAX_BYTES: ${MAXIMUM_BYTES_ALLOWED_PER_MESSAGE}
  KAFKA_REPLICA_FETCH_RESPONSE_MAX_BYTES: ${MAXIMUM_BYTES_ALLOWED_PER_MESSAGE}
  
  KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT_INT:PLAINTEXT,PLAINTEXT_EXT:PLAINTEXT,SASL_EXT:SASL_PLAINTEXT,SASL_INT:SASL_PLAINTEXT
  KAFKA_INTER_BROKER_LISTENER_NAME: SASL_INT
  KAFKA_OPTS:
    -Djava.security.auth.login.config=/run/secrets/KAFKA_JAAS
  KAFKA_SASL_MECHANISM_INTER_BROKER_PROTOCOL: PLAIN
  KAFKA_SASL_ENABLED_MECHANISMS: PLAIN
  ZOOKEEPER_SASL_ENABLED: "false"
  KAFKA_AUTHORIZER_CLASS_NAME: kafka.security.authorizer.AclAuthorizer
  KAFKA_ALLOW_EVERYONE_IF_NO_ACL_FOUND: "false"
  KAFKA_SUPER_USERS: "User:admin"
  KAFKA_CONFLUENT_REPORTERS_TELEMETRY_AUTO_ENABLE: "false"

x-zookeeper-common: &zookeeper-common
  image: ${CI_REGISTRY_IMAGE}/zookeeper
  <<: *network
  volumes:
    - /data/zookeeper/data:/data
    - /data/zookeeper/datalog:/datalog

x-kafka-common: &kafka-common
  image: ${CI_REGISTRY_IMAGE}/kafka
  <<: *network
  secrets:
    - source: KAFKA_JAAS_PRODMIGRATION
      target: KAFKA_JAAS
  volumes:
    - /data/kafka:/var/lib/kafka/data

x-connect-common: &connect-common
  image: ${CI_REGISTRY_IMAGE}/connect
  <<: *network

x-connect-envs: &connect-envs
  GROUP_ID: connect
  CONFIG_STORAGE_TOPIC: connect-configs
  OFFSET_STORAGE_TOPIC: connect-offsets
  STATUS_STORAGE_TOPIC: connect-statuses
  CONFIG_STORAGE_REPLICATION_FACTOR: 3
  OFFSET_STORAGE_REPLICATION_FACTOR: 3
  STATUS_STORAGE_REPLICATION_FACTOR: 3
  BOOTSTRAP_SERVERS: ${BOOTSTRAP_SERVERS}
  KEY_CONVERTER: io.confluent.connect.avro.AvroConverter
  VALUE_CONVERTER: io.confluent.connect.avro.AvroConverter
  CONNECT_KEY_CONVERTER_SCHEMA_REGISTRY_URL: http://schema-registry:8081
  CONNECT_VALUE_CONVERTER_SCHEMA_REGISTRY_URL: http://schema-registry:8081
  HEAP_OPTS: '-Xmx12G -Xms12G'

services:
  zookeeper-01:
    <<: *zookeeper-common
    hostname: zookeeper-01
    ports:
      - 12181:2181
      - 12888:2888
      - 13888:3888
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-01
    environment:
      <<: *zookeeper-envs
      ZOO_MY_ID: 1

  kafka-01:
    <<: *kafka-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-01
    ports:
      - 29091:29091 # Unauthenticated | Internal | For Connect, Schema registry, etc.
      - 28091:28091 # Authenticated | Interal | For broker-broker comms.
      - 27091:27091 # Unauthenticated | External | For BI clients only.
      - 26091:26091 # Authenticated | External | For the rest of the organization.
    environment:
      <<: *kafka-envs
      KAFKA_BROKER_ID: 1
      KAFKA_LISTENERS: PLAINTEXT_INT://0.0.0.0:29091,SASL_INT://0.0.0.0:28091,PLAINTEXT_EXT://0.0.0.0:27091,SASL_EXT://0.0.0.0:26091
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT_INT://kafka-01:29091,SASL_INT://kafka-01:28091,PLAINTEXT_EXT://172.17.23.146:27091,SASL_EXT://172.17.23.146:26091

  kafka-02:
    <<: *kafka-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-02
    ports:
      - 29092:29092 # Unauthenticated | Internal | For Connect, Schema registry, etc.
      - 28092:28092 # Authenticated | Interal | For broker-broker comms.
      - 27092:27092 # Unauthenticated | External | For BI clients only.
      - 26092:26092 # Authenticated | External | For the rest of the organization.
    environment:
      <<: *kafka-envs
      KAFKA_BROKER_ID: 2
      KAFKA_LISTENERS: PLAINTEXT_INT://0.0.0.0:29092,SASL_INT://0.0.0.0:28092,PLAINTEXT_EXT://0.0.0.0:27092,SASL_EXT://0.0.0.0:26092
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT_INT://kafka-02:29092,SASL_INT://kafka-02:28092,PLAINTEXT_EXT://172.17.23.147:27092,SASL_EXT://172.17.23.147:26092

  kafka-03:
    <<: *kafka-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-03
    ports:
      - 29093:29093 # Unauthenticated | Internal | For Connect, Schema registry, etc.
      - 28093:28093 # Authenticated | Interal | For broker-broker comms.
      - 27093:27093 # Unauthenticated | External | For BI clients only.
      - 26093:26093 # Authenticated | External | For the rest of the organization.
    environment:
      <<: *kafka-envs
      KAFKA_BROKER_ID: 3
      KAFKA_LISTENERS: PLAINTEXT_INT://0.0.0.0:29093,SASL_INT://0.0.0.0:28093,PLAINTEXT_EXT://0.0.0.0:27093,SASL_EXT://0.0.0.0:26093
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT_INT://kafka-03:29093,SASL_INT://kafka-03:28093,PLAINTEXT_EXT://172.17.23.148:27093,SASL_EXT://172.17.23.148:26093

  kafka-04:
    <<: *kafka-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-04
    ports:
      - 29094:29094 # Unauthenticated | Internal | For Connect, Schema registry, etc.
      - 28094:28094 # Authenticated | Interal | For broker-broker comms.
      - 27094:27094 # Unauthenticated | External | For BI clients only.
      - 26094:26094 # Authenticated | External | For the rest of the organization.
    environment:
      <<: *kafka-envs
      KAFKA_BROKER_ID: 4
      KAFKA_LISTENERS: PLAINTEXT_INT://0.0.0.0:29094,SASL_INT://0.0.0.0:28094,PLAINTEXT_EXT://0.0.0.0:27094,SASL_EXT://0.0.0.0:26094
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT_INT://kafka-04:29094,SASL_INT://kafka-04:28094,PLAINTEXT_EXT://172.17.23.149:27094,SASL_EXT://172.17.23.149:26094

  kafka-05:
    <<: *kafka-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-05
    ports:
      - 29095:29095 # Unauthenticated | Internal | For Connect, Schema registry, etc.
      - 28095:28095 # Authenticated | Interal | For broker-broker comms.
      - 27095:27095 # Unauthenticated | External | For BI clients only.
      - 26095:26095 # Authenticated | External | For the rest of the organization.
    environment:
      <<: *kafka-envs
      KAFKA_BROKER_ID: 5
      KAFKA_LISTENERS: PLAINTEXT_INT://0.0.0.0:29095,SASL_INT://0.0.0.0:28095,PLAINTEXT_EXT://0.0.0.0:27095,SASL_EXT://0.0.0.0:26095
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT_INT://kafka-05:29095,SASL_INT://kafka-05:28095,PLAINTEXT_EXT://172.17.23.150:27095,SASL_EXT://172.17.23.150:26095


  connect-01:
    <<: *connect-common
    ports:
      - ${CONNECT_REST_PORT}:8083
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-01
    environment:
      <<: *connect-envs
      ADVERTISED_HOST_NAME: connect-01

  connect-02:
    <<: *connect-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-02
    environment:
      <<: *connect-envs
      ADVERTISED_HOST_NAME: connect-02

  connect-03:
    <<: *connect-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-03
    environment:
      <<: *connect-envs
      ADVERTISED_HOST_NAME: connect-03

  connect-04:
    <<: *connect-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-04
    environment:
      <<: *connect-envs
      ADVERTISED_HOST_NAME: connect-04

  connect-05:
    <<: *connect-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-05
    environment:
      <<: *connect-envs
      ADVERTISED_HOST_NAME: connect-05


  schema-registry:
    image: ${CI_REGISTRY_IMAGE}/schema-registry
    <<: *network
    <<: *deploy-on-front
    ports:
      - 8081:8081
    environment:
      SCHEMA_REGISTRY_HOST_NAME: schema-registry
      SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS: ${BOOTSTRAP_SERVERS}
      SCHEMA_REGISTRY_LISTENERS: http://0.0.0.0:8081
      SCHEMA_REGISTRY_SCHEMA_REGISTRY_INTER_INSTANCE_PROTOCOL: "http"

  rest-proxy:
    image: ${CI_REGISTRY_IMAGE}/rest
    <<: *network
    <<: *deploy-on-front
    ports:
      - ${KAFKA_REST_PORT}:8082
    environment:
      KAFKA_REST_HOST_NAME: rest-proxy
      KAFKA_REST_BOOTSTRAP_SERVERS: ${BOOTSTRAP_SERVERS}
      KAFKA_REST_LISTENERS: "http://0.0.0.0:8082"
      KAFKA_REST_SCHEMA_REGISTRY_URL: 'http://schema-registry:8081'

  kafdrop:
    image: ${CI_REGISTRY_IMAGE}/kafdrop
    <<: *deploy-on-front
    <<: *network
    environment:
      KAFKA_BROKERCONNECT: ${BOOTSTRAP_SERVERS}
      SERVER_SERVLET_CONTEXTPATH: '/'
      JVM_OPTS: '-Xms1g -Xmx1g'
      SERVER_PORT: 9000
      MANAGEMENT_SERVER_PORT: 9000
      SCHEMAREGISTRY_CONNECT: 'http://schema-registry:8081'
    ports:
      - ${KAFDROP_PORT}:9000


secrets:
  KAFKA_JAAS_PRODMIGRATION:
    external: true

networks:
  rhodry:
    external: true

```

### Setting up the ACL rules

The following commands were executed on the cluster so that external services like Connect can work with the brokers:

```bash
# List the ACL rules
kafka-acls --bootstrap-server 172.17.23.**:26091 --command-config auth.props --list

# Add the Cluster rule for ANONYMOUS user
kafka-acls --bootstrap-server 172.17.23.**:26091 --command-config auth.props --allow-principal User:ANONYMOUS --operation ALL --cluster --add

# Add the Topic rule for ANONYMOUS user
kafka-acls --bootstrap-server 172.17.23.**:26091 --command-config auth.props --allow-principal User:ANONYMOUS --operation ALL --topic=* --add

# Add the Group rule for ANONYMOUS user
kafka-acls --bootstrap-server 172.17.23.**:26091 --command-config auth.props --allow-principal User:ANONYMOUS --operation ALL --group=* --add
```

The `auth.props` file is structured like this:


```
security.protocol=SASL_PLAINTEXT
sasl.mechanism=PLAIN
sasl.jaas.config=org.apache.kafka.common.security.plain.PlainLoginModule required username="admin" password="***";
```

Now we have a cluster up and running. Moving on to the migration...

## Update Procedure

### Connect Preparation

Before attempting the update, it is advised to:

1. Stop the producers.

2. Let the consumers catch-up to the latest offsets, and
then stop them as well.

After that, head on to the next steps.

### Update Sequence

1. Add this env to kafka brokers:

```env
KAFKA_INTER_BROKER_PROTOCOL_VERSION=3.1-IV0
```

2. **(This step won't be necessary anymore, since the update is already applied**
**to the production images of version 7.1.1.)**

And prefix connect envs with `CONNECT_` (all except `HEAP_OPTS`).

```env
CONNECT_BOOTSTRAP_SERVERS
CONNECT_GROUP_ID
CONNECT_CONFIG_STORAGE_TOPIC
CONNECT_OFFSET_STORAGE_TOPIC
CONNECT_STATUS_STORAGE_TOPIC
...
```

And rename the `ADVERTISED_HOST_NAME` to `CONNECT_REST_ADVERTISED_HOST_NAME`.

And also add this env:

```env
CONNECT_LISTENERS: http://0.0.0.0:8083
CONNECT_REST_PORT: 8083
```

And also add `hostname` and `container_name` to the compose file; **otherwise connect won't work**.

3. Update the images to Confluent v7.7.1. Remember to tag Kafka connect accordingly
(it's `registry-git.digikala.com/bi/rhodry/connect:7.7.1-3.5.0-dev` at the time of
this experiment).

Also bear in mind that the Kafka image is also built in-house (Prometheus JMX agent is added). So, use the Kafka image at `registry-git.digikala.com/bi/rhodry/kafka:${CONFLUENT_VERSION}`.

Commit and publish the new configuration at this point.

4. Bump the protocol version.

`KAFKA_INTER_BROKER_PROTOCOL_VERSION: 3.7`

Commit again to publish the new configuration.

At this point, Kafka seems to be successfully updated!

## Update Connector Configurations

Since there are a few differences between Debezium configuration in the version currently used
in production versus the version we're updating to, it is advised to stop the connectors before
attempting the upgrade.

As now the version upgrade is complete, we'll go for updating the connector configurations so
they'll match the new version specifications.

### Debezium producers; from v1.9 to v3.0

Use these python functions to update the connector configuration from the current prod env
to proper v3.0 Debezium configs:

```python
def update_cdc_config_to_v3_matching_topic_prefix_to_server_name(config_v1):
    print(f'Updating {config_v1["name"]}')
    
    config_v1['topic.prefix'] = config_v1['database.server.name']

    config_v1['schema.history.internal.kafka.topic'] = config_v1['database.history.kafka.topic']
    del config_v1['database.history.kafka.topic']

    config_v1['schema.history.internal.kafka.bootstrap.servers'] = config_v1['database.history.kafka.bootstrap.servers']
    del config_v1['database.history.kafka.bootstrap.servers']

    if 'producer.request.timeout.ms' in config_v1:
        del config_v1['producer.request.timeout.ms']
    if 'producer.compression.type' in config_v1:
        del config_v1['producer.compression.type']
    if 'connector.client.config.override.policy' in config_v1:
        del config_v1['connector.client.config.override.policy']
    if 'database.server.name' in config_v1:
        del config_v1['database.server.name']
    if 'producer.linger.ms' in config_v1:
        del config_v1['producer.linger.ms']
    if 'producer.batch.size' in config_v1:
        del config_v1['producer.batch.size']

    config_v1['producer.override.compression.type'] = 'lz4'
    config_v1['producer.override.linger.ms'] = 100
    config_v1['producer.override.batch.size'] = 65536

    if 'database.history.store.only.captured.tables.ddl' in config_v1:
        del config_v1['database.history.store.only.captured.tables.ddl']
    config_v1['schema.history.internal.store.only.captured.tables.ddl'] = 'true'

    if 'sanitize.field.names' in config_v1:
        del config_v1['sanitize.field.names']
    config_v1['field.name.adjustment.mode'] = 'avro'

    # Just a cleanup thing
    config_v1['transforms.route.replacement'] = 'supernova.$2.$3'

    # These are required only for the imitation in the lab cluster
    # config_v1['name'] = 'mock-' + prod_cdc_config['name']
    # config_v1['database.server.id'] = 12332102
    # config_v1['schema.history.internal.kafka.bootstrap.servers'] = 'kafka-01:29091,kafka-02:29092,kafka-03:29093,kafka-04:29094,kafka-05:29095'

    return config_v1



def update_ct_config_to_v3_matching_topic_prefix_to_server_name(config_v1):
    print(f'Updating {config_v1["name"]}')

    # config_v1['topic.prefix'] = config_v1['database.include.list']
    config_v1['topic.prefix'] = config_v1['database.server.name']

    del config_v1['database.server.name']
    if 'acks' in config_v1:
        del config_v1['acks']

    config_v1['schema.history.internal.kafka.topic'] = config_v1['database.history.kafka.topic']
    del config_v1['database.history.kafka.topic']

    config_v1['schema.history.internal.kafka.bootstrap.servers'] = config_v1['database.history.kafka.bootstrap.servers']
    del config_v1['database.history.kafka.bootstrap.servers']

    # Just a cleanup thing
    config_v1['transforms.route.replacement'] = 'ct.$2.$3'

    if 'database.history.store.only.captured.tables.ddl' in config_v1:
        del config_v1['database.history.store.only.captured.tables.ddl']
    config_v1['schema.history.internal.store.only.captured.tables.ddl'] = 'true'

    # These are required only for the imitation in the lab cluster
    # config_v1['name'] = 'mock-' + config_v1['name']
    # config_v1['database.server.id'] = 12332101
    # config_v1['schema.history.internal.kafka.bootstrap.servers'] = 'kafka-01:29091,kafka-02:29092,kafka-03:29093,kafka-04:29094,kafka-05:29095'

    return config_v1
```

## Post-Migration

### Key schema issue

THE ISSUE WITH schema `connect.name`.

In the new Connect version, there's a slight difference in the naming of the schema namespace,
which causes a new schema to be generated for previous subject, even though the schema itself
is intact.

This is especially severe in the case that it also affects the message keys; causing in practice
a total repartition on message keys, since we (unforunately) are serializing the message keys,
and new schema id's will be different, leading them to new partitions.

Here's an examples:

*Previous version*

```json
{
    "type": "record",
    "name": "Key",
    "namespace": "ct_digikala_fulfillment_slave.digikala_fulfillment.receipts",
    "fields": [
        {
            "name": "id",
            "type": "long"
        }
    ],
    "connect.name": "ct_digikala_fulfillment_slave.digikala_fulfillment.receipts.Key"
}
```

*New version*

```json
{
    "type": "record",
    "name": "Key",
    "namespace": "ct-digikala_fulfillment-slave.digikala_fulfillment.receipts",
    "fields": [
        {
            "name": "id",
            "type": "long"
        }
    ],
    "connect.name": "ct-digikala_fulfillment-slave.digikala_fulfillment.receipts.Key"
}
```

Notice the dashes and underlines in `namespace` and `connect.name` fields?

Yup, that's a clusterfuck.

### Envelope schema issues

Due to the changes in Debezium's CDC envelope, we main run into loads of errors we have something
to do with the new schemas being incompatible with the previously defined schemas.

The easy fix is to:

1. Stop the producers.

2. Let the sinks catch up, and then stop them as well.

3. Remove all the subjects in the schema registry.

4. Resume the producers so that they begin to register the new schemas as new messages come in.

5. After a while, let the consumers resume.

### Heap Issue

We're often running into this error for the couple of connectors we have set up on lab:

```
java.lang.OutOfMemoryError: Java heap space
```

## Notes

### Idempotence

We should set `enable.idempotence=true` for Kafka after the upgrade.

Take a look at this: https://www.confluent.io/blog/exactly-once-semantics-are-possible-heres-how-apache-kafka-does-it/.

