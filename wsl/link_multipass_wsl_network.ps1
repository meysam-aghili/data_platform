# Run in windows
# for more information visit: https://theko2fi.github.io/wsl2/2023/05/01/WSL2-Access-to-Multipass-VMs.html
# Get-NetIPInterface | where AddressFamily -eq "IPv4" | select ifIndex,InterfaceAlias,AddressFamily,ConnectionState,Forwarding | Sort-Object -Property IfIndex | Format-Table
Set-NetIPInterface -ifAlias "vEthernet (WSL (Hyper-V firewall))" -Forwarding Enabled
Set-NetIPInterface -ifAlias "vEthernet (Default Switch)" -Forwarding Enabled