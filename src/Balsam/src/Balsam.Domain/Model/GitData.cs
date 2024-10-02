using System.Security.Principal;

namespace Balsam.Model
{
    public class GitData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string? SourceLocation { get; set; } //TODO: What is this used for?
        public bool HasTemplate { get { return !string.IsNullOrEmpty(SourceLocation); } } //TODO: What is this used for?
    }
}
