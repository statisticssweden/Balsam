using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OidcProvider.Models;

namespace Keycloak.OidcProvider.Client;

public class KeyCloakClient : IKeyCloakClient
{
    private readonly ILogger _logger;
    private readonly string? _baseUrl;
    private readonly string? _userName;
    private readonly string? _password;
    private readonly string? _clientSecret;
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

    public async Task<GroupCreatedResponse> CreateGroup(string projectName)
    {
        var accessToken = await GetAccessToken();
        var group = new Group(projectName);

        var requestBody = new { name = group.Name };
        var requestUri = $"{_baseUrl}/admin/realms/{_realm}/groups";

        try
        {
            using var response = await PostRequest(requestUri, requestBody, accessToken);
            response.EnsureSuccessStatusCode();

            var groupId = response.Headers.Location?.Segments.Last();

            _logger.LogInformation("User Group created for project: {projectName}, with name {group}", projectName, group.Name);

            return new GroupCreatedResponse { Id = groupId, Name = group.Name };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user group for project [{projectName}] with name [{group}]", projectName, group.PreferredName);
            throw;
        }

    }

    public async Task AddUserToGroup(string groupId, string userName)
    {
        var accessToken = await GetAccessToken();

        var userId = await GetUserId(userName, accessToken);
        var requestUri = $"{_baseUrl}/admin/realms/{_realm}/users/{userId}/groups/{groupId}";

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                }
            };
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred when adding user {userName} to group {groupId}", userName, groupId);
            throw;
        }
    }

    private async Task<string> GetUserId(string userName, string accessToken)
    {
        var requestUri = $"{_baseUrl}/admin/realms/{_realm}/users?username={userName}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri)
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
            }
        };
        
        try
        {
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var responseContentStream = response.Content.ReadAsStringAsync().Result;

            var user = JsonConvert.DeserializeObject<List<User>>(responseContentStream);
            return user.First().Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to Id for user: {userName}", userName);
            throw;
        }
    }

    private async Task<HttpResponseMessage> PostRequest(string requestUri, object requestBody, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
            }
        };
        return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
    }

    private async Task<string> GetAccessToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token")
        {
            Content = new FormUrlEncodedContent(new KeyValuePair<string?, string?>[]
            {
                    new("client_id", _clientId),
                    new("client_secret", _clientSecret),
                    new("username", _userName),
                    new("password", _password),
                    new("grant_type", "password")
            })
        };

        try
        {
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var responseContentStream = response.Content.ReadAsStringAsync().Result;

            var accessToken = JsonConvert.DeserializeObject<AccessToken>(responseContentStream);
            return accessToken.access_token;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve accessToken");
            throw;
        }
    }

    private async Task<string> GetIdForClient(string accesstoken, string clientId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/admin/realms/{_realm}/clients?clientId={clientId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        try
        {
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var responseContentStream = response.Content.ReadAsStringAsync().Result;

            var user = JsonConvert.DeserializeObject<List<User>>(responseContentStream);
            return user.First().Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get client Id from Keycloak api for client: {clientId}", clientId);
            throw;
        }
    }

    private class AccessToken
    {
        //[JsonPropertyName("access_token")]
        public string access_token { get; set; }
    }

    private class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

}