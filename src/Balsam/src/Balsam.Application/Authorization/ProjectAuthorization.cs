using Balsam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Balsam.Application.Authorization
{
    public class ProjectAuthorization
    {
        public bool CanUserDeleteProject(ClaimsPrincipal user, BalsamProject project)
        {
            return user.Claims.Any(c => c.Type == "groups" && c.Value == project.Oidc.GroupName);
        }

        public bool CanUserCreateBranch(ClaimsPrincipal user, BalsamProject project)
        {
            return user.Claims.Any(c => c.Type == "groups" && c.Value == project.Oidc.GroupName);
        }

        public bool CanUserEditBranch(ClaimsPrincipal user, BalsamProject project)
        {
            return user.Claims.Any(c => c.Type == "groups" && c.Value == project.Oidc.GroupName);
        }

        public bool CanUserDeleteBranch(ClaimsPrincipal user, BalsamProject project)
        {
            return user.Claims.Any(c => c.Type == "groups" && c.Value == project.Oidc.GroupName);
        }
    }
}
