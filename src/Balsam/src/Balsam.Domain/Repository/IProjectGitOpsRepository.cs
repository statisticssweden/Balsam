using Balsam.Model;

namespace Balsam.Repositories
{
    public interface IProjectGitOpsRepository
    {
        Task CreateProjectManifests(BalsamProject project);
        Task DeleteBranchManifests(string projectId, string branchId);
        Task DeleteProjectManifests(string projectId);
    }
}