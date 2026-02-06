import requests
import pandas as pd

baseUrl = "https://api.tgju.org/v1/market/indicator/summary-table-data/"
cryptos = ["crypto-bitcoin"]

df = pd.DataFrame()
for crypto in cryptos:
    response = requests.get(baseUrl + crypto)
    data = response.json()["data"]
    df = pd.concat(
        [
            df,
            pd.DataFrame(data, 
                columns=["open", "lowest", "highest", "last", "changed", "changed_percent", "date", "persian_date"]
            ).drop(["changed", "changed_percent"], axis=1).assign(title = crypto)
        ]
    )

print(df.head())