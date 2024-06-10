using Balsam.Interfaces;
using Balsam.Model;
using Balsam.Repositories;

namespace Balsam.Services
{

    public class KnowledgeLibraryService : IKnowledgeLibraryService
    {
        private readonly IKnowledgeLibraryRepository _knowledgeLibraryRepository; 


        public KnowledgeLibraryService(IKnowledgeLibraryRepository knowledgeLibraryRepository)
        {
            _knowledgeLibraryRepository = knowledgeLibraryRepository;
        }

        public async Task<List<BalsamKnowledgeLibrary>> ListKnowledgeLibraries()
        {
            return await _knowledgeLibraryRepository.ListKnowledgeLibraries();
        }

        public async Task<BalsamKnowledgeLibrary> GetKnowledgeLibrary(string knowledgeLibraryId)
        {
            return await _knowledgeLibraryRepository.GetKnowledgeLibrary(knowledgeLibraryId);
        }

        public async Task<List<BalsamRepoFile>> GetRepositoryFileTree(string knowledgeLibraryId)
        {
            return await _knowledgeLibraryRepository.GetRepositoryFileTree(knowledgeLibraryId);
        }

        public string GetRepositoryFilePath(string repositoryId, string fileId)
        {
            return _knowledgeLibraryRepository.GetRepositoryFilePath(repositoryId, fileId);
        }

        public async Task<string> GetZippedResource(string repositoryId, string repositoryUrl, string fileId)
        {
            return await _knowledgeLibraryRepository.GetZippedResource(repositoryId, repositoryUrl, fileId);
        }
    }
}
