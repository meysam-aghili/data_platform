# Ansible Playbooks

The contents of this folder is intended for automation
of setting up (initial, add-ons) rhodry servers.

We'll be using ansible to automate the set-up of the new
servers.

## Setting up the cluster

What's covered here is the set of instructions for setting
up a ssh-connected cluster of servers, which is what ansible
requires for applying the operations defined in the yaml playbooks
on all the servers.

The current setup of rhodry workers is done through two steps/playbooks:

1. [setup-storage.yml](/playbooks/setup-storage.yml) which increases the OS
storage of the rhodry worker to 50GBs, creates a new volume for data
produced by Kafka, and mounting that volume onto a directory at `/data`.

2. [install-docker.yml](/playbooks/install-docker.yml) which installs Docker
on each server and adds the user `rhodry` to the `docker` group, allowing
rhodry to execute docker commands without `sudo`.

The target of these playbooks is the intenvtory defined in [hosts](/playbooks/hosts).
The inventory is split into two groups:

1. `[manager]` group which contains the `rhodry-manager`.
2. `[worker]` group which contains the rhodry worker nodes.

Due to the fact that the manager node's storage is set up differently, the
[setup-storage.yml](/playbooks/setup-storage.yml) playbook is executed on
the worker nodes only, unlike [install-docker.yml](/playbooks/install-docker.yml)
which is applied to manager and worker alike.

### Creating users on Rhodry workers

While ansbile itself does not require installation
of anything on `rhodry-xx` servers, it relies on seamless
ssh connections to other servers through ssh-keys.

Our control node for managing the worker clusters will be
`rhodry-master`. We need to generate an SSH-key pair (first-time only)
on `rhodry-master`, and copy its public key 

Before we move on to adding `rhodry-master` to the known hosts
of a `rhodry-xx` server and copying the public ssh key onto it,
we'll be creating the `rhodry` user on the worker.

While it is possible to generate the `rhodry` linux users
on `rhodry-xx` servers using ansible, that's not what we'll
be doing here. We'll want to make the SSH-keys known to the
`rhodry` user on the destination server instead of the default
`infra` user. So, the first thing we'll be doing on a new worker,
will be setting up the user.

```bash
$ sudo useradd -m -s /bin/bash rhodry
$ sudo usermod -aG sudo rhodry
$ sudo passwd rhodry

# a prompt will appear asking for the new user's password.
# for the sake of simplicity, we'll be using the same password
# on all rhodry workers (and the master).
```

While we're at it, let us also rename the hostname of the worker.

```bash
sudo hostnamectl set-hostname rhodry-${xx}

# wherein xx is the number of the node, e.g. 01, 02, etc.
# so the resulting hostname will be rhodry-01, rhodry-02, etc.
```

And we're all done with the worker. From here on, ansible will
hanlde the rest (setting up of the storage and the installation
of docker) from `rhodry-master`.

### Generating SSH-keys on `rhodry-master`

While on `rhodry-manager` node, we'll generate an ssh-key pair.

This is only needed on the first time setup; when we're adding new workers,
we'll be using the same ssh-key that we've generated the first time.


```bash
# on rhodry-master
$ ssh-keygen -t rsa

# several prompts appear asking for the location in which to store the ssh-key
# and also the password for ssh-key. Leave all at default (press enter all
# way through). This will create a password-less ssh-key pair on the default
# location, i.e. ~/.ssh/id_rsa and ~/.ssh/id_rsa.pub
```

Now we need to copy the public key onto the rhodry workers so that they'll
allow ssh connections from `rhodry-master` using the key that we've generated.

```bash
# on rhodry-master
$ ssh-copy-id -i ~/.ssh/id_rsa.pub ${rhodry-xx}
```

Wherein `${rhodry-xx}` must be replaced by the ip or dns-registered hostname
of the rhodry worker.

### Installing Ansible on `rhodry-master`

We only need to install ansible on the `rhodry-master`.

```bash
# rhodry-master
$ sudo apt install ansible
```

### Testing Ansbile

To make sure Ansible is setup correctly and can access all the nodes properly,
you can execute a ping command against the inventory:

```bash
$ ansible all -i hosts -m ping
172.17.23.35 | SUCCESS => {
    "ansible_facts": {
        "discovered_interpreter_python": "/usr/bin/python3"
    },
    "changed": false,
    "ping": "pong"
}
172.17.23.36 | SUCCESS => {
    "ansible_facts": {
        "discovered_interpreter_python": "/usr/bin/python3"
    },
    "changed": false,
    "ping": "pong"
}
172.17.23.37 | SUCCESS => {
    "ansible_facts": {
        "discovered_interpreter_python": "/usr/bin/python3"
    },
    "changed": false,
    "ping": "pong"
}
# ...
```

## Running the Playbooks

Assuming that:

1. `rhodry-manager` is set up,
2. `rhodry` user is created on the new workers,
3. Public ssh-key of `rhodry-manager` is copied onto new rhodry workers,
4. IP addresses of the new workers are added to the [inventry file](/playbooks/hosts)

we can execute the playbooks to set-up the workers.

```bash
$ ansible-playbook -i hosts setup-storage.yml -K
$ ansible-playbook -i hosts install-docker.yml -K

# after executing each of these commands, a prompt
# will appear asking for the sudo password of the
# user rhodry on the destination servers.
```

And we're done!

### A note on Ansible

This tutorial assumes that all the workers are listed under the `[workers]` group in the
inventory. This means that, if a new worker is added and we run the playbook as described
here, it'll execute the playbook for already set up workers all over again.

This is OK!

By nature, ansible is idempotent (at least in most scenarios). That means that it'll try
to bring the server to the state that is described in the playbook. If the server is already
in that state, it will not affect anything.

That said, if you want to be doubly sure that you don't break the already set up servers (e.g.
reformat the storage), you can edit the [inventory](/playbooks/hosts) and add the new servers
under a new group, say `[new-workers]`, and replace the hosts in the playbooks with the name of this
new group, i.e. `new-workers`. This way, the playbook will only be tried on the new servers.

## Playbook: `add-new-storage.yml`

This playbook is for adding new storage for data to already set-up servers.

Currently, the new storage is mounted at `/dev/sdc`. Add the newly added storage block to the
list of `pvs` in the first and second play.
