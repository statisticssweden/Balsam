using System.Text.Json.Serialization;

namespace RocketChatChatProvider.Client
{
    public class RocketChatChannelDelete
    {
        [JsonPropertyName("roomId")]
        public string roomId { get; }

        public RocketChatChannelDelete(string roomid)
        {
            roomId= roomid;
        }
    }
}
