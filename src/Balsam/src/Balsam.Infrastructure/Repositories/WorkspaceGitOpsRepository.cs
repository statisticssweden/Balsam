using Balsam.Model;
using Balsam.Utility;

namespace Balsam.Repositories
{
    public class WorkspaceGitOpsRepository : GitOpsRepository, IWorkspaceGitOpsRepository
    {
        public static string WorkspaceUrlAnnotationKey = "balsam-workspace-url";

        public WorkspaceGitOpsRepository(IHubRepositoryClient hubRepoClient)
            : base(hubRepoClient)
        {
        }

        public async Task CreateWorkspaceManifests(BalsamProject project, BalsamBranch branch, BalsamWorkspace workspace, UserInfo user, string templateId)
        {
            var workspacePath = HubPaths.GetWorkspacePath(project.Id, branch.Id, user.UserName, workspace.Id);
            var context = new WorkspaceContext(project, branch, workspace, user);

            HubRepositoryClient.PullChanges();

            string workspaceTemplatesPath = HubPaths.GetWorkspaceTemplatesPath(templateId);
            await CreateManifests(context, workspacePath, workspaceTemplatesPath);

            HubRepositoryClient.PersistChanges($"Manifests for workspace {workspace.Id} persisted");

        }

        public async Task DeleteWorkspaceManifests(string projectId, string branchId, string workspaceId, string userName)
        {
            var workspacePath = HubPaths.GetWorkspacePath(projectId, branchId, userName, workspaceId);

            HubRepositoryClient.PullChanges();

            await DeleteManifests(workspacePath);

            HubRepositoryClient.PersistChanges($"Deleted workspace manifests with id {workspaceId}");
        }

        public async Task<string> GetWorkspaceUrl(string projectId, string branchId, string userName, string workspaceId, string urlConfigFile)
        {
            //TODO: Pull?

            return await Task.Run(() =>
            {
                string manifestFilePath = GetWorkspaceManifestFilePath(projectId, branchId, userName, workspaceId, urlConfigFile);
 
                return ManifestUtil.GetAnnotation(manifestFilePath, WorkspaceUrlAnnotationKey);
            });
        }

        private string GetWorkspaceManifestFilePath(string projectId, string branchId, string userName, string workspaceId, string manifestFileName)
        {
            return Path.Combine(HubPaths.GetWorkspacePath(projectId, branchId, userName, workspaceId), manifestFileName);
        }
    }
}
