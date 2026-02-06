from fastapi import APIRouter, HTTPException
from .models import DailyReport, Traffic
from .db import client


router = APIRouter()

@router.post("/{request_id}")
def write_traffic(request_id: int, entry: Traffic) -> dict[str, str]:
    try:
        print(entry.created_at)
        client.command(
            """
            INSERT INTO snapp.traffic (request_id, user_id, created_at, page_url)
            VALUES (%d, %d, %s, %s)
            """,
            (request_id, entry.user_id, entry.created_at, entry.page_url),
        )
        return {"status": "ok"}
    except Exception as e:
        raise HTTPException(500, str(e))

@router.get("/daily/{user_id}", response_model=DailyReport)
def daily_report(user_id: int) -> DailyReport:
    try:
        rows = client.query(
            """
            SELECT page_url, count() AS cnt
            FROM snapp.traffic
            WHERE user_id = %(user_id)d
              AND created_at >= now() - INTERVAL 1 DAY
            GROUP BY page_url
            """,
            parameters={"user_id": user_id},
        ).result_rows

        report = {url: count for url, count in rows}
        print(report)
        return DailyReport(report)

    except Exception as e:
        raise HTTPException(500, str(e))
