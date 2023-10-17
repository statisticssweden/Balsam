namespace Balsam.Api.Models
{
    public class ChatData
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ChatData(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
