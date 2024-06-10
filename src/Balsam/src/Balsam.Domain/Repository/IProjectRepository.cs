
using Balsam.Model;

namespace Balsam.Repositories
{
    public interface IProjectRepository
    {
        Task<BalsamBranch?> CreateBalsamBranchFromGitBranch(string projectId, string gitBranchName);
        Task<BalsamBranch?> CreateBranch(string projectId, string branchName, string description);
        Task<BalsamProject?> CreateProject(BalsamProject project, string defaultBranchName);
        Task DeleteBranch(string projectId, string branchId);
        Task DeleteProject(string projectId);
        Task<BalsamBranch?> GetBranch(string projectId, string branchId);
        //Task<BalsamProject> GetProject(string projectId);
        Task<BalsamProject?> GetProject(string projectId, bool includeBranches = true);
        Task<List<BalsamProject>> GetProjects(bool includeBranches = true);
        Task<bool> ProjectExists(string projectName);
        Task<FileContent> GetFile(string projectId, string branchId, string fileId);
        Task UpdateProject(BalsamProject project);
    }
}