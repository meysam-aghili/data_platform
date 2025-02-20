#!/bin/bash

# do not use " as a character inside your env-vars (wrapping your env-vars around "s is okay though).

# Check if a filename argument is provided
if [ $# -eq 0 ]; then
    echo "Please provide a docker-compose filename as an argument."
    exit 1
fi

# Store the input filename
input_file="$1"
input_env_file="$2"

# Check if the input file exists
if [ ! -f "$input_file" ]; then
    echo "Error: File '$input_file' not found."
    exit 1
fi

if [ ! -f "$input_env_file" ]; then
    input_env_file=".env"
fi

if [ -f .env ]; then
    echo "processing .env"
    set -a
    source <(cat $input_env_file | sed -e '/^#/d;/^\s*$/d' -e "s/'/'\\\''/g" -e "s/=\(.*\)/='\1'/g" | tr -d '"')
    set +a
fi
# envsubst < docker-compose-kafka.yml > docker-compose-kafka.prod.yml
envsubst < "$input_file" > "${input_file%.yml}.prod.yml"
