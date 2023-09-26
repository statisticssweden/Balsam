# Installation instructions for installing a demo Balsam hub

Only for demo purposes...

Klowledge of Helm Kubernetes etc.

## Prerequisites

* Knowledge of Helm Kubernetes etc.
* A Kubernetes cluster with
  * storage driver that can provision PV and a default storage class defined.
  * a could provider that can provide external IP adresses for services in the cluster.
* A wild card DNS entry in your local DNS för all services in the cluster. Or a DNS zone in your local DNS and external DNS configured so that it can uppdate DNS entries there.
* A least a cluster of X vCPU:s and Y GB of memory.
* Helm 3

## General instructions

Replace `<YOUR-DOMAIN>` with your own DNS-entry

## Install dependencies

xxx

### Install and configure ArgoCD

1. Install ArgoCD following the instruction at (https://argo-cd.readthedocs.io/en/stable/getting_started/)[https://argo-cd.readthedocs.io/en/stable/getting_started/]
2. Add an ingress to ArgoCD with the following definition

```yaml
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: argocd-ingress
spec:
  rules:****
  - host: argo-cd.<YOUR-DOMAIN>
    http:
      paths:
      - backend:
          serviceName: argocd-server
          servicePort: http
```

3. Sign in to ArgoCD and verify that it is running.

### Install and configure KeyCloak
Prerequisites: To be able to get a demo functionality in keycloak you will have to create a realm named Balsam. We have prepared this for you in the realm.json file. For the helm deployment to work you will
have to create a ConfigMap to import the realm to the keycloak installation. You do this with the following command: 
```bash
kubectl create cm keycloak-realm --namespace=keycloak --from-file=realm.json
```
1. Install KeyCloak with Helm from Bitnami see (https://bitnami.com/stack/keycloak/helm)[https://bitnami.com/stack/keycloak/helm] and use the following values.yaml file:

  ```yaml
auth:
  adminUser: ##add admin user
  adminPassword: ##add admin password

ingress:
  enabled: false ##enable by setting this to true
  hostname: ##add hostname

postgresql:
  enabled: true
  auth:
    username: bn_keycloak
    password: ##add password
    database: bitnami_keycloak
    existingSecret: ""
  architecture: standalone


extraStartupArgs: "--import-realm"

extraVolumeMounts:
  - name: config
    mountPath: "/opt/bitnami/keycloak/data/import"
    readOnly: true
extraVolumes:
  - name: config
    configMap:
      name: keycloak-realm
      items:
      - key: "realm.json"
        path: "realm.json"
extraEnvVars:
  - name: MY_CLIENT_SECRET
    value: ""##add Clientsecret

  ```

2. Sign in and verify that it is running.
3. Add new Realm `Balsam`
4. Create new Client `demo`
5. 

### Install and configure GitLab

1. Configure Keycloak by following these [instructions](https://medium.com/@panda1100/gitlab-sso-using-keycloak-as-saml-2-0-idp-86b75abadaab) in the Keycloak realm of `Balsam`
   
2. Add the GitLab repo to Helm with 

  ```bash
  helm repo add gitlab https://charts.gitlab.io/
  ```

3. Create a secret to connect Gitlab to Keycloak via SAML 
```yaml
name: saml
label: 'Keycloak Login'
args:
  assertion_consumer_service_url: 'http://gitlab.<YOUR-DOMAIN>/users/auth/saml/callback'
  idp_cert_fingerprint: '' ## Get the fingerprint using the instructions: https://medium.com/@panda1100/gitlab-sso-using-keycloak-as-saml-2-0-idp-86b75abadaab
  idp_sso_target_url: 'http://keycloak.<YOUR-DOMAIN>/realms/Balsam/protocol/saml/clients/gitlab.<YOUR-DOMAIN>'
  issuer: 'gitlab'
  name_identifier_format: 'urn:oasis:names:tc:SAML:2.0:nameid-format:persistent'
```
```bash
kubectl create secret generic gitlab-saml -n gitlab --from-file=provider=provider.yaml
```

4. Create a values.yaml file for GitLab as follows

```yaml
global:
  hosts:
    domain: <YOUR-DOMAIN>
    hostSuffix:
    https: false
    externalIP:
    ssh: ~
    gitlab:
      name: gitlab.<YOUR-DOMAIN>
      https: false

  ## https://docs.gitlab.com/charts/charts/globals#configure-ingress-settings
  ingress:
    apiVersion: ""
    configureCertmanager: false


    ## https://docs.gitlab.com/charts/charts/globals#omniauth
    omniauth:
      enabled: true
      autoSignInWithProvider:
      syncProfileFromProvider: []
      syncProfileAttributes: ['email','first_name','last_name', 'roles']
      allowSingleSignOn: [saml]
      blockAutoCreatedUsers: false
      autoLinkLdapUser: false
      autoLinkSamlUser: true
      autoLinkUser: []
      externalProviders: []
      allowBypassTwoFactor: []
      providers:
        - secret: gitlab-saml
          key: provider

  ## https://docs.gitlab.com/charts/charts/gitlab/kas/
  kas:
    enabled: false

certmanager:
  install: false

gitlab-runner:
  install: true
```

4. Install GitLab with Helm

```bash
helm install gitlab gitlab/gitlab -f GitLab/values.yaml --namespace=gitlab
```

### Install and configure MinIO

1. Add Bitnami Helm repo
  
```bash
helm repo add bitnami https://charts.bitnami.com/bitnami
```
2. Change the values.yaml for MinIO to match your environment

3. Install the helmchart for minio
```bash
helm install minio bitnami/minio -f MinIO/values.yaml --namespace=minio
```

### Install and configure RocketChat
1. helm repo add rocketchat https://rocketchat.github.io/helm-charts
2. Under dependencies, change the values.yaml to match your environment
3. Install the helmchart for minio
```bash
helm install rocketchat rocketchat/rocketchat -f RocketChat/values.yaml --namespace=rocketchat
```
4. For Oauth to work, you may need to disable the option of two factor authentication if you have not setup an smtp server to send out a verification code.
  - You will need to log into RocketChat as an administrator and disable two factor authentication. You can do this by going to the Admin Page > Account > Two Factor Authentication and then disabling it.


## Configure and install a Balsam hub
### Prepare hub repository
### Configure ArgoCD
### Configure KeyCloak
1. Create realm
2. create role
3. Create user
4. Create client
etc