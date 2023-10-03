﻿using System.Net.Http.Headers;
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

    public async Task<RoleCreatedResponse> CreateGroup(string projectName)
    {
        var group = new Group(projectName);
        try
        {
            var accessToken = await GetAccessToken();
            var id = await GetIdForClient(accessToken, _clientId);

            var jsonBody = new { name = group.Name };

            var request =
                new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/admin/realms/{_realm}/groups")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(jsonBody), Encoding.UTF8, "application/json"),
                    Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessToken) }
                };

            try
            {
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var groupId = GetIdFromLocation( response.Headers.Location);

                _logger.LogInformation("User Group created for project: {projectName}, with name {group}", projectName, group.Name);

                return new RoleCreatedResponse {Id = groupId, Name = group.Name };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to POST group on Keycloak API");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user group for project [{projectName}] with name [{group}]", projectName, group.PreferredName);
            throw;
        }
    }

    private string GetIdFromLocation(Uri headersLocation)
    {
        var id = headersLocation.Segments.Last();
        return id;
    }

    public async Task<string> CreateRole(string program)
    {
        var role = new Role(program);
        try
        {
            var accessToken = await GetAccessToken();
            var id = await GetIdForClient(accessToken, _clientId);
            
            var jsonBody = new {name = role.Name, description = "Balsam users for project " + role.PreferredName};

            var request =
                new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/admin/realms/{_realm}/clients/{id}/roles")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(jsonBody), Encoding.UTF8, "application/json"),
                    Headers = {Authorization = new AuthenticationHeaderValue("Bearer", accessToken)}
                };

            try
            {
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var responseContentStream = response.Content.ReadAsStringAsync().Result;
                _logger.LogInformation("User Role created for program: {program}", program);

                return role.Name;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to POST role on Keycloak API");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create role with name {role}", role.PreferredName);
            throw;
        }
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

            var id = JsonConvert.DeserializeObject<List<Id>>(responseContentStream);
            return id.First().id;
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

    private class Id
    {
        public string id { get; set; }
    }
}