import argparse
import time
import json
from kafka import KafkaProducer
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

producer = KafkaProducer(
    bootstrap_servers = 'kafka:29092',
    value_serializer = lambda v: json.dumps(v).encode('utf-8'))

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

    records = df.to_dict(orient = 'records')
    [producer.send(table, record) for record in records]

    dglogger.info(f'sleeping for {interval} seconds...')
    time.sleep(interval)
