"""
# `rhodry_topics`

This module contains methods that are cluster-aware.
These methods are prefixed with the cluster name.
Methods prefixed with a cluster name are meant to
be executed on that cluster only. e.g.
`staging_assign_topics`.

Other methods should be told to what cluster they
belong once are called. Cluster is by default read
from `CLUSTER` env var.

The `common.topics` table in rhodry database
only keeps tab of the topics existing in the
production cluster.

"""

from collections import deque
from typing import List, Union, Dict
import rhodry_config as config
from rhodry_utils import execute
from topic_dtos import TopicAssignmentDto

def _generate_replica_assignments(
        broker_ids: List[Union[int, str]],
        replicated = True) -> str:
    """
    ## `_generate_replica_assignment`

    Generates the topic assignment which can
    be used as argument to the kafka-topics
    cli create topic command.

    Splits the topic into partitions based on the
    number of the broker_ids. If `replicated`, it
    creates a homogeneous cluster of replicated
    topic-partitions. e.g. for input broker_ids
    1, 4 and 7, it places 3 partition-replicas 
    on each broker.
    Think of it as a master with two replicas,
    but partitioned 3-way. 
    """
    brokers = deque([str(b) for b in broker_ids])
    replica_assignments = ''
    if replicated:
        for _ in broker_ids:
            replica_assignments += ':'.join(brokers)
            replica_assignments += ','
            brokers.rotate()
        return replica_assignments[:-1]
    else:
        return ','.join(brokers)

def delete_topics(topics: Union[List[str], str]) -> None:
    topic_str = topics if isinstance(topics, str) else ','.join(topics)
    command = [
        'kafka-topics', '--delete',
        '--bootstrap-server', config.get_bootstrap_servers(),
        '--topic', topic_str
    ]
    execute(command)

def create_topic(topic: str, assignment: TopicAssignmentDto) -> None:
    command = [
        'kafka-topics', '--create',
        '--bootstrap-server', config.get_bootstrap_servers(),
        '--topic', topic,
        '--replica-assignment',
        _generate_replica_assignments(assignment.brokers, assignment.replicated),
        *[
            '--config',
            f'{cfg}={topic[cfg]}'
            for cfg in assignment.configs or []
        ]
    ]
    execute(command)

def alter_topic_config(topic: str, configs: Dict[str, str]):
    command = [
        'kafka-configs', '--alter',
        '--bootstrap-server', config.get_bootstrap_servers(),
        '--entity-type', 'topics',
        '--entitiy-name', topic,
        '--add-config',
        ','.join([ f'{cfg}={configs[cfg]}' for cfg in configs ])
    ]
    execute(command)

def reassign_topic_partitions(topic: TopicAssignmentDto) -> None:
    pass
