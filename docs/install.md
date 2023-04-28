# Installation instructions for installing a demo Balsam hub
Only for demo purposes...

Klowleadg of Helm Kubernetes etc.

## Prerequisites
* Knowledge of Helm Kubernetes etc.
* A Kubernetes cluster with
    * storage driver that can provision PV and a default storage class defined.
    * a could provider that can provide external IP adresses for services in the cluster.
* A wild card DNS entry in your local DNS för all services in the cluster. Or a DNS zone in your local DNS and external DNS configured so that it can uppdate DNS entries there.
* A least a cluster of X vCPU:s and Y GB of memory.
* Helm 3

## Generla instructions
Replace `<YOUR_DNS_WILDCARD_ENTRY>` with your own DNS-entry

## Install dependencies
xxx
### Install and configure ArgoCD
1. Install ArgoCD following the instrction at (https://argo-cd.readthedocs.io/en/stable/getting_started/)[https://argo-cd.readthedocs.io/en/stable/getting_started/]
2. Add an ingress to ArgoCD with the following definition
```yaml
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: argocd-ingress
spec:
  rules:
  - host: argo-cd.<YOUR_DNS_WILDCARD_ENTRY>
    http:
      paths:
      - backend:
          serviceName: argocd-server
          servicePort: http
```
3. Sign in to ArgoCD and verify that it is running.

### Install and configure KeyCloak
1. Install KeyCloak with Helm from Bitnami see (https://bitnami.com/stack/keycloak/helm)[https://bitnami.com/stack/keycloak/helm] and use the following values.yaml file:
```yaml
auth:
  adminUser: user
  adminPassword: <YOUR-PASSWORD>

ingress:
  hostname: balsam-keycloak.<YOUR_DNS_WILDCARD_ENTRY>

postgresql:
  enabled: true
  auth:
    username: bn_keycloak
    password: <YOUR-DB-PASSWORD>
    database: bitnami_keycloak
    existingSecret: ""
  architecture: standalone

```
2. Sign in and verify that it is running.

### Install and configure GitLab
1. Add the GitLab repo to Helm with 
```
helm repo add gitlab https://charts.gitlab.io/
```
2. Create a values.yaml file for GitLab as follows
```yaml

```
3. Install GitLab with Helm 
```
helm repo add gitlab https://charts.gitlab.io/
```
### Install and configure MinIO

## Configure and install a Balsam hub
### Prepare hub repository
### Configure ArgoCD
### Configure KeyCloak
1. Create realm
2. create role
3. Create user
4. Create client
etc