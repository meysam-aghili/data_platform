from faker import Faker
import random
import time
import threading
import psycopg2
import datetime
import argparse


pg_host = "172.20.65.42"
pg_username = "postgres"
pg_password = "postgres"
fake = Faker()

def generate_customers_row_data():
    first_name = fake.first_name()
    last_name = fake.last_name()
    email = fake.email()
    address = fake.address()
    return {"first_name": first_name, "last_name": last_name, "email": email, "address": address}

def generate_products_row_data():
    name = fake.word()
    description = fake.text()
    return {"name": name, "description": description}

def generate_orders_row_data():
    customer_id = random.randint(1, 10000)
    order_date = fake.date_time_between(start_date="-1y", end_date="now").strftime("%Y-%m-%d %H:%M:%S")
    return {"customer_id": customer_id, "order_date": order_date}

def generate_order_items_row_data():
    order_id = random.randint(1, 1000000)
    product_id = random.randint(1, 1000)
    quantity = random.randint(1, 20)
    unit_price = random.randint(1000, 100000000)
    price = quantity*unit_price
    return {"order_id": order_id, "product_id": product_id, "quantity": quantity, "unit_price": unit_price, "price": price}

def generate_log_row_data():
    log_level = random.choice(["INFO", "WARNING", "ERROR"])
    message = fake.sentence()
    timestamp = datetime.datetime.now()
    device_id = random.randint(1, 20)
    return {"device_id": device_id, "log_level": log_level, "timestamp": timestamp, "message": message} 

def generate_sensor_row_data():
    sensor_id = random.randint(1, 20)
    timestamp = datetime.datetime.now()
    value = random.randint(0, 100)
    return {"sensor_id": sensor_id, "timestamp": timestamp, "value": value}

def pg_exec_query(database: str, query: str, params = None, autocommit = True, ignore_errors = False):
    conn = psycopg2.connect(host=pg_host, database=database, user=pg_username, password=pg_password)
    conn.autocommit = autocommit
    cur = conn.cursor()
    try:
        cur.execute(query, params)
        if not autocommit:
            conn.commit()
    except (Exception, psycopg2.Error) as error:
        if not autocommit:
            conn.rollback()
        if not ignore_errors:
            raise error
        print(error)
    finally:
        cur.close()
        conn.close()

def insert_row_into_pg(database: str, table_name: str, columns: tuple, row: tuple):
    pg_exec_query(database=database, query=f"INSERT INTO {table_name} {str(columns).replace("'", "")} VALUES {str(tuple(["%s" for i in range(len(columns))])).replace("'", "")}", params=row)

def etl_log_data(event: threading.Event):
    while not event.is_set(): 
        time.sleep(1)
        table_name = "logs.log_data"
        data = generate_log_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("log", table_name, columns, row)

def etl_sensor_data(event: threading.Event):
    while not event.is_set(): 
        time.sleep(1)
        table_name = "sensors.sensor_data"
        data = generate_sensor_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("iot", table_name, columns, row)

def etl_orders_data(event: threading.Event):
    while not event.is_set(): 
        time.sleep(1)
        table_name = "sales.orders"
        data = generate_orders_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("akrana", table_name, columns, row)

def etl_customers_data(event: threading.Event):
    while not event.is_set(): 
        time.sleep(1)
        table_name = "sales.customers"
        data = generate_customers_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("akrana", table_name, columns, row)

def etl_products_data(event: threading.Event):
    while not event.is_set(): 
        time.sleep(1)
        table_name = "sales.products"
        data = generate_products_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("akrana", table_name, columns, row)

def etl_order_items_data(event: threading.Event):
    while not event.is_set(): 
        time.sleep(1)
        table_name = "sales.order_items"
        data = generate_order_items_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("akrana", table_name, columns, row)

