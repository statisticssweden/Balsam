namespace Balsam.Model
{
    public class CapabilityOptions
    {
        public bool Enabled { get; set; }
        public string? ServiceLocation { get; set; }
        public string[]? Actions { get; set; }
    }

    public class Capabilities
    {
        public const string Git = "Git";
        public const string S3 = "S3";
        public const string Authentication = "Authentication";
        public const string Chat = "Chat";
    }
}
