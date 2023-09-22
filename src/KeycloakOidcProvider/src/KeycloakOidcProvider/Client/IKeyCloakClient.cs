namespace Keycloak.OidcProvider.Client;

public interface IKeyCloakClient
{
    Task<string> CreateRole(string program);
}