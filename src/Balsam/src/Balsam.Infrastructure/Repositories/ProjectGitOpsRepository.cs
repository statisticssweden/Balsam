using Balsam.Model;

namespace Balsam.Repositories
{
    public class ProjectGitOpsRepository : GitOpsRepository, IProjectGitOpsRepository
    {

        public ProjectGitOpsRepository(IHubRepositoryClient hubRepoClient)
            : base(hubRepoClient)
        {
        }

        public async Task CreateProjectManifests(BalsamProject project)
        {
            string projectPath = HubPaths.GetProjectPath(project.Id);
            var context = new ProjectContext() { Project = project };

            HubRepositoryClient.PullChanges();
            string projectsTemplatePath = HubPaths.GetProjectsTemplatesPath();
            await CreateManifests(context, projectPath, projectsTemplatePath);

            HubRepositoryClient.PersistChanges($"Manifests for project {project.Id} persisted");
        }

        public async Task DeleteBranchManifests(string projectId, string branchId)
        {
            var branchPath = HubPaths.GetBranchPath(projectId, branchId);

            HubRepositoryClient.PullChanges();

            await DeleteManifests(branchPath);

            HubRepositoryClient.PersistChanges($"Branch {branchId} deleted for project {projectId}");
        }

        public async Task DeleteProjectManifests(string projectId)
        {
            var projectPath = HubPaths.GetProjectPath(projectId);

            HubRepositoryClient.PullChanges();

            await DeleteManifests(projectPath);

            HubRepositoryClient.PersistChanges($"Project {projectId} deleted");
        }
    }
}
