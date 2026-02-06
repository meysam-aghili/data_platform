cd certs
openssl genrsa -out gitlab.key 2048
openssl req -new -key gitlab.key -out gitlab.csr
openssl x509 -req -days 3650 -in gitlab.csr -signkey gitlab.key -out gitlab.crt
chmod 400 gitlab.key

openssl dhparam -out dhparam.pem 2048

cd registry/certs
openssl genrsa -out registry.key 2048
openssl req -new -key registry.key -out registry.csr
openssl x509 -req -days 3650 -in registry.csr -signkey registry.key -out registry.crt
chmod 400 registry.key