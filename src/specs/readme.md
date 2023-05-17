# Specs for providers

## GitProvider
Provides git repositories to Balsam hubs.

Use the following command to generate the souces from the spec.
```
docker run --rm -v ${PWD}:/localin -v ${PWD}\..\GitLabGitProvider:/localout openapitools/openapi-generator-cli generate -i /localin/GitProvider.yaml -g aspnetcore -o /localout/ -c /localin/GitProvider-config.yaml
```