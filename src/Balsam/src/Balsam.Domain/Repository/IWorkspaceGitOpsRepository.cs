
using Balsam.Model;

namespace Balsam.Repositories
{
    public interface IWorkspaceGitOpsRepository
    {
        Task CreateWorkspaceManifests(BalsamProject project, BalsamBranch branch, BalsamWorkspace workspace, UserInfo user, string templateId);
        Task DeleteWorkspaceManifests(string projectId, string branchId, string workspaceId, string userName);
        Task<string> GetWorkspaceUrl(string projectId, string branchId, string userName, string workspaceId, string urlConfigFile);
    }
}