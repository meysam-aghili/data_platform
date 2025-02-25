#!/bin/bash

# Check if an argument was provided
if [ $# -eq 0 ]; then
    echo "Please provide a name."
    exit 1
fi

NAME=$1

# Perform actions using the provided name
./compose_envs.sh ./${NAME}/docker-compose.yml .env ./${NAME}/.env
docker stack deploy platform-${NAME} --compose-file ./${NAME}/docker-compose.prod.yml
rm ./${NAME}/docker-compose.prod.yml

echo "Actions completed for ${NAME}"