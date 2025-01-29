ARG DGPY_VERSION

FROM registry-git.digikala.com/bi/dgpy:${DGPY_VERSION}

COPY . .

ENTRYPOINT [ "uvicorn" ]
CMD [ "webserver:app", "--reload", "--host", "0.0.0.0",  "--port", "80", "--timeout-keep-alive", "1200" ]
