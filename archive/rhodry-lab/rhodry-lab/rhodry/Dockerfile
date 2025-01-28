FROM registry-git.digikala.com/bi/rhodry:base

COPY . .

ENTRYPOINT [ "bash", "-c" ]

CMD [ "uvicorn", "webserver:app", "--host", "0.0.0.0", "--port", "80" ]
