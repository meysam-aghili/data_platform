import argparse
import time
from dgpy.dgsqlaio import DigiSql
from dgpy.dgsecrets import DigiSecret
from dgpy import dglogger

dglogger.initialize('supernova-streamer')

supernova_config = {
    "flavor": 'mysql',
    "server": f'{DigiSecret()["DG_SHARED_ADDR"]}:{DigiSecret()["DG_SHARED_PORT"]}',
    "database": 'digikala',
    "username": DigiSecret()['DG_SHARED_USER'],
    "password": DigiSecret()['DG_SHARED_PASS']
}

upstream_config = {
    "flavor": 'mysql',
    "server": 'mysql:3306',
    "database": 'supernova',
    "username": 'root',
    "password": 'x0J2_KQ3)SBu1vTY'
}

parser = argparse.ArgumentParser()
parser.add_argument('--table', '-t', help = 'table name to check', type = str)
parser.add_argument('--interval', '-i', help = 'interval in seconds to wait between each fetch', type = int, default = 10)
parser.add_argument('--max-records', '-l', help = 'maximum number of records to fetch in each query', type = int, default = 200)
parser.add_argument('--lag', help = 'how long to sleep before starting', type = int, default = 0)
args = parser.parse_args()
table = args.table
interval = args.interval
limit = args.max_records
lag = args.lag

time.sleep(lag)

first_run = True

while True:
    if first_run:
        with DigiSql(**supernova_config) as supernova:
            offset = supernova.to_df(f'SELECT MAX(`id`) AS `id` FROM {table};')['id'].iloc[0]
            dglogger.info(f'initial run. offset set to: {offset}')
        first_run = False
        continue

    dglogger.info('fetching new records...')

    with DigiSql(**supernova_config) as supernova:
        df = supernova.to_df(f'SELECT * FROM {table} WHERE `id` > {offset} LIMIT {limit};')
        if len(df) > 0:
            offset = df['id'].max()

    dglogger.info(f'found {len(df)} new records.')

    with DigiSql(**upstream_config) as upstream:
        upstream.to_sql(df, table, create_table_if_not_exists = False)

    dglogger.info(f'sleeping for {interval} seconds...')
    time.sleep(interval)
