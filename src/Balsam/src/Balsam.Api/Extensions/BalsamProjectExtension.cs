using Balsam.Model;
using BalsamApi.Server.Models;
using System.Xml.Linq;

namespace Balsam.Api.Extensions
{
    public static class BalsamProjectExtension
    {
        public static Project ToProject(this BalsamProject project)
        {
            return new Project()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Branches = project.Branches.Select(b => b.ToBranch()).ToList(),
                AuthGroup = project.Oidc?.GroupName,
                GitUrl = project.Git?.Path
            };
        }


        //TODO: Do we need both Project and ProjectResponse?
        public static ProjectResponse ToProjectResponse(this BalsamProject project)
        {
            return new ProjectResponse()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Branches = project.Branches.Select(b => b.ToBranch()).ToList(),
                AuthGroup = project.Oidc?.GroupName,
                GitUrl = project.Git?.Path
            };
        }
    }
}
