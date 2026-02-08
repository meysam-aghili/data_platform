# run runner in background
useradd --create-home forgejo-runner
usermod -aG docker forgejo-runner
sudo forgejo-runner register
sudo mv .runner /home/forgejo-runner/
sudo chown forgejo-runner /home/forgejo-runner/.runner
sudo su forgejo-runner
forgejo-runner daemon --config /etc/forgejo-runner-config.yml > /dev/null 2>&1 &
