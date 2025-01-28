from typing import Dict, Any
import json
import random
from enum import Enum
from dgpy.dgsecrets import DigiSecret

class Cluster(Enum):
    LAB = 'lab'
    PRODUCTION = 'production'

class Service(Enum):
    RHODRY = 'rhodry'
    PROMETHEUS = 'prometheus'
    CONNECT = 'connect'

CLUSTER = Cluster(DigiSecret()['CLUSTER'].lower())

def get_config() -> Dict[str, Any]:
    with open('config.json') as config_file:
        return json.load(config_file)

def get_front_ip() -> str:
    return random.choice(
        get_config()['clusters'][CLUSTER.name.lower()]['front_ips']
        )

def get_worker_ip() -> str:
    return random.choice(
        get_config()['clusters'][CLUSTER.name.lower()]['front_ips']
        )

def get_port(service: Service) -> str:
    return get_config()['ports'][service.name.lower()]

def get_bootstrap_servers() -> str:
    return ','.join(get_config()['clusters'][
        CLUSTER.name.lower()]['bootstrap_servers'])

