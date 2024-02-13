using System.Security.Principal;

namespace Balsam.Api.Models
{
    public class GitData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string? SourceLocation { get; set; }
        public bool HasTemplate { get { return !string.IsNullOrEmpty(SourceLocation); } }
    }
}
