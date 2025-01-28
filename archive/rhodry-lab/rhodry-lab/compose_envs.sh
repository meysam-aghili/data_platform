#!/bin/bash

# do not use " as a character inside your env-vars (wrapping your env-vars around "s is okay though).

if [ -f .env ]; then
    echo "processing .env"
    set -a
    source <(cat .env | sed -e '/^#/d;/^\s*$/d' -e "s/'/'\\\''/g" -e "s/=\(.*\)/='\1'/g" | tr -d '"')
    set +a
fi
for env in "$@"
do
    if [ -f "$env" -a ! -z "$1" ]; then
        echo "processing $env"
        set -a
        source <(cat $env | sed -e '/^#/d;/^\s*$/d' -e "s/'/'\\\''/g" -e "s/=\(.*\)/='\1'/g" | tr -d '"')
        set +a
    fi
done
envsubst < docker-compose.yml > docker-compose.prod.yml
