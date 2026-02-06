from fastapi import FastAPI
from .routes import router as traffic_router
from .settings import settings
import contextlib


@contextlib.asynccontextmanager
async def lifespan(app: FastAPI):
    print("settings: ", settings)
    print("start...")
    yield

app = FastAPI(lifespan=lifespan, title="Clickhouse Service")

app.include_router(traffic_router, prefix="/api/traffic", tags=["traffic"])

@app.get("/health")
def root():
    return {"status": "ok"}
