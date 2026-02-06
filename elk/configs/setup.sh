#!/bin/sh
set -e

echo 'Waiting for Elasticsearch to be ready...';
until curl -s -u elastic:elasticpassword http://elasticsearch:9200/_cluster/health | grep -q '"status":"green"'; do
  sleep 10;
  echo 'Still waiting for Elasticsearch...';
done;

echo 'Elasticsearch is ready! Setting up user passwords...';

# Set Kibana system password
curl -s -u elastic:elasticpassword -X POST http://elasticsearch:9200/_security/user/kibana_system/_password \
  -H 'Content-Type: application/json' \
  -d '{"password":"kibana_system_password"}' || echo 'Kibana password may already be set';
echo 'Kibana password setup attempted';

echo 'Creating logstash_writer role from file...';
if [ -f /roles/logstash_writer_role.json ]; then
  # Delete existing role first to avoid conflicts
  curl -s -u elastic:elasticpassword -X DELETE http://elasticsearch:9200/_security/role/logstash_writer;
  
  # Create the role
  curl -s -u elastic:elasticpassword \
    -X PUT http://elasticsearch:9200/_security/role/logstash_writer \
    -H 'Content-Type: application/json' \
    -d @/roles/logstash_writer_role.json;
  echo 'logstash_writer role created/updated!';
else
  echo 'Error: /roles/logstash_writer_role.json not found!';
  ls -la /roles/;
  exit 1;
fi;

curl -s -u elastic:elasticpassword -X POST http://elasticsearch:9200/_security/user/logstash_internal \
  -H 'Content-Type: application/json' \
  -d '{
    "password": "logstash_system_password",
    "roles": ["logstash_system", "superuser", "logstash_writer"],
    "full_name": "Logstash Internal User"
  }' || echo 'User creation may have failed';
echo 'Logstash user creation attempted';

echo 'Setup completed successfully!';