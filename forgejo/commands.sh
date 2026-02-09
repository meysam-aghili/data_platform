# run runner in background
useradd --create-home forgejo-runner
usermod -aG docker forgejo-runner
sudo forgejo-runner register
sudo mv .runner /home/forgejo-runner/
sudo chown forgejo-runner /home/forgejo-runner/.runner

# run one time
sudo su forgejo-runner
forgejo-runner daemon --config /etc/forgejo-runner-config.yml > /dev/null 2>&1 &

# or add to services
sudo nano /etc/systemd/system/forgejo-runner.service
""
    [Unit]
    Description=Forgejo Runner
    After=network.target

    [Service]
    Type=forking
    User=forgejo-runner
    Group=forgejo-runner
    WorkingDirectory=/home/forgejo-runner
    ExecStart=/bin/bash -c "/usr/local/sbin/forgejo-runner daemon --config /etc/forgejo-runner-config.yml > /dev/null 2>&1 &"
    Restart=on-failure
    RestartSec=10s

    [Install]
    WantedBy=multi-user.target
""
sudo systemctl daemon-reload
sudo systemctl enable forgejo-runner.service
sudo systemctl start forgejo-runner.service
sudo systemctl status forgejo-runner.service
sudo journalctl -u forgejo-runner -f
ps aux | grep forgejo-runner
