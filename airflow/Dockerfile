FROM apache/airflow:2.10.1-python3.11
USER root
ENV TZ=Asia/Tehran
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
RUN groupadd -g 988 docker && \
    usermod -aG docker airflow
WORKDIR /app
USER airflow
COPY requirements.txt .
RUN pip install -i https://mirror-pypi.runflare.com/simple --upgrade pip && \
    pip install -i https://mirror-pypi.runflare.com/simple -r requirements.txt && \
    pip install -i https://mirror-pypi.runflare.com/simple wtforms==3.1.2