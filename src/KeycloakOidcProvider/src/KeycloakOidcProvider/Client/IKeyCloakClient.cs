namespace Keycloak.OicdProvider.Client;

public interface IKeyCloakClient
{
    Task<string> CreateRole(string program);
}