namespace GitLabProvider.Client
{
    public partial class GitLabClient
    {
        public class Action
        {
            public string? action { get; set; }
            public string? file_path { get; set; }
            public string? content { get; set; }
            public string? encoding { get; set; }
        }
    }
}
