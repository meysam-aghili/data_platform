from faker import Faker
import random
import time
import threading
import psycopg2
import datetime


pg_host = "172.20.65.42"
pg_username = "postgres"
pg_password = "postgres"
fake = Faker()

def generate_customers_row_data():
    id = random.randint(1, 10000)
    first_name = fake.first_name()
    last_name = fake.last_name()
    email = fake.email()
    address = fake.address()
    return {"id": id, "first_name": first_name, "last_name": last_name, "email": email, "address": address}

def generate_products_row_data():
    id = random.randint(1, 1000)
    name = fake.word()
    description = fake.text()
    return {"id": id, "name": name, "description": description}

def generate_orders_row_data():
    id = random.randint(1, 1000000)
    customer_id = random.randint(1, 10000)
    order_date = fake.date_time_between(start_date="-1y", end_date="now").strftime("%Y-%m-%d %H:%M:%S")
    return {"id": id, "customer_id": customer_id, "order_date": order_date}

def generate_order_items_row_data():
    id = random.randint(1, 1000000)
    order_id = random.randint(1, 1000000)
    product_id = random.randint(1, 1000)
    quantity = random.randint(1, 20)
    unit_price = random.randint(1000, 100000000)
    price = quantity*unit_price
    return {"id": id, "order_id": order_id, "product_id": product_id, "quantity": quantity, "unit_price": unit_price, "price": price}

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

def etl_log_data():
    while True:
        time.sleep(1)
        table_name = "log.log_data"
        data = generate_log_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("logs", table_name, columns, row)

def etl_sensor_data():
    while True:
        time.sleep(1)
        table_name = "iot.sensor_data"
        data = generate_sensor_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("sensors", table_name, columns, row)

def etl_orders_data():
    while True:
        time.sleep(1)
        table_name = "sales.orders"
        data = generate_orders_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("akrana", table_name, columns, row)

def etl_customers_data():
    while True:
        time.sleep(1)
        table_name = "sales.customers"
        data = generate_customers_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("akrana", table_name, columns, row)

def etl_products_data():
    while True:
        time.sleep(1)
        table_name = "sales.products"
        data = generate_products_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("akrana", table_name, columns, row)

def etl_order_items_data():
    while True:
        time.sleep(1)
        table_name = "sales.order_items"
        data = generate_order_items_row_data()
        columns = tuple(data.keys())
        row = tuple(data.values())
        insert_row_into_pg("akrana", table_name, columns, row)

def create_pg_db():
    pg_exec_query(database="postgres", query="CREATE DATABASE sensors;", ignore_errors=True)
    pg_exec_query(database="sensors", query="CREATE SCHEMA iot;", ignore_errors=True)
    pg_exec_query(database="postgres", query="CREATE DATABASE logs;", ignore_errors=True)
    pg_exec_query(database="logs", query="CREATE SCHEMA log;", ignore_errors=True)
    pg_exec_query(database="postgres", query="CREATE DATABASE akrana;", ignore_errors=True)
    pg_exec_query(database="akrana", query="CREATE SCHEMA sales;", ignore_errors=True)
    
    pg_exec_query(
        database="sensors", 
        query="""
        CREATE TABLE IF NOT EXISTS iot.sensor_data
        (
            sensor_id INTEGER,
            timestamp TIMESTAMP,
            value INTEGER
        );""")
    pg_exec_query(
        database="logs", 
        query="""
        CREATE TABLE IF NOT EXISTS log.log_data
        (
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
            id INTEGER,
            first_name VARCHAR(100),
            last_name VARCHAR(100),
            email VARCHAR(100),
            address TEXT
        );
        CREATE TABLE IF NOT EXISTS sales.products
        (
            id INTEGER,
            name VARCHAR(100),
            description TEXT
        );
        CREATE TABLE IF NOT EXISTS sales.orders
        (
            id INTEGER,
            customer_id INTEGER,
            order_date TIMESTAMP
        );
        CREATE TABLE IF NOT EXISTS sales.order_items
        (
            id INTEGER,
            order_id INTEGER,
            product_id INTEGER,
            quantity INTEGER,
            unit_price INTEGER,
            price INTEGER
        );""")
    
def main():
    create_pg_db()
    threads = []
    # for i in range(5):
    #     thread = threading.Thread(target=etl_sensor_data)
    #     threads.append(thread)
    #     thread.start()
   
    # for i in range(1):
    #     thread = threading.Thread(target=etl_log_data)
    #     threads.append(thread)
    #     thread.start()

    # for i in range(5):
    #     thread = threading.Thread(target=etl_customers_data)
    #     threads.append(thread)
    #     thread.start()

    # for i in range(5):
    #     thread = threading.Thread(target=etl_products_data)
    #     threads.append(thread)
    #     thread.start()

    for i in range(5):
        thread = threading.Thread(target=etl_orders_data)
        threads.append(thread)
        thread.start()
    
    # for i in range(5):
    #     thread = threading.Thread(target=etl_order_items_data)
    #     threads.append(thread)
    #     thread.start()

    for thread in threads:
        thread.join()
    print("Data insertion completed.")

if __name__=="__main__":
    main()
    