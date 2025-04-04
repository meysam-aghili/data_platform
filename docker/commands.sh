# install docker https://github.com/Gozargah/Marzban/discussions/987

# add user to docker sudo
sudo groupadd docker
sudo usermod -aG docker $USER
newgrp docker
docker image ls
reboot

# add docker configs
sudo mkdir /etc/docker/
sudo cp ./docker/daemon.json /etc/docker/
sudo systemctl daemon-reload
sudo systemctl restart docker

# backup/restore docker images
sudo docker save image:tag | gzip -c > filename.tgz
gunzip -c filename.tgz | sudo docker load
for file in *.tgz; do
    gunzip -c "$file" | sudo docker load
done

# enable docker registry

--->add this code to .bashrc or wsl.conf under boot(command="...")  sudo bash -c "echo '172.20.65.42 localregistry.com' >> /etc/hosts" , "in windows C:\Windows\System32\drivers\etc"
openssl req -x509 -newkey rsa:4096 -nodes -sha256 -days 365 -keyout domain.key -out domain.crt -subj "/CN=localregistry.com" -addext "subjectAltName = DNS:localregistry.com"
---> config docker compose registry
sudo mkdir -p /etc/docker/certs.d/localregistry.com
sudo cp domain.key /etc/docker/certs.d/localregistry.com/domain.key
sudo cp domain.crt /etc/docker/certs.d/localregistry.com/domain.cert

# push all local images to registry
for i in $(docker images | sed '1d' | awk '{print $1 ":" $2}');do 
    echo docker tag $i localregistry.com/$i;
done
for i in $(docker images | grep localregistry.com | awk '{print $1 ":" $2}');do
    echo docker push $i;
done

# docker swarm
docker swarm init --advertise-addr "172.20.65.42"
docker swarm join-token worker
docker swarm join --token SWMTKN-1-4pezkspxru37upoux3xv1yzvpnk56r2jinyot9pn1zqs2dng0c-epzekc49ivlexdv03ey291a8i 172.20.65.42:2377

# allow essential ports for docker swarm communication
sudo ufw allow 2377/tcp
sudo ufw allow 7946/tcp
sudo ufw allow 7946/udp
sudo ufw allow 4789/udp


# Allow all listening ports
sudo ufw disable
