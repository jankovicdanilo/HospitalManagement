# Server configuration with Ansible

This playbook prepares a fresh Ubuntu 24.04 server for HMS. It turns the manual steps from the main
README ("One-time server setup") into code, so a server can be rebuilt with one command.

**What it does**

| Role             | Tag      | What it sets up                                                                                    |
| ---------------- | -------- | -------------------------------------------------------------------------------------------------- |
| `base`           | `base`   | Packages, the admin user's SSH keys, SSH password and root logins off, firewall (22, 80, 443)      |
| `k3s`            | `k3s`    | K3s, kubeconfig for the admin user, the Traefik HTTP to HTTPS redirect, Helm                       |
| `cluster_addons` | `addons` | cert-manager and the two Let's Encrypt issuers (staging and production)                            |
| `hms_prereqs`    | `hms`    | The `hms-dev` namespace, the `hms-secrets` secret, the deploy script and the restricted CI SSH key |

**What it does not do.** It does not release the application: after the playbook, run the
`Deploy to VM` workflow in GitHub, which installs the images. It also does not create the DNS name,
the Cloudflare frontend, the GitHub secrets, or snapshots.

Every task is safe to run again. A second run should report `changed=0`.

## One-time setup on the machine that runs Ansible (Windows)

Ansible's control side needs Linux, so use WSL.

1. In an administrator PowerShell: `wsl --install -d Ubuntu`, then restart and create the Linux user.
2. In the Ubuntu terminal:

```bash
sudo apt update && sudo apt install -y ansible git
ssh-keygen -t ed25519 -f ~/.ssh/id_ed25519 -C "ansible-control-machine"
cat ~/.ssh/id_ed25519.pub
```

Keep the SSH key in the WSL home folder, not under `/mnt/c`: Windows drives look world-writable to
Linux, and `ssh` refuses a private key it considers unprotected.

3. Add that public key to the server, from a session that can already log in:

```bash
echo 'ssh-ed25519 AAAA... ansible-control-machine' | sudo tee -a /home/<admin-user>/.ssh/authorized_keys
```

4. Work from this folder. The repository is under `/mnt/c/...` in WSL. Ansible ignores an
   `ansible.cfg` in a world-writable folder, so name it explicitly:

```bash
cd /mnt/c/Users/<you>/.../HospitalManagement/ansible
export ANSIBLE_CONFIG=$PWD/ansible.cfg
```

## Configure

```bash
cp inventory/hosts.example.ini inventory/hosts.ini           # the server address and admin user
cp group_vars/hms/vault.example.yml group_vars/hms/vault.yml # personal values and secrets
ansible-vault encrypt group_vars/hms/vault.yml
```

Both copies are git-ignored. For `vault_db_password`, `vault_jwt_key` and `vault_admin_password`, use the
values that are already in your password manager. They are only used when the `hms-secrets` secret
does not exist yet. An existing secret is never overwritten.

## Run

```bash
ansible hms -m ping --ask-vault-pass
```

`pong` means the connection works. Then go through the roles one at a time. **Always run check mode
first.** It shows what would change and changes nothing:

```bash
ansible-playbook site.yml --tags base --check --diff -K --ask-vault-pass
ansible-playbook site.yml --tags base -K --ask-vault-pass
```

`-K` asks for the sudo password of the admin user. Repeat with `--tags k3s`, `--tags addons` and
`--tags hms`, or run everything without `--tags`. Check mode skips the commands it cannot simulate,
so on a brand-new machine it reports less than a real run does.

Keep a second SSH window open while you run `base`: it changes the SSH settings and the firewall.

## Before the first run on a new machine

Create the admin user and install your public key by hand. That is the one step Ansible cannot do for
itself, because it needs a way in:

```bash
adduser <admin-user>
usermod -aG sudo <admin-user>
mkdir -p /home/<admin-user>/.ssh
echo 'ssh-ed25519 AAAA... ansible-control-machine' >> /home/<admin-user>/.ssh/authorized_keys
chown -R <admin-user>:<admin-user> /home/<admin-user>/.ssh && chmod 700 /home/<admin-user>/.ssh
chmod 600 /home/<admin-user>/.ssh/authorized_keys
```

## After rebuilding a server

- The new server has a new SSH host key, so update the `DEPLOY_KNOWN_HOSTS` secret in GitHub.
- Point the DNS name at the new address, and update `DEPLOY_HOST` if the address changed.
- Then run `Deploy to VM` with the latest commit on `main`.
