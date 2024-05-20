using Balsam.Model;

namespace Balsam.Repositories
{
    public interface IKnowledgeLibraryRepository
    {
        Task<BalsamKnowledgeLibrary> GetKnowledgeLibrary(string knowledgeLibraryId);
        Task<List<BalsamRepoFile>> GetRepositoryFileTree(string knowledgeLibraryId);
        string GetRepositoryFilePath(string repositoryId, string fileId);
        Task<string> GetZippedResource(string repositoryId, string repositoryUrl, string fileId);
        Task<List<BalsamKnowledgeLibrary>> ListKnowledgeLibraries();
    }
}