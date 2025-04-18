import pika
import pika.exchange_type
import pika.spec
from typing import Literal


def singleton(cls):
    _instance = {}
    def wrapper(*args, **kwargs):
        if cls not in _instance:
            _instance[cls] = cls(*args, **kwargs)
        return _instance[cls]
    return wrapper

@singleton
class DigiRabbitMQ:
    channel = None
    connection = None
    def __init__(self, host: str = 'localhost', port: int = 5672, username: str = None, password: str = None) -> None:
        self.host = host
        self.port = port 
        self.username = username 
        self.password = password 

    def __enter__(self):
        self.connect()
        return self
    
    def __exit__(self):
        self.disconnect()
        return self

    def connect(self):
        if self.username is not None and self.username.strip() != '' and self.password is not None and self.password.strip() != '':
            credentials = pika.PlainCredentials(self.username, self.password)
        else:
            credentials = None
        self.connection = pika.BlockingConnection(pika.ConnectionParameters(self.host, self.port, credentials=credentials))
        self.channel = self.connection.channel()
    
    def disconnect(self):
        if self.is_connected():
            self.connection.close()
            self.connection = None

    def is_connected(self) -> bool:
        if self.connection is None:
            return False
        return self.connection.is_open
    
    def queue_declare(self, queue='', durable=False, exclusive=False) -> str:
        result = self.channel.queue_declare(queue=queue, durable=durable, exclusive=exclusive)
        queue = result.method.queue
        return queue

    def exchange_declare(self, exchange='', exchange_type: Literal['direct', 'fanout', 'headers', 'topic'] = 'direct', durable=False):
        self.channel.exchange_declare(exchange=exchange, exchange_type=exchange_type, durable=durable)

    def queue_bind(self, queue: str, exchange: str, routing_key: str = ''):
        self.channel.queue_bind(queue=queue, exchange=exchange, routing_key=routing_key)

    def publish(self, message: str, exchange='', routing_key='', persist=False, json=False, correlation_id: str | None = None, reply_to: str | None = None):
        if not self.is_connected():
            self.connect()
        delivery_mode = pika.spec.PERSISTENT_DELIVERY_MODE if persist else None
        content_type = 'application/json' if json else None
        properties = pika.BasicProperties(delivery_mode=delivery_mode, content_type=content_type, correlation_id=correlation_id, reply_to=reply_to) 
        self.channel.basic_publish(
            exchange=exchange,
            routing_key=routing_key,
            body=message,
            properties=properties
        )

    def on_message_received_sample_callback(ch, method, properties, body):
        print(f'processing {body}')
        print(f'finished processing')
        ch.basic_ack(delivery_tag=method.delivery_tag)
        print('acknowledged message')
        ch.basic_nack(delivery_tag=method.delivery_tag)
        print('not acknowledged message')
        print(properties.reply_to, properties.correlation_id)

    def consume(self, queue: str, callback):
        self.channel.basic_qos(prefetch_count=1)
        self.channel.basic_consume(queue=queue, on_message_callback=callback)
        self.channel.start_consuming()

    def temp_queue_declare(self) -> str:
        queue = self.queue_declare(exclusive=True)
        return queue
    
    def fanout_publish(self, exchange: str, message: str):
        """
            Send message to a exchange only.
            No need to specif the queue and routing key.
            You can bind any queue to the exchange and consume messeges.
        """
        self.exchange_declare(exchange=exchange, exchange_type='fanout')
        self.publish(message=message, exchange=exchange)

    def fanout_consume(self, exchange: str, queue: str | None = None, callback = None):
        self.exchange_declare(exchange=exchange, exchange_type='fanout')
        if queue is None:
            queue = self.temp_queue_declare()
        else:
            self.queue_declare(queue=queue)
        self.queue_bind(queue=queue, exchange=exchange)
        self.consume(queue=queue, callback=callback)

    def topic_publish(self, exchange: str, message: str, routing_key: str):
        """
            Send message to a exchange with flexible routing key.
            No need to specif the queue.
            You can bind any queue to the exchange with the routing key and consume messeges.
        """
        self.exchange_declare(exchange=exchange, exchange_type='topic')
        self.publish(message=message, exchange=exchange, routing_key=routing_key)

    def topic_consume(self, exchange: str, routing_key: str, queue: str | None = None, callback = None):
        self.exchange_declare(exchange=exchange, exchange_type='topic')
        if queue is None:
            queue = self.temp_queue_declare()
        else:
            self.queue_declare(queue=queue)
        self.queue_bind(exchange=exchange, queue=queue, routing_key=routing_key)
        self.consume(queue=queue, callback=callback)
