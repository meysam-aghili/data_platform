curl -X POST -u elastic:elasticpassword "172.20.65.42:9200/_security/service/elastic/kibana/credential/token/token1?pretty"

curl -X POST "http://localhost:9200/_security/user/elasticuser" \
  -u elastic:elasticpassword \
  -H "Content-Type: application/json" \
  -d '{
        "password" : "elasticuser",
        "roles" : ["superuser"],
        "full_name" : "elasticuser"
      }'

curl -X GET "http://localhost:9200/_security/user" \
  -u elastic:elasticpassword \
  -H "Content-Type: application/json"


docker exec -it es01 /usr/share/elasticsearch/bin/elasticsearch-reset-password -u elastic
docker exec -it es01 /usr/share/elasticsearch/bin/elasticsearch-create-enrollment-token -s kibana