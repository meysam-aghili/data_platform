import requests


host = 'http://172.20.65.42'
port = '3001'
admin_email = 'meysamaghili533@gmail.com'
user_email = 'meysamaghili533@gmail.com'
admin_first_name = 'metabase'
admin_last_name = 'metabase'
password = 'metabase111'
site_name = 'Data Platform'

endpoints = {
    'health_check': '/api/health',
    'properties': '/api/session/properties',
    'setup': '/api/setup',
    'database': '/api/database',
    'login': '/api/session',
    'user': '/api/user',
    'api_key': '/api/api-key',
    'export': '/api/ee/serialization/export'
}
for k, v in endpoints.items():
    endpoints[k] = f"{host}:{port}{v}"

clickhouse_datasource_payload = {
    "name": "ClickHouse",
    "engine": "clickhouse",
    "is_on_demand": False,
    "is_full_sync": True,
    "is_sample": False,
    "refingerprint": False,
    "auto_run_queries": True,
    "schedules": {},
    "details": {
        "host": "clickhouse-node-01",
        "port": 8123,
        "user": "clickhouse",
        "password": "clickhouse",
        "dbname": "nesaj",
        "scan-all-databases": False,
        "ssl": False,
        "tunnel-enabled": False,
        "advanced-options": False
    }
}

def healthy():
	response = requests.get(endpoints['health_check'], verify=False)
	if response.json()['status'] == 'ok':
		return True
	print('not healthy')
	return False

def setup_metabase(session: requests.Session):
    token = session.get(endpoints['properties']).json()['setup-token']
    setup_payload = {
            'token': f'{token}',
            'user': {
                'first_name': admin_first_name,
                'last_name': admin_last_name,
                'email': admin_email,
                'site_name': site_name,
                'password': password,
                'password_confirm': password
            },
            'prefs': {
                'site_name': site_name,
                'site_locale': 'en',
                'allow_tracking': False
            }
        }
    id = session.post(endpoints['setup'], json=setup_payload).json()['id']
    print(id)      
 
def login(session: requests.Session):
    login_token = session.post(endpoints['login'], json={"username": admin_email, "password": password})
    print(login_token.content)

def create_datasource(session: requests.Session, payload: dict):
    response = session.post(endpoints['database'], json=clickhouse_datasource_payload)
    print(response.content)

def create_apikey(session: requests.Session):
    payload = {
        "group_id": 2, # Administrator
        "name": "exporter_importer"
    }
    api_key = session.post(endpoints['api_key'], json=payload).json()['unmasked_key']
    print('api_key: ', api_key)
    return api_key

def export(session: requests.Session, api_key: str):
    headers = {
        'x-api-key': api_key,
    }
    payload = {
        'all_collections': True,
        'settings': True,
        'data_model': True,
        'field_values': True,
        'database_secrets': True,
        'continue_on_error': False,
        'full_stacktrace': True,
    }
    response = requests.post(endpoints['export'], headers=headers, json=payload)
    if response.status_code == 200:
        with open('metabase_data.tgz', 'wb') as file:
            file.write(response.content)
        print("Export saved as metabase_data.tgz")
    else:
        print(f"Failed to export data: {response.text}")

if __name__ == '__main__':

    if healthy():
        session = requests.Session()
        setup_metabase(session)
        login(session)
        create_datasource(session, clickhouse_datasource_payload)

        # api_key = create_apikey(session)
        # export(session, api_key)
        