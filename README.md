# Hospital Management System

[![Backend CI](https://github.com/jankovicdanilo/HospitalManagement/actions/workflows/ci.yml/badge.svg)](https://github.com/jankovicdanilo/HospitalManagement/actions/workflows/ci.yml)

A backend REST API for managing hospital operations, built with .NET 8 and structured
as a set of microservices handling authentication, core domain data, and appointment
scheduling independently.

The React frontend lives in a separate repository,
[hospital-management-frontend](https://github.com/jankovicdanilo/hospital-management-frontend).

## Tech stack

- **Runtime**: .NET 8, ASP.NET Core Web API
- **ORM**: Entity Framework Core 8 with SQL Server
- **Auth**: JWT Bearer via `Microsoft.AspNetCore.Authentication.JwtBearer`
- **Mapping**: AutoMapper
- **Validation**: FluentValidation
- **Logging**: NLog
- **Caching**: Redis (statistics cache, token revocation list)
- **Resilience**: Polly (retry, circuit breaker, timeout)
- **AI integration**: Model Context Protocol (MCP) server
- **Containerisation**: Docker, Docker Compose
- **Orchestration**: Kubernetes (Docker Desktop locally, K3s on the VPS), Helm
- **Ingress and TLS**: Traefik, cert-manager with Let's Encrypt
- **CI/CD**: GitHub Actions, GitHub Container Registry (GHCR)
- **Testing**: NUnit, Moq
- **Documentation**: Swagger / Swashbuckle

## Architecture

This system is built as a set of microservices, each owning its own database and
communicating over HTTP.

```mermaid
architecture-beta
    group system(cloud)[Hospital Management System]

    %% Databases
    service cqrsdb(database)[CQRS DB] in system
    service authdb(database)[AuthDB] in system
    service appdb(database)[AppointmentsDB] in system

    %% Services
    service command(server)[CommandService] in system
    service auth(server)[AuthService] in system
    service app(server)[AppointmentService] in system
    service query(server)[QueryService] in system
    service invoice(server)[InvoiceService] in system
    service stats(server)[StatisticsService] in system

    %% Invisible bend, so AppointmentsDB sits below AuthService but connects only to AppointmentService
    junction appjunction in system

    %% Clients and shared infrastructure (outside the system boundary)
    service claude(internet)[Claude Code]
    service mcp(server)[MCP Server]
    service redis(database)[Redis]

    %% Connections that cannot sit side by side (listed first, so the layout edges below take priority)
    auth:L -- R:command
    auth:R -- L:invoice
    app:L -- R:query

    %% Database ownership
    command:R -- L:cqrsdb
    query:L -- R:cqrsdb
    auth:L -- R:authdb
    app:B -- T:appjunction
    appjunction:L -- R:appdb

    %% Authentication (JWT) and business communication
    auth:T -- B:query
    auth:R -- L:app
    stats:B -- T:app
    query:R -- L:stats
    app:R -- L:invoice

    %% MCP: started by Claude Code, calls the services with a service token issued by Auth
    claude:R -- L:mcp
    mcp:R -- L:command{group}

    %% Redis: statistics cache and token revocation list
    redis:L -- R:invoice{group}
```

### Services

**Auth Service** (`HospitalManagement.Auth`) handles all authentication concerns:
user registration, login, role assignment, and JWT token issuance, for both users and
service accounts. It is the only service that generates tokens. All other services
validate incoming tokens using the same shared signing key but never generate them.

**Command Service** (`HospitalManagement.CommandService`) is the write side of the
core domain. It owns Doctor, DoctorSchedule, Patient, and Procedure data in the CQRS
database.

**Query Service** (`HospitalManagement.QueryService`) is the read side of the core
domain. It serves Doctor, DoctorSchedule, Patient, and Procedure data from the CQRS
database and is the source other services call to look these up.

**Appointment Service** (`HospitalManagement.Appointments`) owns everything
appointment-related: Appointment, AppointmentProcedure, Treatment, and the discount
calculator. It has its own database and calls the Query Service over HTTP to validate
and look up doctors, patients, and schedules. Procedure name and price are
snapshotted onto appointment records at creation time, so later catalog changes do not
alter past appointments. It also exposes `GET /api/appointment/stats-data`, a flat
list of appointments with discounted totals used by the Statistics Service.

**Invoice Service** (`HospitalManagement.InvoiceService`) generates PDF and DOCX
invoices (English or Montenegrin) for completed appointments. It has no database and
fetches appointment data from the Appointment Service.

**Statistics Service** (`HospitalManagement.Statistics`) provides read-only aggregates
for the statistics dashboard: doctor load and revenue, procedure revenue, and patient
statistics. It has no database; it aggregates data from the Appointment and Query
services and caches results in Redis for 5 minutes. Downstream calls use Polly
(retry, circuit breaker, timeout). All endpoints take `from` and `to` dates.

**MCP Server** (`HospitalManagement.Mcp`, .NET 10) lets an AI assistant such as Claude
Code work with hospital data through the Model Context Protocol. It runs locally and is
started by the AI client over stdio, so it is not a deployed service. Its tools cover
doctors, patients, doctor schedules, free slots, and appointments: searching, reading,
creating, and deleting. Delete tools require an explicit confirmation. It calls the
Command, Query, and Appointment services over HTTP with a service token (role `Mcp`)
kept in its user secrets, never a user password. An admin issues the token through
Auth; it expires and can be revoked at any time. The `Mcp` role cannot use statistics,
invoices, or user management.

### Roles and permissions

Every user has exactly one role. The backend enforces the rules; the frontend only
mirrors them.

| Role        | What they do                                                                                                                         |
| ----------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| **Admin**   | Full access, including user accounts and service tokens                                                                              |
| **Manager** | Runs the clinic's records: patients, doctors, schedules, and procedures. Sees statistics and invoices. Has no access to appointments |
| **Doctor**  | Works with their own appointments and patients, and can register new patients                                                        |
| **Mcp**     | A service role used only by the MCP server. It is never a login                                                                      |

| What                                                                                     | Admin | Manager | Doctor                |
| ---------------------------------------------------------------------------------------- | ----- | ------- | --------------------- |
| View doctors, schedules, patients, and procedures                                        | Yes   | Yes     | Yes                   |
| Add, edit, and delete patients                                                           | Yes   | Yes     | Yes                   |
| Add, edit, and delete doctors, schedules, and procedures                                 | Yes   | Yes     | No                    |
| Create, view, change, and delete appointments, including their procedures and treatments | Yes   | No      | Own appointments only |
| View a patient's history and AI summary                                                  | Yes   | No      | Own patients only     |
| View statistics and download invoices                                                    | Yes   | Yes     | No                    |
| Manage user accounts and service tokens                                                  | Yes   | No      | No                    |

**What "own" means for a doctor**

- An appointment is theirs when they are its doctor.
- A patient is theirs when they have at least one appointment with that patient, in any status.
- They can still find and register any patient, so they can book a first visit.
- Their appointment list only ever contains their own appointments, whatever filter is sent.
- They can create or edit an appointment only with themselves as the doctor, and look up free slots only for themselves.

**When a request is refused**

The service answers `403` with the same body as other errors, `{ "errorCode": "...", "message": "..." }`.
For example, `NOT_YOUR_APPOINTMENT` means a doctor touched another doctor's appointment.
A `401` still means the user is not logged in, or the token expired or was revoked.

**Creating users** (admin only) with `POST /api/auth/register`:

```json
{
  "username": "doctor6",
  "email": "doctor6@hospital.com",
  "password": "...",
  "role": "Doctor",
  "doctorId": 6
}
```

- `doctorId` is required for `Doctor` and not allowed for the other roles. It must be the id of an existing doctor; Auth does not check it.
- A role change applies after the next login, because the role is stored in the token.

**User account rules**

- Usernames and emails are unique, on register and on update.
- A doctor can have at most one account (`DOCTOR_ALREADY_LINKED`).
- An admin cannot delete their own account (`CANNOT_DELETE_SELF`) or change their own role (`CANNOT_CHANGE_OWN_ROLE`).
- There is no change-password endpoint. To replace an admin password, create a new admin and delete the old one.

**The first admin**

When the Auth database has no admin yet, Auth creates one at startup. The password comes from the
`Admin:Password` setting (the environment variable `Admin__Password`, which the chart reads from the
`ADMIN_PASSWORD` key of the Kubernetes secret). Outside the Development environment, startup fails
if it is missing, so no default password can reach a server. In Development it falls back to a
default password and logs a warning.

**Redis requirement**

Every service that accepts service tokens needs Redis (`Redis:ConnectionString`). Tokens
with a `jti` are checked against the revocation list, and a service that cannot reach
Redis rejects them. Login tokens have no `jti` and are not affected.

### Shared library

`HospitalManagement.Shared` is a class library referenced by all services. It contains
shared primitives (`Result<T>`, `PagedResult<T>`, `BaseController`), the role names and
authorization policies, the token-forwarding HTTP handler, and shared DTOs and enums
such as `AppointmentStatus`. It has no runtime dependency on any service and is not
deployed independently.

## Deployment

The system runs on Kubernetes and is installed with one Helm chart, `hms-chart/`. For each of the
six services it creates a Deployment, a Service and an autoscaler, plus one Ingress that routes
by path (`/auth`, `/query`, `/command`, `/appointments`, `/invoice`, `/statistics`). The chart
also runs SQL Server (on a persistent volume), Redis and Seq.

| Environment | Cluster                   | Ingress                                  | Values                           |
| ----------- | ------------------------- | ---------------------------------------- | -------------------------------- |
| Local       | Docker Desktop Kubernetes | `nginx` by default (`ingress.className`) | `values.yaml`                    |
| VPS         | K3s, one node             | Traefik, HTTPS from Let's Encrypt        | `values.yaml` + `values-vm.yaml` |

`values.yaml` holds the defaults: ports, the ingress path of each service, environment variables
and the allowed CORS origins. `values-vm.yaml` overrides them for the VPS: images from GHCR (with a
`COMMIT` placeholder for the tag), TLS, the Traefik ingress class, `Production` as the environment,
and the frontend's CORS origin. **Neither file contains secrets.**

### The secret

Both environments need a Kubernetes secret named `hms-secrets` in the `hms-dev` namespace, with five keys:

| Key              | Value                                                                           |
| ---------------- | ------------------------------------------------------------------------------- |
| `DB_PASSWORD`    | The SQL Server `sa` password. It must meet SQL Server's complexity rules        |
| `JWT_KEY`        | The signing key shared by all services, at least 32 characters                  |
| `JWT_ISSUER`     | The token issuer, the same for every service (for example `HospitalManagement`) |
| `JWT_AUDIENCE`   | The token audience, the same for every service                                  |
| `ADMIN_PASSWORD` | The password of the first admin account                                         |

Generate each value in a password manager and keep it out of git.

### Deploy locally

1. Build the images (once per change): `docker build -t hospitalmanagement-<service>:<tag> -f HospitalManagement.<Project>/Dockerfile .`
   and set the matching `image:` values for the services.
2. Create the namespace and the secret, with the five values in environment variables:

```powershell
kubectl create namespace hms-dev
kubectl create secret generic hms-secrets -n hms-dev `
  --from-literal=DB_PASSWORD="$env:DB_PASSWORD" `
  --from-literal=JWT_KEY="$env:JWT_KEY" `
  --from-literal=JWT_ISSUER="$env:JWT_ISSUER" `
  --from-literal=JWT_AUDIENCE="$env:JWT_AUDIENCE" `
  --from-literal=ADMIN_PASSWORD="$env:ADMIN_PASSWORD"
```

3. Install: `helm upgrade --install hms .\hms-chart -n hms-dev`
4. Check: `kubectl get pods -n hms-dev`

The cluster needs an ingress controller. The chart supports ingress-nginx and Traefik
(`--set ingress.className=traefik`). ingress-nginx was retired in March 2026 and gets no
security fixes, so use Traefik for anything long-lived.

### The VPS

```
browser --HTTPS--> Cloudflare (frontend, static files)
browser --HTTPS--> VPS: Traefik --> six services --> SQL Server, Redis, Seq
```

The VPS is one Ubuntu 24.04 machine running K3s. Traefik is K3s's own, with a redirect from HTTP to
HTTPS. cert-manager gets a certificate from Let's Encrypt for the API's DNS name (a free DuckDNS
name) and renews it by itself, using the HTTP-01 challenge on port 80. The release is called `hms`,
in the `hms-dev` namespace.

<details>
<summary>One-time server setup (about 2 hours)</summary>

**1. Base server.** Update the system, create a normal user with an SSH key, then turn off password
and root logins:

```bash
echo -e "PasswordAuthentication no\nPermitRootLogin no" | sudo tee /etc/ssh/sshd_config.d/00-hardening.conf
sudo sshd -t && sudo systemctl restart ssh
```

Check from a second window that the key login still works before closing the first one.

**2. Firewall.** Only SSH, HTTP and HTTPS are open from outside. The two `10.x` ranges let K3s's
pod and service networks talk to each other:

```bash
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow from 10.42.0.0/16 to any
sudo ufw allow from 10.43.0.0/16 to any
sudo ufw enable
```

Never open the Kubernetes API port (6443) to the internet. Use an SSH tunnel for `kubectl` from your own machine.

**3. K3s, with its packaged Traefik, and Helm.**

```bash
curl -sfL https://get.k3s.io | sh -
mkdir -p ~/.kube && sudo cp /etc/rancher/k3s/k3s.yaml ~/.kube/config
sudo chown $USER ~/.kube/config && chmod 600 ~/.kube/config
echo 'export KUBECONFIG=$HOME/.kube/config' >> ~/.bashrc
curl -fsSL https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash
```

**4. Traefik: redirect from HTTP to HTTPS.** Save this as
`/var/lib/rancher/k3s/server/manifests/traefik-config.yaml`; K3s applies it by itself:

```yaml
apiVersion: helm.cattle.io/v1
kind: HelmChartConfig
metadata:
  name: traefik
  namespace: kube-system
spec:
  valuesContent: |-
    additionalArguments:
      - "--entryPoints.web.http.redirections.entryPoint.to=:443"
      - "--entryPoints.web.http.redirections.entryPoint.scheme=https"
      - "--entryPoints.web.http.redirections.entryPoint.permanent=true"
```

The target must be `:443`. Traefik's secure entrypoint listens on port 8443 inside the pod, so
`to=websecure` would send visitors to a port nobody can reach, and Let's Encrypt would fail the
renewal check.

**5. cert-manager and the two issuers.** Staging has generous limits, so try it first:

```bash
helm repo add jetstack https://charts.jetstack.io
helm repo update
helm install cert-manager jetstack/cert-manager --namespace cert-manager --create-namespace --set crds.enabled=true
```

```yaml
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-staging
spec:
  acme:
    server: https://acme-staging-v02.api.letsencrypt.org/directory
    email: <your-email>
    privateKeySecretRef:
      name: letsencrypt-staging-account
    solvers:
      - http01:
          ingress:
            ingressClassName: traefik
---
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-prod
spec:
  acme:
    server: https://acme-v02.api.letsencrypt.org/directory
    email: <your-email>
    privateKeySecretRef:
      name: letsencrypt-prod-account
    solvers:
      - http01:
          ingress:
            ingressClassName: traefik
```

**6. The secret.** Type the values at the prompts, so they never land in the shell history:

```bash
kubectl create namespace hms-dev
read -rsp "DB password: " DB_PASSWORD; echo
read -rsp "JWT key: " JWT_KEY; echo
read -rsp "Admin password: " ADMIN_PASSWORD; echo
kubectl create secret generic hms-secrets -n hms-dev \
  --from-literal=DB_PASSWORD="$DB_PASSWORD" \
  --from-literal=JWT_KEY="$JWT_KEY" \
  --from-literal=JWT_ISSUER=HospitalManagement \
  --from-literal=JWT_AUDIENCE=HospitalManagement \
  --from-literal=ADMIN_PASSWORD="$ADMIN_PASSWORD"
unset DB_PASSWORD JWT_KEY ADMIN_PASSWORD
```

**7. The deploy script and the restricted CI key.** Install `deploy/vm/hms-deploy.sh` as root-owned:

```bash
sudo install -o root -g root -m 755 hms-deploy.sh /usr/local/bin/hms-deploy
```

Create a separate key pair for CI, and add the public half to the deploy user's `~/.ssh/authorized_keys`
as a line that allows only this script and nothing else:

```
command="/usr/local/bin/hms-deploy",restrict ssh-ed25519 AAAA... github-actions-deploy
```

The key cannot open a shell, forward ports or run any other command. The private half goes into the
`DEPLOY_SSH_KEY` secret (see CI/CD), and should not stay anywhere else.

</details>

## CI/CD

Two GitHub Actions workflows build, test and ship the backend:

| Workflow         | File                           | Runs when                                                                        |
| ---------------- | ------------------------------ | -------------------------------------------------------------------------------- |
| **Backend CI**   | `.github/workflows/ci.yml`     | On every pull request to `main`, and on every push to `main`                     |
| **Deploy to VM** | `.github/workflows/deploy.yml` | By hand with a commit id ("Run workflow"), or called by Backend CI after a merge |

```
pull request   ->  build-and-test
push to main   ->  build-and-test -> push-images (6 images) -> deploy -> smoke test
```

**What runs on what**

| Event                          | `build-and-test` | `push-images` | `deploy`                          |
| ------------------------------ | ---------------- | ------------- | --------------------------------- |
| Pull request                   | Yes              | Skipped       | Skipped                           |
| Merge to `main`                | Yes              | Yes           | Only when `AUTO_DEPLOY` is `true` |
| "Run workflow" on Deploy to VM | No               | No            | Yes                               |

A pull request never publishes an image or touches the server.

### Images

`push-images` builds the six Dockerfiles in parallel and pushes them to GHCR as
`ghcr.io/jankovicdanilo/hospitalmanagement/<service>:<tag>`, where the service is `auth-service`,
`appointments-service`, `command-service`, `invoice-service`, `query-service` or `statistics-service`.
Each image gets two tags: the short commit id (seven characters), and `latest`. Deployments always use
the commit tag. The packages are public, so the cluster pulls them without credentials.

### Deploy to VM

1. **`check`**: the commit id is 40 hex characters, exists, and is part of `main`. For an automatic
   run it must also still be the newest commit on `main`, otherwise the run is skipped, so an old
   run can never overwrite a newer release. All six images for that commit must exist.
2. **`deploy`** (environment `production`): connects over SSH with the restricted key, with the
   server's host key pinned, and runs `hms-deploy <commit id>` on the VPS. The script downloads the
   chart for that exact commit from GitHub, fills in the image tag, and runs
   `helm upgrade --atomic`. If the pods do not become ready in time, Helm rolls the release back by itself.
3. **Smoke test**: the `/health` endpoint of all six services must answer `200`.
4. The key is removed from the runner, even when a step fails.

Only one deploy runs at a time, and a running deploy is never cancelled.

### Set up once in GitHub

Create an environment named `production` (Settings, Environments), and limit its deployment branches
to `main`. Add these to it:

| Name                 | Kind                | Purpose                                                             |
| -------------------- | ------------------- | ------------------------------------------------------------------- |
| `DEPLOY_HOST`        | Environment secret  | The server's address                                                |
| `DEPLOY_USER`        | Environment secret  | The SSH user                                                        |
| `DEPLOY_SSH_KEY`     | Environment secret  | The private half of the restricted CI key                           |
| `DEPLOY_KNOWN_HOSTS` | Environment secret  | The server's host key, so the job cannot be sent to another machine |
| `AUTO_DEPLOY`        | Repository variable | `true` turns automatic deploys on. Anything else turns them off     |

To get the host key line, run this on the server: `echo "<server-ip> $(cut -d' ' -f1,2 /etc/ssh/ssh_host_ed25519_key.pub)"`.
Adding yourself as a required reviewer on `production` makes every automatic deploy wait for an approval.

### Releasing

**A normal release.** Open a pull request and wait for the green check. Merge it, and merge one
change at a time. The images are built, and the deploy follows by itself when `AUTO_DEPLOY` is `true`.
Check what runs:

```bash
helm history hms -n hms-dev | tail -3
kubectl get deploy -n hms-dev -o custom-columns=NAME:.metadata.name,IMAGE:.spec.template.spec.containers[0].image
```

Each deploy adds a revision described `images <tag> (CI deploy)`.

**Deploying or rolling back by hand.** Actions, then Deploy to VM, then Run workflow, and paste the
full 40-character id of a commit on `main`. The images for it must exist. To roll back, use the id of
the last good commit.

**A release with database migrations.** Right now Helm's rollback does not undo migrations, so:

1. Set `AUTO_DEPLOY` to `false`.
2. Take a snapshot of the VPS.
3. Merge the backend, wait for the images, and deploy by hand.
4. Check the app, then merge the frontend, then set `AUTO_DEPLOY` back to `true`.

The backend goes first because the frontend rebuilds on every merge, and a new frontend calling
endpoints that do not exist yet would break.

## Frontend

The frontend is a static site built from the `main` branch of its own repository and published on
Cloudflare (Workers static assets) with every push. It reads the API addresses from the build
variables `VITE_AUTH_SERVICE_URL`, `VITE_QUERY_SERVICE_SERVICE_URL`, `VITE_COMMAND_SERVICE_SERVICE_URL`,
`VITE_APPOINTMENT_SERVICE_URL`, `VITE_INVOICE_SERVICE_URL` and `VITE_STATISTICS_SERVICE_URL`. They are
baked into the files at build time, so changing one needs a rebuild. The site's address must be in
`cors.allowedOrigins` in `values-vm.yaml`, written exactly (`https://`, no trailing slash). The list
replaces the one in `values.yaml`, so keep the local development origins in it.

## Everyday commands

```bash
kubectl get pods -n hms-dev
kubectl logs deploy/auth -n hms-dev --tail=50
kubectl get certificate -n hms-dev
helm history hms -n hms-dev
```

## Known limitations

- **One VPS.** If the machine goes down, the whole system is down until it is back. It recovers by
  itself after a reboot, but there is no second node.
- **No automated database backups.** The only copy is the VPS disk and the provider's snapshots, which
  expire after 30 days.
- **Traffic between the ingress and the services is plain HTTP.** That is acceptable on one node, and
  needs encryption between nodes if a second one is added.
- **No network policies.** Any pod can reach any other pod, including SQL Server.
- **The server setup is manual.** The steps above are a runbook, not automation.
