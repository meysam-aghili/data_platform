FROM jupyter/pyspark-notebook:x86_64-spark-3.5.0
USER root
RUN apt-get update && apt-get install -y nano curl unixodbc \
    && mkdir -p /work \
    && chmod 777 -R /work
WORKDIR /root
COPY requirements.txt .
RUN pip install -i https://mirror-pypi.runflare.com/simple --no-cache-dir -r requirements.txt