#!/usr/bin/env bash
# Installed on the VPS as /usr/local/bin/hms-deploy (root-owned, mode 755):
#   sudo install -o root -g root -m 755 hms-deploy.sh /usr/local/bin/hms-deploy
# Called by the CI deploy key as an SSH forced command. Usage over SSH: deploy <40-character commit sha>
# Downloads that commit from GitHub, fills in the image tag, runs helm upgrade --atomic.
set -euo pipefail

export PATH="/usr/local/bin:/usr/bin:/bin"
export KUBECONFIG="${HOME}/.kube/config"

REPO="jankovicdanilo/HospitalManagement"
NAMESPACE="hms-dev"
RELEASE="hms"

log() { echo "[$(date -u +%H:%M:%S)] $*"; }

read -r ACTION SHA EXTRA <<< "${SSH_ORIGINAL_COMMAND:-}"
if [[ "${ACTION:-}" != "deploy" || -n "${EXTRA:-}" || ! "${SHA:-}" =~ ^[0-9a-f]{40}$ ]]; then
  echo "usage: deploy <40-character commit sha>" >&2
  exit 2
fi
TAG="${SHA:0:7}"

exec 9>"${HOME}/.hms-deploy.lock"
if ! flock -n 9; then
  echo "another deploy is already running" >&2
  exit 3
fi

WORKDIR="$(mktemp -d)"
trap 'rm -rf "$WORKDIR"' EXIT

log "deploying commit $SHA (image tag $TAG)"
log "downloading the chart from GitHub"
curl -fsSL "https://codeload.github.com/${REPO}/tar.gz/${SHA}" | tar -xz -C "$WORKDIR" --strip-components=1

if [[ ! -f "$WORKDIR/hms-chart/Chart.yaml" ]]; then
  echo "chart not found in that commit" >&2
  exit 4
fi

sed "s/COMMIT/${TAG}/g" "$WORKDIR/hms-chart/values-vm.yaml" > "$WORKDIR/values.yaml"
if grep -q COMMIT "$WORKDIR/values.yaml"; then
  echo "image tag placeholder left in the values file" >&2
  exit 4
fi

log "helm upgrade (atomic: rolls back by itself on failure)"
helm upgrade "$RELEASE" "$WORKDIR/hms-chart" -n "$NAMESPACE" -f "$WORKDIR/values.yaml" \
  --atomic --timeout 10m --description "images $TAG (CI deploy)"

log "release history and images"
helm history "$RELEASE" -n "$NAMESPACE" | tail -3
kubectl get deploy -n "$NAMESPACE" -o custom-columns=NAME:.metadata.name,IMAGE:.spec.template.spec.containers[0].image
log "done"
