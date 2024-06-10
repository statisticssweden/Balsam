namespace Balsam.Model
{
    public class BalsamBranch
    {
        public string Id { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GitBranch { get; set; }
    }
}
