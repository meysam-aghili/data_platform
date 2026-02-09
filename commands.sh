# ssl
sudo apt update
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d meysamaghili.ir -d www.meysamaghili.ir
sudo certbot --nginx -d git.meysamaghili.ir -d www.git.meysamaghili.ir
# or manually
sudo certbot certonly --manual --preferred-challenges dns -d git.meysamaghili.ir

# nfs 
## server side
sudo apt install nfs-kernel-server -y
sudo mkdir -p /mnt/nfs
sudo chown nobody:nogroup /mnt/nfs
sudo sudo chmod 777 /mnt/nfs
sudo nano /etc/exports
    add this: /mnt/nfs 178.131.8.158(rw,sync,no_subtree_check)
sudo exportfs -ra
sudo systemctl restart nfs-kernel-server
sudo ufw allow from 178.131.8.158 to any port 111
sudo ufw allow from 178.131.8.158 to any port 2049
sudo ufw allow from 178.131.8.158 to any port 20048  # common mountd
sudo ufw allow from 178.131.8.158 to any port 32769  # lockd/statd
## client side
sudo apt install nfs-common -y
sudo mkdir -p /mnt/nfs
sudo mount 91.107.184.134:/mnt/nfs /mnt/nfs
echo "91.107.184.134:/mnt/nfs /mnt/nfs nfs defaults 0 0" | sudo tee -a /etc/fstab

# gitlab-runner
curl -L "https://packages.gitlab.com/install/repositories/runner/gitlab-runner/script.deb.sh" | sudo bash
sudo apt install gitlab-runner
sudo usermod -aG docker gitlab-runner

# internal
docker network create --driver overlay --scope swarm --attachable data_platform || echo "network already exists. skipping."
mkdir /mnt/nfs/nginx
mkdir -p /mnt/nfs/grafana/dashboards
cd /etc/letsencrypt/live/meysamaghili.ir/
docker secret create RESUME_SSL_CERT ./fullchain.pem
docker secret create RESUME_SSL_KEY privkey.pem
cd /etc/letsencrypt/live/git.meysamaghili.ir/
docker secret create GIT_SSL_CERT ./fullchain.pem
docker secret create GIT_SSL_KEY privkey.pem
printf "postgresql+psycopg2://airflow:airflow@postgres/airflow" | docker secret create AIRFLOW_DB_CONN_URI -
printf "db+postgresql://airflow:airflow@postgres/airflow" | docker secret create AIRFLOW_DB_BACKEND_CONN_URI -
printf "airflow" | docker secret create AIRFLOW_POSTGRES_PASSWORD -
printf "123456789" | docker secret create POSTGRES_PASSWORD -

# create ssh key
ssh-keygen -t ed25519 -C "meysamaghili533@gmail.com"
