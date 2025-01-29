from typing import Dict
import datetime
import requests
import pandas as pd
from dgpy.dgsecrets import DigiSecret
from dgpy.dgsqlaio import DigiSql

def get_ksql_query(query: str) -> Dict:
    response = requests.post(f'{DigiSecret()["KSQL_HOST"]}/query', json = {
        'ksql': query,
        'streamsProperties': {}
        }, headers = {
            'Accept': 'application/vnd.ksql.v1+json'
        })
    response.raise_for_status()
    return response.json()

def prep_sales_dataframe(ksql_results: Dict) -> pd.DataFrame:
    df = pd.DataFrame([ row['row']['columns'] for row in ksql_results[1:] ])
    df.columns = [
        'date', 'hour',
        'gross_items', 'gmv',
        'net_items', 'nmv',
        'net_items_fcast', 'nmv_fcast'
    ]
    df = df[df['date'] == str(datetime.date.today())]
    return df

def prep_targets_dataframe() -> pd.DataFrame:
    with DigiSql() as warehouse:
        df = warehouse.to_df("""
        SELECT
            FullDate [date],
            [hour],
            SUM(NMV_Target) nmv_target,
            SUM(Items_Target) net_items_target
        FROM
            DWSNDigiKala.Sales.Hourly_Live_Targets
        GROUP BY
            FullDate,
            [hour]        
        """)
    return df

def prep_metrics(df: pd.DataFrame) -> str:
    metrics = ''
    metric_list = [
        col for col in df.columns
        if col not in [ 'date', 'hour' ]
        ]
    for metric in metric_list:
        metrics += '\n'.join(df.apply(lambda row:
          f'live_sales_{metric}'
          f'{{date="{row["date"]}", '
          f'hour="{str(row["hour"]).zfill(2)}"}}'
          f' {row[metric]}',
          axis = 'columns'))
        metrics += '\n'
    return metrics
