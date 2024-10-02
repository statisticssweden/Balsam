using Balsam.Model;

namespace Balsam.Repositories
{
    public interface IKnowledgeLibraryContentRepository
    {
        Task<List<BalsamRepoFile>> GetFileTree(string repositoryId, string repositoryUrl);
        string GetFilePath(string repositoryId, string fileId);
        Task<string> GetZippedResource(string repositoryId, string repositoryUrl, string fileId);
    }
}