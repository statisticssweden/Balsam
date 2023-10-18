using System.Text;
using ChatProvider.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RocketChatChatProvider.Configuration;

namespace RocketChatChatProvider.Client;

public interface IRocketChatClient
{
    Task<AreaCreatedResponse?> CreateArea(string channelName);
}

public class RocketChatClient : IRocketChatClient
{
    private readonly ILogger<RocketChatClient> _logger;
    private readonly ApiOptions _api;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _camelCase;

    public RocketChatClient(IOptions<ApiOptions> options, ILogger<RocketChatClient> logger, HttpClient httpClient)
    {
        _api = options.Value;
        _logger = logger;
        _httpClient = httpClient;
        _camelCase = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public async Task<AreaCreatedResponse?> CreateArea(string channelName)
    {
        _logger.LogDebug("Starting creating channel {channel}", channelName);

        var requestUri = $"{_api.BaseUrl}api/v1/channels.create";

        var channel = new RocketChatChannel(channelName, "", false, true);

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(JsonConvert.SerializeObject(channel, _camelCase), Encoding.UTF8, "application/json")
        };

        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", _api.Token);
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", _api.UserId);

        try
        {
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            if (response.Content is not {Headers.ContentType.MediaType: "application/json"}) return null;

            var content = await response.Content.ReadAsStringAsync();

            dynamic createdResponse = JsonConvert.DeserializeObject(content) ?? throw new DeserializeException();
            
            var chatResponse = new AreaCreatedResponse
            {
                Id = createdResponse.channel._id,
                Name = createdResponse.channel.name
            };

            _logger.LogInformation("Channel created: {channel}", chatResponse.Name);
            return chatResponse;
        }
        catch (DeserializeException e)
        {
            _logger.LogError(e,"Could not interpret the response object from Rocket.Chat-api.");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred when creating channel {channel}", channelName);
            return null;
        }
    }
}

public class DeserializeException : Exception
{
}