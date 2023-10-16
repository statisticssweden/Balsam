using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using ChatProvider.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RocketChatChatProvider.Configuration;

namespace RocketChatChatProvider.Client;

public interface IRocketChatClient
{
    Task CreateArea(CreateAreaRequest createAreaRequest);
}

public class RocketChatClient : IRocketChatClient
{
    private readonly ILogger<RocketChatClient> _logger;
    private readonly ApiOptions _api;
    private HttpClient _httpClient;
    private JsonSerializerSettings _camelCase;

    public RocketChatClient(IOptions<ApiOptions> options, ILogger<RocketChatClient> logger, HttpClient httpClient)
    {
        _api = options.Value;
        _logger = logger;
        _httpClient = httpClient;
        _camelCase = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public async Task CreateArea(CreateAreaRequest createAreaRequest)
    {
        _logger.LogDebug("Starting creating channel {channel}", createAreaRequest.Name);
        
        var requestUri = $"{_api.Endpoint}api/v1/channels.create";

        var channel = new RocketChatChannel(createAreaRequest.Name, "first", false, true);
        var jsonBody = JsonConvert.SerializeObject(channel, _camelCase);

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(JsonConvert.SerializeObject(channel, _camelCase), Encoding.UTF8, "application/json")
        };
        
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", _api.Token);
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", _api.UserId);

        try
        {
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            if (response.IsSuccessStatusCode)
            {
                return ;
            }
            else
            {
                
                _logger.LogError("An error occurred when creating channel {channel}", createAreaRequest.Name);

            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred when creating channel {channel}", createAreaRequest.Name);
            throw;
        }
        
        _logger.LogDebug("created channel {channel}", channel.Name);
    }
}

public class RocketChatChannel
{
    [JsonPropertyName("name")]
    public string Name { get; }
    
    [JsonPropertyName("excludeSelf")]
    public bool ExcludeSelf { get; }

    [JsonPropertyName("readOnly")]
    public bool ReadOnly { get; }

    [JsonPropertyName("members")]
    public IEnumerable<string> Members { get; set; }

    public RocketChatChannel(string name, string firstMember, bool? isReadOnly, bool excludeSelf)
    {
        Name = name;
        ExcludeSelf = excludeSelf;
        Members = new List<string>() {firstMember};
        ReadOnly = isReadOnly ?? false;
    }
}
