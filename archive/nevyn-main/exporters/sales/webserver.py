from fastapi import FastAPI
from fastapi.responses import PlainTextResponse
from sales import \
    get_ksql_query, \
    prep_sales_dataframe, \
    prep_metrics, \
    prep_targets_dataframe

app = FastAPI()

@app.get('/metrics', response_class = PlainTextResponse)
async def metrics():
    results = get_ksql_query("""
        SELECT
            SPLIT(DATEHOUR, ' ')[1] DATE,
            SPLIT(DATEHOUR, ' ')[2] HOUR,
            GROSS_ITEMS,
            GMV,
            NET_ITEMS,
            NMV,
            CAST(ROUND(NET_ITEMS_FCAST, 0) AS BIGINT) NET_ITEMS_FCAST,
            CAST(ROUND(NMV_FCAST, 0) AS BIGINT) NMV_FCAST
        FROM
            LIVE;
    """)
    df = prep_sales_dataframe(results)
    df_targets = prep_targets_dataframe()
    metrics = prep_metrics(df)
    metrics += prep_metrics(df_targets)
    return metrics
