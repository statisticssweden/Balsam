namespace Balsam.Api.Models
{
    public class BalsamWorkspace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TemplateId { get; set; }

        public BalsamWorkspace(string id, string name, string templateId) 
        { 
            Id = id;
            Name = name;
            TemplateId = templateId;    
        }
    }
}
