from pymongo import MongoClient
from datetime import datetime
from bson import ObjectId


def connect_to_mongo(host='localhost', port=27017, username=None, password=None, auth_database='admin'):
    try:
        if username and password:
            client = MongoClient(host, port, username=username, password=password, authSource=auth_database)
        else:
            client = MongoClient(host, port)
        return client
    except Exception as e:
        print(f"Failed to connect to MongoDB: {e}")
        return None

def insert_document(client, db_name, collection_name, document):
    try:
        db = client[db_name]
        collection = db[collection_name]
        result = collection.insert_one(document)
        print(f"Document inserted with ID: {result.inserted_id}")
    except Exception as e:
        print(f"Failed to insert document: {e}")

def populate_roles(client):
    db_name = "api"
    collection_name = "roles"
    document = {
        "_id": ObjectId(),
        "slug": "admin",
        "created_at": datetime(2025, 1, 1),
        "created_by": "maghili",
        "updated_at": datetime(2025, 1, 1),
        "updated_by": "maghili",
        "users": ["artinzamani", "maghili", "test"]
    }
    insert_document(client, db_name, collection_name, document)

    document = {
        "_id": ObjectId(),
        "slug": "advanced-analytics",
        "created_at": datetime(2025, 1, 1),
        "created_by": "maghili",
        "updated_at": datetime(2025, 1, 1),
        "updated_by": "maghili",
        "users": ["artinzamani", "maghili", "test"]
    }
    insert_document(client, db_name, collection_name, document)

def populate_apis(client):
    db_name = "api"
    collection_name = "apis"
    document = {
        "_id": ObjectId(),
        "slug": "sales-target-values",
        "created_at": datetime(2025, 1, 1),
        "created_by": "maghili",
        "updated_at": datetime(2025, 1, 1),
        "updated_by": "maghili",
        "users": ["artinzamani", "maghili", "test"],
        "stored_procedure": {
            "database": {
                "server": "Warehouse",
                "database": "DW"
            },
            "schema": "api",
            "stored_procedure": "sp_sales_target_values"
        },
        "encryption_key": None,
        "method": "GET",
        "unmasked": False
    }
    insert_document(client, db_name, collection_name, document)

def populate_jobs(client):
    db_name = "api"
    collection_name = "jobs" 
    document = {
        "_id": ObjectId(),
        "slug": "test-job",
        "created_at": datetime(2025, 1, 1),
        "created_by": "maghili",
        "users": ["maghili"],
        "image": "localregistry.com/python:3.9.21",
        "entrypoint": "python",
        "command": "--version",
        "detached": False,
        "secrets": {"A": "B"},
        "environment": {"C": "D"},
        "use_host_network": False,
        "networks": ["platform"]
    }
    insert_document(client, db_name, collection_name, document)

def main():
    host = '172.20.65.42'
    port = 27017
    username = 'admin'
    password = 'admin'
    auth_database = 'admin'
    client = connect_to_mongo(host, port, username, password, auth_database)
    if client:
        populate_roles(client)
        populate_apis(client)
        populate_jobs(client)
        client.close()

if __name__ == "__main__":
    main()
