using OidcProvider.Models;

namespace Keycloak.OidcProvider.Client;

public interface IKeyCloakClient
{
    Task<string> CreateRole(string program);
    Task<RoleCreatedResponse> CreateGroup(string name);
}