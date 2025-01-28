from typing import List, Optional, Dict
from pydantic import BaseModel

class TopicAssignmentDto(BaseModel):
    brokers: List[int]
    configs: Optional[Dict[str, str]]
    replicated: Optional[bool]
