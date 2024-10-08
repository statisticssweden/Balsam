# Installation instructions for installing a demo Balsam hub

## ⚠️ Only for demo purposes ⚠️

## Prerequisites

* Knowledge of Helm and Kubernetes
* A Kubernetes cluster with:
    * Storage driver that can provision PV and a default storage class defined
    * DNS provider that can provide external IP adresses for services in the cluster
* A wild card DNS entry in your local DNS för all services in the cluster. Or a DNS zone in your local DNS and external DNS configured so that it can update DNS entries there.
* A least a cluster of X vCPU:s and Y GB of memory
* Helm 3

## General instructions

Replace `<YOUR-DOMAIN>` with your own DNS-entry

## Install dependencies

### Install and configure ArgoCD

1. Install ArgoCD following the instruction at [https://argo-cd.readthedocs.io/en/stable/getting_started/](https://argo-cd.readthedocs.io/en/stable/getting_started/)
1. Add an ingress to ArgoCD with the following definition

    ```yaml
    apiVersion: networking.k8s.io/v1
    kind: Ingress
    metadata:
      name: argocd-ingress
      namespace: argocd
    spec:
      rules:
      - host: argo-cd.<YOUR-DOMAIN>
        http:
          paths:
          - backend:
              serviceName: argocd-server
              servicePort: http
    ```

1. Sign in to ArgoCD and verify that it is running.

### Install and configure KeyCloak

Prerequisites: To be able to get a demo functionality in keycloak you will have to create a realm named Balsam. We have prepared this for you in the `realm.json` file. For the helm deployment to work you will
have to create a `ConfigMap` to import the realm to the keycloak installation. You do this with the following command:

```bash
kubectl create ns keycloak
kubectl create cm keycloak-realm --namespace=keycloak --from-file=realm-export.json
```

1. Install KeyCloak with Helm from `Bitnami` see [https://bitnami.com/stack/keycloak/helm](https://bitnami.com/stack/keycloak/helm) and use the following `values.yaml` file:

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
          - key: "realm-export.json"
            path: "realm-export.json"
    extraEnvVars:
      - name: MY_CLIENT_SECRET
        value: ""##add Clientsecret
    ```

### Install and configure GitLab

1. Configure Keycloak by following these [instructions](https://medium.com/@panda1100/gitlab-sso-using-keycloak-as-saml-2-0-idp-86b75abadaab) in the Keycloak realm of `Balsam`

1. Add the GitLab repo to Helm with

    ```bash
    helm repo add gitlab https://charts.gitlab.io/
    ```

1. Create a secret to connect GitLab to Keycloak via SAML:

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
    kubectl create ns gitlab
    kubectl create secret generic gitlab-saml -n gitlab --from-file=provider=provider.yaml
    ```

1. Create a `values.yaml` file for GitLab as follows:

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
      appConfig:
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

1. Install GitLab with Helm

    ```bash
    helm install gitlab gitlab/gitlab -f GitLab/values.yaml --namespace=gitlab
    ```

### Install and configure MinIO

1. Add `Bitnami` Helm repo

    ```bash
    helm repo add bitnami https://charts.bitnami.com/bitnami
    ```

1. Change the `values.yaml` for MinIO to match your environment
1. Install the helmchart for MinIO:

    ```bash
    helm install minio bitnami/minio -f MinIO/values.yaml --namespace=minio
    ```

### Install and configure RocketChat

1. helm repo add [rocketchat](https://rocketchat.github.io/helm-charts)
1. Under dependencies, change the `values.yaml` to match your environment
1. Install the helmchart for MinIO:

    ```bash
    helm install rocketchat rocketchat/rocketchat -f RocketChat/values.yaml --namespace=rocketchat
    ```

1. For OAuth to work, you may need to disable the option of two factor authentication if you have not setup an SMTP server to send out a verification code
    * You will need to log into RocketChat as an administrator and disable two factor authentication. You can do this by going to the Admin Page > Account > Two Factor Authentication and then disabling it

## Configure and install a Balsam hub

### Configure GitLab

1. Sign in with the KeyCloak user for GitLab and sign out (So that the user is created in GitLab)
1. Sign in with the root account in GitLab. Password is in secret `gitlab-gitlab-initial-root-password`
1. Make KeyCloak user an admin
1. Sign in with KeyCloak user and create a personal access token
1. Create a new Group
1. Remove the branch protection rules in the new group

### Prepare hub repository

1. Create a private Git repository (will contain sensitive information so keep it private)
1. Copy the files from `dependencies/HubRepoTemplates` and changes the placeholder in the templates in the format `<PLACEHOLDER>` and commit it to the repository
1. Create a user that has access to read and write to the repository

### Configure KeyCloak

1. Sign in to the admin console and change the URL:s in GitLab and rocketchat client
1. Add a user that should be admin in MiniIO. Add that user to the group `consoleAdmin`
1. Add a user that should be admin in GitLab
1. Add a user in KeyCloak that should be admin form `Balsam` realm

### Configure MinIO

1. Sign in with the user from keycloak in MinIO console
1. Create an access key with full rights

### Configure RocketChat

1. Sign in as admin
1. Create PAT

### Configure ArgoCD

1. Add a new application. Point it to the hub repository.
1. Enable auto sync

## Install Balsam

Use helm to install Balsam:

```bash
helm install balsam oci://registry-1.docker.io/statisticssweden/balsam-chart --version 0.1.2 -f YOUR-VALUES-FILE.yaml
```

### Values template for Balsam

Use the following values template and replace the placeholder with your settings:

```yaml
balsamApi:
  secret:
    name: balsam-api-secret
    data:
      user: <HUB-REPOSITORY-USER>
      password: <HUB-REPOSITORY-PASSWORD-OR-TOKEN>
  configMap:
    name: balsam-api-config
    data:
      repoUrl: <HUB-REPOSITORY-URL>
      authority: http://<KEYCLOAK-URL>/realms/Balsam
  ingress:
    hosts:
      host: <BALSAM-API-URL>

balsamUi:
  ingress:
    hosts:
      host: <BALSAM-UI-URL>

s3Provider:
  secret:
    name: minio-s3-provider-secret
    data:
      API__ACCESSKEY: <MINIO-ACCESSKEY-FOR-ADMIN>
      API__SECRETKEY: <MINIO-SECRETKEY-FOR-ADMIN>
  configMap:
    name: minio-s3-provider-config
    data:
      API__DOMAIN: <MINIO-URL> ##balsam-minio-pilot-api.tanzu.scb.intra
      API__PROTOCOL: http

gitProvider:
  secret:
    name: gitlab-provider-secret
    data:
      API__PAT: <GITLAB-PAT>
  configMap:
    name: gitlab-provider-config
    data:
      API__GroupID: <GITLAB-GROUP-ID>
      API__BaseUrl: <GITLAB-URL>
      API__TemplatePath: /app/templates

oidcProvider:
  secret:
    name: keycloak-provider-secret
    data:
      KEYCLOAK__ClientSecret: "MySecretSas"
      KEYCLOAK__User: "<KEYKLOAK-ADMIN-USER>"
      KEYCLOAK__Password: "<KEYCLOAK-ADMIN-PASSWORD>"
  configMap:
    name: keycloak-provider-config
    data:
      KEYCLOAK__BaseUrl: "<KEYCLOAK-URL>"
      KEYCLOAK__Realm: "Balsam"
      KEYCLOAK__ClientId: "demo"

chatProvider:
  secret:
    name: rocketchat-provider-secret
    data:
      API__Token: "<ROCKETCHAT-TOKEN>"
  configMap:
    name: rocketchat-provider-config
    data:
      API__BaseUrl: "<ROCKETCHAT-URL>"
      API__UserId: "<ROCKETCHAT-USER>"

roleBinding:
  enabled: true
```
