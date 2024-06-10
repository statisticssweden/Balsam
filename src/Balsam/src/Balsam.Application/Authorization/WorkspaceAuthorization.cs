using Balsam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Balsam.Application.Authorization
{
    public class WorkspaceAuthorization
    {
        public bool CanUserCreateWorkspace(ClaimsPrincipal user, BalsamProject project)
        {
            return user.Claims.Any(c => c.Type == "groups" && c.Value == project.Oidc.GroupName);
        }

        public bool CanUserDeleteWorkspace(ClaimsPrincipal user, BalsamProject project, BalsamWorkspace workspace)
        {
            var userName = user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            return user.Claims.Any(c => c.Type == "groups" && c.Value == project.Oidc.GroupName) 
                                    && userName == workspace.Owner;
        }

        public bool CanUserGetWorkspaces(ClaimsPrincipal user, BalsamProject project)
        {
            return user.Claims.Any(c => c.Type == "groups" && c.Value == project.Oidc.GroupName);
        }
    }
}