def create_pg_db():
    pg_exec_query(database="postgres", query="CREATE DATABASE iot;", ignore_errors=True)
    pg_exec_query(database="iot", query="CREATE SCHEMA sensors;", ignore_errors=True)
    pg_exec_query(database="postgres", query="CREATE DATABASE log;", ignore_errors=True)
    pg_exec_query(database="log", query="CREATE SCHEMA logs;", ignore_errors=True)
    pg_exec_query(database="postgres", query="CREATE DATABASE akrana;", ignore_errors=True)
    pg_exec_query(database="akrana", query="CREATE SCHEMA sales;", ignore_errors=True)
    
    pg_exec_query(
        database="iot", 
        query="""
        CREATE TABLE IF NOT EXISTS sensors.sensor_data
        (
			id SERIAL primary key,
            sensor_id INTEGER,
            timestamp TIMESTAMP,
            value INTEGER
        );""")
    pg_exec_query(
        database="log", 
        query="""
        CREATE TABLE IF NOT EXISTS logs.log_data
        (
			id SERIAL primary key,
            device_id INTEGER,
            log_level VARCHAR(50),
            timestamp TIMESTAMP,
            message TEXT
        );""")
    pg_exec_query(
        database="akrana", 
        query="""
        CREATE TABLE IF NOT EXISTS sales.customers
        (
            id SERIAL primary key,
            first_name VARCHAR(100),
            last_name VARCHAR(100),
            email VARCHAR(100),
            address TEXT
        );
        CREATE TABLE IF NOT EXISTS sales.products
        (
            id SERIAL primary key,
            name VARCHAR(100),
            description TEXT
        );
        CREATE TABLE IF NOT EXISTS sales.orders
        (
            id SERIAL primary key,
            customer_id INTEGER,
            order_date TIMESTAMP
        );
        CREATE TABLE IF NOT EXISTS sales.order_items
        (
            id SERIAL primary key,
            order_id INTEGER,
            product_id INTEGER,
            quantity INTEGER,
            unit_price INTEGER,
            price INTEGER
        );""")

stop_threads = threading.Event()

class CustomThread(threading.Thread):
    def __init__(self, target):
        super().__init__(target=target)
        self.daemon = True  # Allow program exit even if thread is still running

    def run(self):
        while not stop_threads.is_set():  # Check for stop event instead of infinite loop
            try:
                super().run()
            except Exception as e:
                print(f"Error in thread: {e}")
                break

def generate_inserts():

    try:
        threads = []
        for i in range(5):
            thread = CustomThread(target=lambda: etl_sensor_data(stop_threads))
            threads.append(thread)
            thread.start()
        for i in range(1):
            thread = CustomThread(target=lambda: etl_log_data(stop_threads))
            threads.append(thread)
            thread.start()        
        for i in range(5):
            thread = CustomThread(target=lambda: etl_customers_data(stop_threads))
            threads.append(thread)
            thread.start()        
        for i in range(5):
            thread = CustomThread(target=lambda: etl_products_data(stop_threads))
            threads.append(thread)
            thread.start()        
        for i in range(5):
            thread = CustomThread(target=lambda: etl_orders_data(stop_threads))
            threads.append(thread)
            thread.start()
        for i in range(5):
            thread = CustomThread(target=lambda: etl_order_items_data(stop_threads))
            threads.append(thread)
            thread.start()   
        for i in range(len(threads)):
            while True and len([t for t in threads if t.is_alive()]) == len(threads):
                pass
    except KeyboardInterrupt as e:
        stop_threads.set()	
    for t in threading.enumerate():
        if t != threading.current_thread():
            try: 
                t.join(timeout=2)
                print(f"{t.name} joined")
            except RuntimeError as e:
                print(e)
    
def main():
    parser = argparse.ArgumentParser(description='Database Creation Script')
    parser.add_argument('-initdb', help='create databases', action='store_true')
    parser.add_argument('-gen', help='generate inserts', action='store_true')
    args = parser.parse_args()
    
    if args.initdb:
        create_pg_db()
        print('initdb done')
        
    if args.gen:
        generate_inserts()
        print('generate_inserts done')

if __name__=="__main__":
    main()
    