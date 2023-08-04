namespace Balsam.Api.Models
{
    public class HubRepositoryOptions
    {
        public const string SectionName = "HubRepo";

        public string RemoteUrl { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }

    }
}
