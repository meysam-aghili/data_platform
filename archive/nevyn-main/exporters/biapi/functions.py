from typing import List
import pandas as pd

def df_to_prom_metrics(
    df: pd.DataFrame,
    metric_name: str,
    metric_cols: List[str] = None) -> str:
    metric_cols = metric_cols or df.columns[-1:]
    exporter_metrics = ''
    for metric in metric_cols:
        for index, row in df.iterrows():
            exporter_metrics += metric_name + \
            "{" + \
            ','.join([
                f'{col}="{str(row[col])}"'
                for col in df.columns if col not in metric_cols
                ]) + \
            "}" + " " + str(row[metric]) + '\n'
    return exporter_metrics
