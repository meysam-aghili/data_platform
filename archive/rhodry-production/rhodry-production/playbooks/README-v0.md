## Zero to Working Setup

In this section, we'll go through the step-by-step instructions for deploying rhodry
from scratch. The instructions explain how to add new nodes to an existing cluster, as 
well as how to deploy rhodry on vanilla servers.

There are two types of servers on rhodry's architecture, and there's a corresponding
ansible group for each of them: `[front]`-end servers, and `[worker]`s. 

`[worker]`s host the essential kafka services; they each contain a kafka broker
and a connect worker. `[front]` servers contain the auxilliary services: UI tools and
monitoring, alerting and administrative services.

I'll go ahead and declare a hypothetical third, `[ansible]`, for the sake of the docs.
It is the server on which we carry out ansible plays. Preferrably choose a front-end
server as your ansible control plane. Currently, it is `rhodry-front-01`. 

#### 1. `[all]` Create users and set the hostnames

On each node, create a linux user called `rhodry`, and set the
node's hostname to `rhodry-${xx}` for `[worker]`s, and `rhodry-front-${xx}`
for `[front]`s, where `${xx}` is the two-digit number of the node,
e.g. `rhodry-front-01` or `rhodry-02`.

```bash
$ sudo useradd -s /bin/bash -m rhodry
$ sudo usermod -aG sudo rhodry
$ sudo passwd rhodry
# a prompt will appear asking for the password for user rhodry.

# for front:
$ sudo hostnamectl set-hostname rhodry-front-${xx}
# for worker:
$ sudo hostnamectl set-hostname rhodry-${xx}

# ${xx}: node number
```

#### 2. `[ansible]` Generate an SSH-key pair and copy its public key onto each server

If you are setting up from scratch, you'll need to generate an SSH-key pair
on your ansible control plane. No need to do this if you already have SSH-keys,
like when you are adding new nodes to an existing cluster.

```bash
$ ssh-keygen -t rsa
# several prompts will appear, asking for the location to store the keys,
# and the key's password. Leave all at blank (default).
# This will generate a password-less ssh-key pair at /home/rhodry/id_rsa
# and /home/rhodry/id_rsa.pub, assuming we've logged in as rhodry.
```

You then have to copy the public key of the SSH-key you've generated on your
ansible plane onto each new server. Repeat the following command for each
new server:

```bash
$ ssh-copy-id -i ~/.ssh/id_rsa.pub ${node-ip}
# wherein ${node-ip} is replaced by the resolvable dns-name of the rhodry
# server, or its ip address, e.g. ssh-copy-id -i ~/.ssh/id_rsa.pub 172.17.23.35
# A prompt will also appear asking for the password of rhodry on the worker.
```

Make sure you also do the same process for the ip address of the frontend server itself.
Ansible SSHs into each server on its inventory (even itself!) before executing
the playbook. So it'll have to allow connections from itself using its own SSH key as well.

#### 3. `[ansible]` Set up ansible

On your ansible control-plane:

```bash
$ sudo apt install -y ansible
```

You also need to have the contents of the [playbooks](/playbooks/) directory
on your ansible control-plane. I'd suggest cloning this repository, but you have
to make sure that the plane can resolve, and has access to `git.digikala.com`.

`ping git.digikala.com` and check if you can reach it using its dns name. If it
doesn't work, `ping 172.30.212.22` to check if you have network access to git.
If you can reach it using its ip, but cannot do so using the dns, you have to update
the `/etc/hosts` on ansible control plane, and add this line:
`git.digikala.com 172.30.212.22`. If you cannot ping the ip neither, check with
the network guys to make sure the servers has access to the git server.

The git ip mentioned here belongs to the http/https protocol. It seems that the
git's SSH ip is different (check with Mohsen/Kaveh). If you intend to clone the
repo using SSH (recommended), make sure you have access to that ip as well.

