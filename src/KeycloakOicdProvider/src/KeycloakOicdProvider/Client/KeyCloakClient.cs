using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Keycloak.OicdProvider.Client;

public class KeyCloakClient : IKeyCloakClient
{
    private readonly ILogger _logger;
    private readonly string _baseUrl;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _clientSecret;
    private readonly string? _realm;
    private readonly string? _clientId;
    private static HttpClient _httpClient;

    public KeyCloakClient(ILogger<KeyCloakClient> logger, IOptions<KeyCloakOptions> options, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _clientId = options.Value.ClientId;
        _baseUrl = options.Value.BaseUrl;
        _realm = options.Value.Realm;
        _userName = options.Value.User;
        _password = options.Value.Password;
        _clientSecret = options.Value.ClientSecret;
    }


    public async Task<string> CreateRole(string program)
    {
        var accesstoken = await GetAccessToken();
        var id = await GetIdForClient(accesstoken, _clientId);
        var roleName = $"{program}-users";

        //TODO: is name of role best suited? Programname-users, balsam-programname
        var myObject = new { name = roleName, description = "Balsam programanvändare för program " + program };
        
        var test = JsonConvert.SerializeObject(myObject);

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/admin/realms/{_realm}/clients/{id}/roles")
        {

            Content = new StringContent(test, Encoding.UTF8, "application/json"),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accesstoken) }
        };

        //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        //try
        response.EnsureSuccessStatusCode();

        var responseContentStream = response.Content.ReadAsStringAsync().Result;
        _logger.LogInformation("User Role created for program: {program}", program);
        return "Role";
    }

    private async Task<string> GetAccessToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token")
        {
            Content = new FormUrlEncodedContent(new KeyValuePair<string?, string?>[]
            {
                    new("client_id", "admin-cli"),
                    new("client_secret", _clientSecret),
                    new("username", _userName),
                    new("password", _password),
                    new("grant_type", "password")
            })
        };

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var responseContentStream = response.Content.ReadAsStringAsync().Result;

        var accessToken = JsonConvert.DeserializeObject<AccessToken>(responseContentStream);
        return accessToken.access_token;
    }

    private async Task<string> GetIdForClient(string accesstoken, string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/admin/realms/{_realm}/clients?clientId={id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var responseContentStream = response.Content.ReadAsStringAsync().Result;

        var clientId = JsonConvert.DeserializeObject<List<ID>>(responseContentStream);

        return clientId.FirstOrDefault().id;
    }

    private class AccessToken
    {
        public string access_token { get; set; }
    }
    private class ID
    {
        public string id { get; set; }
    }
}