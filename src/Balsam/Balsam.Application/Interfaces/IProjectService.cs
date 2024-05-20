using Balsam.Model;

namespace Balsam.Interfaces
{
    public interface IProjectService
    {
        Task CopyFromKnowledgeLibrary(string projectId, string branchId, string knowledgeLibraryId, string fileId);
        Task<BalsamBranch?> CreateBranch(string projectId, string fromBranch, string branchName, string description);
        Task<BalsamProject?> CreateProject(string preferredName, string description, string defaultBranchName, string username, string? sourceLocation = null);
        Task DeleteBranch(string projectId, string branchId);
        Task DeleteProject(string projectId);
        Task<BalsamBranch?> GetBranch(string projectId, string branchId);
        Task<FileContent> GetFile(string projectId, string branchId, string fileId);
        Task<List<BalsamRepoFile>?> GetGitBranchFiles(string projectId, string branchId);
        Task<BalsamProject?> GetProject(string projectId, bool includeBranches = true);
        Task<List<BalsamProject>> GetProjects(bool includeBranches = true);
    }
}