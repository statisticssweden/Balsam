using Balsam.Model;
using BalsamApi.Server.Models;

namespace Balsam.Api.Extensions
{
    public static class WorkspaceTemplateExtension
    {
        public static Template ToTemplate(this WorkspaceTemplate workspaceTemplate)
        {
            return new Template
            {
                Id = workspaceTemplate.Id,
                Name = workspaceTemplate.Name,
                Description = workspaceTemplate.Description
            };
        }
    }
}
