ARG SUPERSET_VERSION
FROM apache/superset:${SUPERSET_VERSION}
USER root
RUN apt-get update -qq && apt-get install -yqq --no-install-recommends build-essential
COPY requirements.txt .
RUN pip install -i https://mirror-pypi.runflare.com/simple -r requirements.txt
RUN apt-get autoremove -yqq --purge build-essential && apt-get clean && rm -rf /var/lib/apt/lists/* /var/cache/apt/archives/*
USER superset