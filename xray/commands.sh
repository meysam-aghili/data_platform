# Generate REALITY keys:
docker run --rm -it teddysun/xray:25.6.8 xray x25519
# Private key: ENmipn5gh4dCIk3scvoLQ2Dw4kkInr6k_9ittacYA0o
# Public key: B9bI7K06INfqDueN6Xsx9jnuVkiOEQ5jYd3YbjWRvFE

# Generate random SID:
openssl rand -hex 8

# Generate UUID:
uuidgen