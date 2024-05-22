using Balsam.Model;
using BalsamApi.Server.Models;

namespace Balsam.Api.Extensions
{
    public static class BalsamWorkspaceExtension
    {
        public  static Workspace ToWorkspace(this BalsamWorkspace w)
        {
            return new Workspace
            {
                Id = w.Id,
                Name = w.Name,
                ProjectId = w.ProjectId,
                BranchId = w.BranchId,
                TemplateId = w.TemplateId,
                Url = w.Url,
                Owner = w.Owner
            };
        }
    }
}
