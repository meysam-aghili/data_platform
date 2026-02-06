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

# Process each provided .env file, starting from the second argument
shift # Remove the first argument (input_file)
env_files=("$@") # Store remaining arguments as array

# If no .env files provided, add default .env to the array if it exists
if [ ${#env_files[@]} -eq 0 ] && [ -f .env ]; then
    env_files=(".env")
fi

# Source each .env file in the array
for input_env_file in "${env_files[@]}"; do
    if [ -f "$input_env_file" ]; then
        echo "Processing $input_env_file"
        set -a
        source <(cat "$input_env_file" | tr -d '\r' | sed -e '/^#/d;/^\s*$/d' -e "s/'/'\\\''/g" -e "s/=\(.*\)/='\1'/g" | tr -d '"')
        set +a
    else
        echo "Warning: File '$input_env_file' not found. Skipping."
    fi
done
envsubst "$(printf '${%s} ' $(env | cut -d'=' -f1))" < "$input_file" > "${input_file%.yml}.prod.yml"
