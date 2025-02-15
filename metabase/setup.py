import copy
import logging
import os
import pprint
import time
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
}
for k, v in endpoints.items():
    endpoints[k] = f"{host}:{port}{v}"

db_base_payload = {
    "is_on_demand": False,
    "is_full_sync": True,
    "is_sample": False,
    "cache_ttl": None,
    "refingerprint": False,
    "auto_run_queries": True,
    "schedules": {},
    "details": {
        "host": "clickhouse",
        "port": 8123,
        "user": "default",
        "password": None,
        "dbname": "default",
        "scan-all-databases": False,
        "ssl": False,
        "tunnel-enabled": False,
        "advanced-options": False
    },
    "name": "Our ClickHouse",
    "engine": "clickhouse"
}

{
  "name": "",
  "is_on_demand": false,
  "cache_ttl": 1,
  "engine": "",
  "details": {},
  "is_full_sync": true,
  "connection_source": "admin",
  "auto_run_queries": true,
  "schedules": {
    "cache_field_values": {
      "schedule_day": "sun",
      "schedule_frame": "first",
      "schedule_hour": 1,
      "schedule_minute": 1,
      "schedule_type": "hourly"
    },
    "metadata_sync": {
      "schedule_day": "sun",
      "schedule_frame": "first",
      "schedule_hour": 1,
      "schedule_minute": 1,
      "schedule_type": "hourly"
    }
  }
}


def healthy():
	response = requests.get(endpoints['health_check'], verify=False)
	if response.json()['status'] == 'ok':
		return True
	print('not healthy')
	return False

if __name__ == '__main__':
	if healthy():
		session = requests.Session()
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
