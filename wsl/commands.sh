# bind multipass in wsl
sudo ln -s '/mnt/c/Program Files/Multipass/bin/multipass.exe' /usr/bin/multipass

wsl hostname -I

sfc /scannow
DISM /online /Cleanup-Image /ScanHealth
DISM /Online /Cleanup-Image /RestoreHealth 

