import os
import requests
import logging
import subprocess
import ast
import sys


# https://jupyterhub-dockerspawner.readthedocs.io/en/latest/api/index.html

def get_secret(name: str) -> str:
    with open(f'/run/secrets/{name}', 'r') as secret:
        return secret.read().replace('\n', '')

def get_notebook_images():
    registry_url = os.environ.get('DOCKER_REGISTRY_URL')
    jupyterhub_images = []
    images = ast.literal_eval(
        subprocess.run(["curl", "-k", "https://registry/v2/_catalog"], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True).stdout
        )["repositories"]
    for image in images:
        if "notebook" in image:
            tags = ast.literal_eval(
                subprocess.run(["curl", "-k", f"https://registry/v2/{image}/tags/list"], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True).stdout
            )["tags"]
            for tag in tags:
                jupyterhub_images.append(f"{registry_url}/{image}:{tag}")
    return jupyterhub_images

def image_selector(spawner) -> str:
    html = '<label for="image">Image</label>\n'
    html += '<select name="image" id="image">\n'
    for image in get_notebook_images():
        html += f'<option value="{image}">{image}</option>\n'
    html += '</select>'
    return html

def create_notebook_directory_hook(spawner):
    username = spawner.user.escaped_name.replace('.', '-2e')
    os.umask(0)
    os.makedirs(
        f'/mnt/nfsdir/notebooks-{username}',
        exist_ok = True,
        mode = 0o777)

c.ConfigurableHTTPProxy.should_start = False
c.ConfigurableHTTPProxy.api_url = 'http://spark-jupyterhub-proxy:8001'
c.JupyterHub.log_level = logging.DEBUG
c.JupyterHub.hub_ip = '0.0.0.0'
c.JupyterHub.hub_connect_ip = 'spark-jupyterhub'
c.JupyterHub.spawner_class = 'dockerspawner.SwarmSpawner'
c.DockerSpawner.prefix = 'jupyterhub'
c.DockerSpawner.options_form = image_selector
c.DockerSpawner.allowed_images = '*'
c.DockerSpawner.image = os.environ.get('DOCKER_NOTEBOOK_IMAGE')
c.DockerSpawner.cmd = [ 'jupyterhub-singleuser', '--allow-root' ]
c.DockerSpawner.notebook_dir = os.environ.get('DOCKER_NOTEBOOK_DIR')
c.SwarmSpawner.network_name = os.environ.get('DOCKER_NETWORK_NAME')
c.SwarmSpawner.spawn_timeout = 60
c.DockerSpawner.remove = True
c.DockerSpawner.remove_containers = True
c.DockerSpawner.volumes = {
    os.environ.get('HOST_NOTEBOOKS_DIR')+'/notebooks-{username}': os.environ.get('DOCKER_NOTEBOOK_DIR')
}
c.Spawner.pre_spawn_hook = create_notebook_directory_hook
# c.DockerSpawner.extra_host_config.update({
#     'extra_hosts': {
#         'kafka-01': '172.17.23.146',
#         'kafka-02': '172.17.23.147',
#         'kafka-03': '172.17.23.148',
#         'kafka-04': '172.17.23.149',
#         'kafka-05': '172.17.23.150',
#     }
# })

c.JupyterHub.authenticator_class = 'dummy'

# ldap config
# c.JupyterHub.authenticator_class = 'ldapauthenticator.LDAPAuthenticator'
# c.LDAPAuthenticator.server_address = os.getenv('LDAP_SERVER')
# c.LDAPAuthenticator.server_port = int(os.getenv('LDAP_PORT') or 636)
# c.LDAPAuthenticator.use_ssl = True
# c.LDAPAuthenticator.lookup_dn = True
# c.LDAPAuthenticator.lookup_dn_user_dn_attribute = os.getenv('LDAP_USER_ATTR')
# c.LDAPAuthenticator.lookup_dn_search_filter = '({login_attr}={login})'
# c.LDAPAuthenticator.lookup_dn_search_user = os.getenv('LDAP_BIND_DN')
# c.LDAPAuthenticator.lookup_dn_search_password = get_secret('LDAP_BIND_PASS')
# c.LDAPAuthenticator.user_search_base = os.getenv('LDAP_SEARCH_BASE')
# c.LDAPAuthenticator.user_attribute = os.getenv('LDAP_USER_ATTR')
# c.LDAPAuthenticator.admin_users = { 'artin.zamani', 'm.bolhasani' }

c.JupyterHub.load_roles = [
    {
        "name": "jupyterhub-idle-culler-role",
        "scopes": [
            "list:users",
            "read:users:activity",
            "read:servers",
            "delete:servers",
            # "admin:users", # if using --cull-users
        ],
        # assignment of role's permissions to:
        "services": ["jupyterhub-idle-culler-service"],
    }
]
c.JupyterHub.services = [
    {
        "name": "jupyterhub-idle-culler-service",
        "command": [
            sys.executable,
            "-m", "jupyterhub_idle_culler",
            "--timeout=3600",
        ],
        # "admin": True,
    }
]
