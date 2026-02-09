--->add this code to .bashrc or wsl.conf under boot(command="...")  sudo bash -c "echo '172.20.65.42 localregistry.com' >> /etc/hosts" , "in windows C:\Windows\System32\drivers\etc"
openssl req -x509 -newkey rsa:4096 -nodes -sha256 -days 365 -keyout domain.key -out domain.crt -subj "/CN=localregistry.com" -addext "subjectAltName = DNS:localregistry.com"

ldapwhoami -H ldaps://localregistry.com:636 -x -D "cn=admin,dc=dataplatform,dc=com" -w adminpassword
ldapsearch -x -H ldap://localhost:389 -b "dc=dataplatform,dc=com" -D "cn=admin,dc=dataplatform,dc=com" -w adminpassword "(uid=maghili)"