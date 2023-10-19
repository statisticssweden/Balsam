namespace GitLabProvider.Client
{
    public class GitLabTreeFile
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public string? path { get; set; }
        public string? mode { get; set; }

        public GitLabTreeFile()
        {
        }
    }
}
