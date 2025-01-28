# Migrating from ZooKeeper to KRaft

This is intended to be a step-by-step guide for upgrading Kafka version and migrating from a ZooKeeper controller cluster to the KRaft mode.

## Steps

### Updating Kafka

Before we attempt to migrate to KRaft, we should first update the Kafka version. To that end, you should first find out the version of the Kafka you are running. If we're using Confluent distribution of Apache Kafka (which we are doing in Rhodry), take a look at the [Supported Versions and Interoperability for Confluent Platform](https://docs.confluent.io/platform/current/installation/versions-interoperability.html) to find out which version of Confluent images are built on top of which versions of Kafka.

Here is the table at the time of writing:

| Confluent Platform | Apache Kafka® | Release Date     | Standard End of Support | Platinum End of Support |
|--------------------|---------------|------------------|-------------------------|-------------------------|
| 7.6.x              | 3.6.x         | February 9, 2024 | February 9, 2026        | February 9, 2027        |
| 7.5.x              | 3.5.x         | August 25, 2023  | August 25, 2025         | August 25, 2026         |
| 7.4.x              | 3.4.x         | May 3, 2023      | May 3, 2025             | May 3, 2026             |
| 7.3.x              | 3.3.x         | November 4, 2022 | November 4, 2024        | November 4, 2025        |
| 7.2.x              | 3.2.x         | July 6, 2022     | July 6, 2024            | July 6, 2025            |
| 7.1.x              | 3.1.x         | April 5, 2022    | April 5, 2024           | April 5, 2025           |
| 7.0.x              | 3.0.x         | October 27, 2021 | October 27, 2023        | October 27, 2024        |
| 6.2.x              | 2.8.x         | June 8, 2021     | June 8, 2023            | June 8, 2024            |
| 6.1.x              | 2.7.x         | February 9, 2021 | February 9, 2023        | February 9, 2024        |

As an example, I'll try to imitate the procedure we went through while migrating Rhodry, which was initially set up using the Confluent's `7.1.1-ce (Commit:c70f323bfaccf78e)` set of images, which were built on top of Kafka v3.1.x.

#### 1. Set `inter.broker.protocol.version` to the current version and update

The first step is to set the config for `inter.broker.protocol.version` according to the current version of Kafka that is in use (in our example, that'd be `3.1`). The config cat be be set by the environment variable `KAFKA_INTER_BROKER_PROTOCOL_VERSION`.

At the same time, update the images to version you are upgrading to. Commit the changes to update the setup and run the cluster with new images, but with the `inter.broker.protocol.version` set to the previous version.

#### 2. Update the images 

After the cluster has updated using the new images, set the `inter.broker.protocol.version` to the version you've upgraded to, ie. `3.6`.

Commit the changes to commence the upgrade.

### Migrating from ZooKeeper to KRaft

At the time of this writing (and probably in the times to come), the most reliable reference for how to migrate from ZK to KRaft can be found at Apache's official doc on [ZooKeeper to KRaft Migration](https://kafka.apache.org/documentation/#kraft_zk_migration), so refer to this at all times for the official instructions.

#### 1. Get the Cluster ID

You must find out the cluster ID that identifies your Kafka cluster.

`GET rest-proxy:8081/v3/clusters`

returns a JSON payload that contains your cluster-id at `.data[0].cluster_id`. Keep it as we're going to be using that ID.

#### 2. Setup the storage for the logs

Create a directory in the data disk for the controller nodes.

```bash
ansible all -i hosts --become -m shell -a 'mkdir /data/kafka-controller/; chmod 777 -R /data/kafka-controller;' -K
```

#### 3. Provision the controller cluster

Bootstrap the Kafka controller clusters. On the lab cluster, we used the following configuration:

```yml
x-kafka-controller-common: &kafka-controller-common
  image: ${CI_REGISTRY_IMAGE}/confluentinc/cp-kafka:${CONFLUENT_VERSION}
  <<: *network
  volumes:
    - /data/kafka-controller:/var/lib/kafka/data

x-kafka-controller-envs: &kafka-controller-envs
  KAFKA_ZOOKEEPER_METADATA_MIGRATION_ENABLE: 'true'
  KAFKA_PROCESS_ROLES: controller
  KAFKA_CONTROLLER_QUORUM_VOTERS: 1001@kafka-controller-01:29061,1002@kafka-controller-02:29062,1003@kafka-controller-03:29063,1004@kafka-controller-04:29064,1005@kafka-controller-05:29065
  KAFKA_CONTROLLER_LISTENER_NAMES: CONTROLLER
  KAFKA_ZOOKEEPER_CONNECT: zookeeper-01:2181/kafka
  KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,CONTROLLER:PLAINTEXT
  KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 5
  KAFKA_HEAP_OPTS: '-Xms1g -Xmx1g'
  CLUSTER_ID: YqLu9-0pSQ6QQCZqFsJlVg

services:
  kafka-controller-01:
    <<: *kafka-controller-common
    deploy:
      placement:
        constraints:
          - node.hostname == rhodrylab-01
    ports:
      - 29061:29061
    environment:
      <<: *kafka-controller-envs
      KAFKA_NODE_ID: 1001
      KAFKA_LISTENERS: CONTROLLER://0.0.0.0:29061

--
```

**Please note that the the node-ids assigned to the controllers must differ from the broker-ids assigned to the actual data brokers, as they share the same ID-namespace.**

After committing this setup, the quorum cluster will initiate the loading phase where the metadata are loaded from the ZooKeeper.

Now we need to tell the brokers that there's a mayor in town...

#### 4. Update the brokers to KRaft brokers

Here are the environment variables that were updated for the actual data brokers:

```yml
KAFKA_ZOOKEEPER_METADATA_MIGRATION_ENABLE: 'true'
KAFKA_PROCESS_ROLES: broker
KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,CONTROLLER:PLAINTEXT
KAFKA_CONTROLLER_QUORUM_VOTERS: 1001@kafka-controller-01:29061,1002@kafka-controller-02:29062,1003@kafka-controller-03:29063,1004@kafka-controller-04:29064,1005@kafka-controller-05:29065
KAFKA_CONTROLLER_LISTENER_NAMES: CONTROLLER
CLUSTER_ID: YqLu9-0pSQ6QQCZqFsJlVg

```

Aside from these, we need to replace `KAFKA_BROKER_ID` with `KAFKA_NODE_ID`.
