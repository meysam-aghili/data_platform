curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

docker swarm init
docker swarm join-token worker
docker swarm join --token SWMTKN-1-4pezkspxru37upoux3xv1yzvpnk56r2jinyot9pn1zqs2dng0c-epzekc49ivlexdv03ey291a8i 172.20.65.42:2377