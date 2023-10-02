namespace Balsam.Api.Models
{
    public class BalsamProject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public S3Data S3 { get; set; }
        public GitData Git { get; set; }
        public List<BalsamBranch> Branches { get; set; }

        public BalsamProject()
        {
            Branches = new List<BalsamBranch>();
        }
    }
}
