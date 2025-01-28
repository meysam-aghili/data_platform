from typing import Dict
from fastapi import FastAPI
from topic_dtos import TopicAssignmentDto
from rhodry_topics import \
    delete_topics, \
    alter_topic_config, \
    create_topic

app = FastAPI()

@app.put('/v2/topics/{topic}')
async def put_create_topic(topic: str, dto: TopicAssignmentDto):
    create_topic(topic, dto)

@app.patch('/v2/topics/{topic}')
async def patch_alter_topic_configs(topic: str, configs: Dict[str, str]):
    alter_topic_config(topic, configs)

@app.delete('/v2/topics/{topic}')
async def delete_delete_topics(topic: str):
    delete_topics(topic.split(','))
