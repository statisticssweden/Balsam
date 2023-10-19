﻿namespace GitLabProvider.Client
{
    public interface IGitLabClient
    {
        Task<RepositoryInfo?> CreateProjectRepo(string repoName, string description, string defaultBranchName);

        Task<bool> CreateBranch(string branchname, string repositoryId);

        Task<string?> CreatePAT(string userName);

        Task<List<GitLabTreeFile>> GetFiles(string repositoryId, string branchName);
    }
}
