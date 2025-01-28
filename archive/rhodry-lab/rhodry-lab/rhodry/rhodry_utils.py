import subprocess
from typing import Tuple, List
import re

def sanitize_field_name(field_name: str) -> str:
    avro_friendly_field_name = field_name.replace('-', '_')
    if bool(re.match(r'^\d', avro_friendly_field_name)):
        avro_friendly_field_name = f'_{avro_friendly_field_name}'
    return avro_friendly_field_name

def execute(command: List[str]) -> Tuple[str, str]:
    result = subprocess.run(command, capture_output = True)
    if result.returncode == 0:
        return result.stdout.decode('utf-8')
    else:
        raise Exception(result.stderr.decode('utf-8'))
