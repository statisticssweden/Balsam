using System.Text.Json.Serialization;

namespace RocketChatChatProvider.Client;

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