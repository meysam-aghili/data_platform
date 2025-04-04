#!/bin/bash

# Check if an argument was provided
if [ $# -eq 0 ]; then
    echo "Please provide a name."
    exit 1
fi

NAME=$1
base_path="$2"

# Check if the input file exists
if [ -z "$base_path" ]; then
    base_path="."
fi

# Perform actions using the provided name
./compose_envs.sh ${base_path}/${NAME}/docker-compose.yml .env ${base_path}/${NAME}/.env
docker stack deploy platform-${NAME} --compose-file ${base_path}/${NAME}/docker-compose.prod.yml
rm ${base_path}/${NAME}/docker-compose.prod.yml

echo "Actions completed for ${NAME}"