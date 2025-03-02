# bind multipass in wsl
sudo ln -s '/mnt/c/Program Files/Multipass/bin/multipass.exe' /usr/bin/multipass

# get wsl ip
wsl hostname -I

# generate token
openssl rand -base64 32

# see listening ports
sudo netstat -tulpn | grep LISTEN