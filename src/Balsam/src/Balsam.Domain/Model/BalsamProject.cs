namespace Balsam.Model
{
    public class BalsamProject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public S3Data? S3 { get; set; }
        public GitData? Git { get; set; }
        public OidcData? Oidc { get; set; }
        public ChatData? Chat { get; set; }

        public List<BalsamBranch> Branches { get; set; }

        public BalsamProject(string name, string description)
        {
            Name = name;
            Description = description;
            Branches = new List<BalsamBranch>();
        }
    }
}
