from pydantic import BaseModel, field_validator, RootModel
from datetime import datetime


class Traffic(BaseModel):
    user_id: int
    created_at: datetime
    page_url: str

    @field_validator("created_at", mode="after")
    @classmethod
    def validate_created_at(cls, value: datetime):
        today = datetime.now()
        if value > today:
            raise ValueError("created_at can not be bigger than now")
        return value

class DailyReport(RootModel[dict[str, int]]):
    pass
