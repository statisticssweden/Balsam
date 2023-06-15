
# Balsam API

Use the following command to generate the souces from the spec.
```
docker run --rm -v ${PWD}:/localin -v ${PWD}\..\Balsam:/localout openapitools/openapi-generator-cli generate -i /localin/Balsam.yaml -g aspnetcore -o /localout/ -c /localin/Balsam-config.yaml
```

# Specs for providers

## GitProvider
Provides git repositories to Balsam hubs.

Use the following command to generate the souces from the spec.
```
docker run --rm -v ${PWD}:/localin -v ${PWD}\..\GitLabGitProvider:/localout openapitools/openapi-generator-cli generate -i /localin/GitProvider.yaml -g aspnetcore -o /localout/ -c /localin/GitProvider-config.yaml
```

## OicdProvider
Provides Balsam hubs to create roles for configured OICD Provider.

Use the following command to generate the souces from the spec.
```
docker run --rm -v ${PWD}:/localin -v ${PWD}\..\KeycloakOicdProvider:/localout openapitools/openapi-generator-cli generate -i /localin/OicdProvider.yaml -g aspnetcore -o /localout/ -c /localin/OicdProvider-config.yaml
```

## S3Provider
Provides Balsam hubs with s3 storage

Use the following command to generate the souces from the spec.
```
docker run --rm -v ${PWD}:/localin -v ${PWD}\..\MinIOS3Provider:/localout openapitools/openapi-generator-cli generate -i /localin/S3Provider.yaml -g aspnetcore -o /localout/ -c /localin/S3Provider-config.yaml
```

