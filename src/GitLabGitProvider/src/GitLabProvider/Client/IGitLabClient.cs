using Microsoft.AspNetCore.Mvc;

namespace GitLabProvider.Client
{
    public interface IGitLabClient
    {
        Task<RepositoryInfo?> CreateProjectRepo(string repoName, string description, string defaultBranchName);

        Task<bool> CreateBranch(string repositoryId, string fromBranch, string branchName);
        Task<bool> DeleteBranch(string repositoryId, string branchId);

        Task<string?> CreatePAT(string userName);

        Task<List<GitLabTreeFile>> GetFiles(string repositoryId, string branchName);

        Task<FileContentResult> GetFile(string repositoryId, string branchName, string fileId);
        Task<bool> DeleteRepository(string repositoryId);

        Task AddResourceFiles(string repositoryId, string branchId, string workPath);
    }
}
