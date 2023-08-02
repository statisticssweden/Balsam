namespace GitLabProvider.Client
{
    public interface IGitLabClient
    {
        Task<RepositoryInfo?> CreateProjectRepo(string repoName);

        Task<bool> CreateBranch(string branchname, string repositoryId);

        Task<string?> CreatePAT(string userName);
    }
}
