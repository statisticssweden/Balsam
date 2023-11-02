namespace Balsam.Api.Models
{
    public class BalsamWorkspace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TemplateId { get; set; }
        public string ProjectId { get; set; }
        public string BranchId { get; set; }
        public string Url { get; set; }

        public BalsamWorkspace(string id, string name, string templateId, string projectId, string branchId) 
        { 
            Id = id;
            Name = name;
            TemplateId = templateId;    
            ProjectId = projectId;
            BranchId = branchId;
        }
    }
}
