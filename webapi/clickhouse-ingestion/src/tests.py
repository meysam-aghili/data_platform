from fastapi.testclient import TestClient
from .main import app
from .db import client


client_app = TestClient(app)

def test_post_traffic(mocker):
    mocker.patch.object(client, "command", return_value=None)

    payload = {
        "user_id": 10,
        "created_at": "2025-01-25T12:00:00",
        "page_url": "/home"
    }

    response = client_app.post("/api/traffic/1", json=payload)
    assert response.status_code == 200
    assert response.json() == {"status": "ok"}

def test_get_daily_report(mocker):
    mock_rows = [("/", 1), ("/home", 5)]
    mock_query = mocker.patch.object(client, "query")
    mock_query.return_value.result_rows = mock_rows

    response = client_app.get("/api/traffic/daily/10")
    assert response.status_code == 200
    assert response.json() == {"/": 1, "/home": 5}

def test_health():
    response = client_app.get("/health")
    assert response.status_code == 200
    assert response.json() == {"status": "ok"}