If you are using SSH, make sure you `cat ~/.ssh/id_rsa.pub`, copy the
contents of the public key, go to https://git.digikala.com, navigate to
[Settings -> SSH Keys](https://git.digikala.com/profile/keys) and add a new
SSH-key with your copied public key.

However you get the contents of this repository onto your control-plane,
`cd playbooks` and make sure the [inventory](/playbooks/hosts) contains
all the IP addresses of the servers in your clusters inside their proper
groups. Then follow through with the next steps.

**NOTE:** *If you wish to apply the playbooks only to your newly added servers,*
*you can create your own inventory file, e.g. `new-hosts`, and populate it with*
*`[workers]` and `[front]` groups which contain only the ip addresses of the*
*newly added servers.*

*The playbooks in this repository are written with the concept of idempotency in mind,*
*meaning running them twice on a machine will not produce unwanted side-effects. However,*
*you may take this approach as an extra precaution for not breaking the already set up servers.*

#### 4. `[ansible]` Set up the storage

`[worker]`s contain 350 GBs of storage, in addition to their 20 GBs dedicated to 
boot, swap, OS, etc.
The OS volume is expanded to 50 GBs, and the rest of the storage is mounted
onto `/data` for persisting the kafka broker's data.

`[front]` servers possess 1 TBs of storage, all of which goes to the default OS
volume.

```bash
# on playbooks directory:
$ ansible-playbook -i hosts setup-storage.yml -K

# a prompt will appear asking for BECOME password, which
# is the sudo password of rhodry on all nodes.
```

*The plays for setting up the front-end server storage haven't been*
*tested yet, but they should be fine.*

There's a post in the slack channel `bi-rhodry`, which explains the steps
for setting up the storage in bash terms.

#### 5. `[ansible]` Install dependencies

```bash
$ sudo ansible-playbook -i hosts install-dependencies.yml -K
```

#### 6. Front: Initialize Docker Swarm

*The steps pertaining to setting up swarm cluster will soon be*
*replaced by an ansible playbook.*

**NOTE:** *The datetime syncing play has not been tested yet.*

```bash
$ docker swarm init
```

This will generate a token for joining this Swarm cluster as a worker.
In our agreed setup, all our rhodry-workers will be Swarm managers.
To generate the manager join token, run this command against the rhodry
front server.

```bash
$ docker swarm join-token manager
# This will produce the following output:
# docker swarm join --token ${TOKEN} ${rhodry-manager-ip}:2377
```

Copy the generated output. We'll be executing it against every rhodry-worker
and rhodry-front server, so they'll join the cluster as managers.

#### 7. `[all]` Join the swarm cluster

```bash
$ docker swarm join --token SWMTKN-1-35dnubutdyhptcpetblxnwnkgu3kb681oxj4q8fytixwihrn3y-a70v3jw42vqblfo96w4orc771 172.17.23.34:2377
```

NOTE: The token will be different for each cluster. Make sure you generate a new token
and replace the token and ip (in case it's different) in the command above.

#### 8. Front: Label the Swarm nodes

In order to be able to control where in the cluster our Kafka services will be deployed,
we'll be assigning labels to our nodes.

You can execute all these commands from a front-end server.

```bash
# on manager
$ docker node update --label-add rhodry=front rhodry-front-${xx}
# wherin ${xx} is replaced by the number of the front-end server.

# repeat the following step for each worker
$ docker node update --label-add rhodry=worker rhodry-${xx}
# make sure you replace the ${xx} with the number of the worker
# e.g.
# docker node update --label-add worker=03 rhodry-03
```

#### 9. Front (Optional): Register a gitlab-runner for rhodry-production

If there's no previously registered production runner for rhodry, we should
first register a runner and obtain its authentication token and config file.

However, after the first-time setup, there'll be a production runner
registered as long as rhodry is based on this repository.
Unless you explicitly delete it or lose the config file, you probably
won't ever need to register a new runner, as you can use the runner config
at [/runners/config.toml](/runners/config.toml).

However, if you do wish to generate a new authentication token, execute
the following commands on a front-end machine.

```bash
# only do this if don't have access to a rhodry-production gitlab-runner
# authentication token.
$ PRIVATE_TOKEN="6pCVxvBnzcPcaXMi1pzk" # [1]
$ RUNNER_TOKEN=$(curl --header "Private-Token: ${PRIVATE_TOKEN}" https://git.digikala.com/api/v4/projects/1480 | jq -r '.runners_token')
$ sudo gitlab-runner register \
    --url https://git.digikala.com \
    --registration-token $RUNNER_TOKEN \
    --executor shell \
    --description rhodry-production \
    --tag-list rhodry-production \
    --non-interactive
```

*[1] The script above uses my own private token (user: artin.zamani). It*
*may no longer work if my gitlab account is deactivated.*
*In order to generate a private-token, go to https://git.digikala.com and navigate to*
*[Profile -> Personal Access Tokens](https://git.digikala.com/profile/personal_access_tokens)*
*and generate a new token. Make sure you check all the boxes (except the write permissions).*

You can `cat /etc/gitlab-runner/config.toml` and retrieve the runner config with the new
authentication token.
Replace [/runners/config.toml](/runners/config.toml) with the new config.

```bash
$ cat ./runners/config.toml | sudo tee /etc/gitlab-runner/config.toml > /dev/null
```

Notice that this command replaces the gitlab-runner config with the config defined in the
[/runners/config.toml](/runners/config.toml), thus removing any additional runners you've set
up on the server (which you hopefully haven't). If for any reason, you need to keep your
existing gitlab-runner config, execute the `tee` command described above with a `-a` (--append)
flag.

#### 11. Front (Active CI Node): Replace the gitlab-runner config 

```bash
# replace the contents of the /etc/gitlab-runner/config.toml with the contents of the
# /runners/config.toml (repo)
# if you have just registered a new runner, you won't need to do this step.
$ cat ./runners/config.toml >> /etc/gitlab-runner/config.toml
# as the runner is registered as a service, it should be started automatically.
# but execute this if you ever needed to start it manually:
$ sudo gitlab-runner run
```

#### 12. Run CI pipeline

While on [Rhodry's gitlab repository](https://git.digikala.com/bi/rhodry), go to CI/CD, and
click Run Pipeline. Make sure you run for production branch.

And that's it! Rhodry must be up and running now.

You can make sure everything's up and running by executing

```bash
$ watch docker stack services rhodry
```

on a node (doesn't matter which one, but prefarrably stick to `rhodry-manager` for SSH sessions).

##### A note on pulling images

By the default, when a docker stack is deployed, docker should automatically try and pull the
required images on each node. More often than not, docker gets stuck on the pull and requires
manually pulling the images.

In order to remove the burden of pulling the images each time there's an update, there's an ansible
playbook which pulls the required images for the worker and manager nodes.

```bash
$ cd playbooks
$ ansible-playbook -i hosts pull-images.yml
# A prompt will appear asking for gitlab-ci username and password.
```
