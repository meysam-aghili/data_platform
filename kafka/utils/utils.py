from typing import Literal
import requests
import json


class KafkaRestProducer:
    
    SESSION = requests.Session()
    HEADERS_MAP = {
        "avro": {
            "Content-Type": "application/vnd.kafka.avro.v2+json",
            "Accept": "application/vnd.kafka.v2+json"
        },
        "json": {
            "Content-Type": "application/vnd.kafka.json.v2+json",
            "Accept": "application/vnd.kafka.v2+json"
        }
    }
    def __init__(self, schema_registry_url: str, rest_proxy_url: str):
        self.schema_registry_url = schema_registry_url
        self.rest_proxy_url = rest_proxy_url
    
    def get_schema(self, topic: str) -> dict:
        response = requests.get(f"{self.schema_registry_url}/subjects/{topic}-value/versions/latest")
        response.raise_for_status()
        return response.json()

    def produce(self, topic: str, payload: dict, format: Literal["json", "avro"] = "json", value_schema: dict = None):

        if format not in self.HEADERS_MAP:
            raise ValueError(f"Invalid format: {format}. Must be 'json' or 'avro'")
        
        if value_schema is None:
            value_schema_id = self.get_schema(topic)["id"]
            payload.update(
                {
                    "value_schema_id": value_schema_id
                }
            )
        else:
            payload.update(
                {
                    "value_schema": json.dumps(value_schema)
                }
            )

        response = self.SESSION.post(f"{self.rest_proxy_url}/topics/{topic}", headers=self.HEADERS_MAP[format], data=json.dumps(payload))
        response.raise_for_status()
        return response