from fastapi import FastAPI
from fastapi.responses import PlainTextResponse
from dgpy.bielastic import BIElastic
from functions import df_to_prom_metrics

app = FastAPI()
elastic = BIElastic()

@app.get('/metrics', response_class = PlainTextResponse)
async def metrics():
    df = elastic.sql_to_df("""
        SELECT
            fields.apiSlug.keyword slug,
            fields.statusCode status,
            COUNT(*) count
        FROM
            "biapi-production-*"
        WHERE
            fields.generatedExplicitly = true
            AND "@timestamp" >= TODAY() - INTERVAL 2 DAYS
        GROUP BY
            1, 2
    """)
    metrics = df_to_prom_metrics(df, 'biapi_usage')
    return metrics
