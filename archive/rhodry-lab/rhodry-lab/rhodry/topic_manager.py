import logging
import json
from os import getenv
import pika
from dgpy.dgsecrets import DigiSecret
from json_logger import config_logger
from rhodry_topics import \
    create_topic, \
    alter_topic_config, \
    reassign_topic_partitions, \
    delete_topics
from topic_dtos import \
    TopicConfigDto, \
    TopicAssignmentDto

LOG_LEVEL = getenv('LOG_LEVEL') or 'INFO'
RABBIT_QUEUE = getenv('RABBIT_QUEUE')
RABBIT_HOST = getenv('RABBIT_HOST') or 'rhodry_rabbit'
RABBIT_PORT = getenv('RABBIT_PORT') or '5672'
RABBIT_USER = DigiSecret()['RABBIT_USER'] or 'rhodry'
RABBIT_PASS = DigiSecret()['RABBIT_PASS']

config_logger()
log = logging.getLogger(__name__)
log.setLevel(LOG_LEVEL)

def callback(channel, method, properties, body):
    command_type = properties.headers['command']
    log.info('Received command of type: %s', command_type)

    if command_type == 'create':
        dto = TopicAssignmentDto(**json.loads(body.decode()))
        create_topic(dto)
    elif command_type == 'alter':
        dto = TopicConfigDto(**json.loads(body.decode()))
        alter_topic_config(dto)
    elif command_type == 'reassign':
        dto = TopicAssignmentDto(**json.loads(body.decode()))
        reassign_topic_partitions(dto)
    elif command_type == 'delete':
        topic = body.decode()
        delete_topics(topic)
    else:
        log.warning(
            'Unrecognized command %s. '
            'Ignoring the request.', properties.headers['command'])

    channel.basic_ack(delivery_tag = method.delivery_tag)

with pika.BlockingConnection(
    pika.ConnectionParameters(
    RABBIT_HOST, RABBIT_PORT,
    credentials = pika.PlainCredentials(RABBIT_USER, RABBIT_PASS))
    ) as rabbit:
    with rabbit.channel() as channel:
        channel.queue_declare(queue = RABBIT_QUEUE, durable = True)
        channel.basic_qos(prefetch_count = 30)
        channel.basic_consume(
            queue = RABBIT_QUEUE,
            on_message_callback = callback)
        channel.start_consuming()
