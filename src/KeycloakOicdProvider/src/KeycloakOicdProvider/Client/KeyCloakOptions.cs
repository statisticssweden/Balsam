namespace Keycloak.OicdProvider.Client;

public class KeyCloakOptions
{
    public string? BaseUrl { get; set; }
    public string? Realm { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? ClientSecret { get; set; }
    public string? ClientId { get; set; }
}