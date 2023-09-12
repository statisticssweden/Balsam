namespace GitLabProvider.Client
{
    public partial class GitLabClient
    {
        public class GitContent
        {
            public string? branch { get; set; }
            public string? commit_message { get; set; }
            public List<Action>? actions { get; set; }
        }
    }
}
