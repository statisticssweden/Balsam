namespace Balsam.Api.Models
{
    public class BalsamProgram
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public S3Data S3 { get; set; }
        public GitData Git { get; set; }
    }
}
