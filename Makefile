# step 1 build kafka
source .env
# docker login ...
docker build -t confluentinc/cp-kafka-prod:${CONFLUENT_VERSION} ./kafka/
# docker push ...

# step 2 build kafka connect
source .env
# docker login ...
docker build -t confluentinc/cp-kafka-connect-prod:${CONFLUENT_VERSION} ./kafka_connect/
# docker push ...

###################################################

# step 3 deploy kafka
compose_envs.sh ...
docker stack deploy platform-kafka --compose-file docker-compose-kafka.yml

# step 4 deploy db
compose_envs.sh ...
docker stack deploy platform-db --compose-file docker-compose-db.yml

# step 5 deploy visulizer
compose_envs.sh ...
docker stack deploy platform-visulizer --compose-file docker-compose-visulizer.yml
