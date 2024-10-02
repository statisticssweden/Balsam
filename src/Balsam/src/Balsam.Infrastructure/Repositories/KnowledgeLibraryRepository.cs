using Balsam.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Balsam.Repositories
{
    public class KnowledgeLibraryRepository : IKnowledgeLibraryRepository
    {
        private readonly ILogger<KnowledgeLibraryRepository> _logger;
        private readonly IHubRepositoryClient _hubRepositoryClient;
        private readonly IKnowledgeLibraryContentRepository _knowledgeLibraryContentRepository;
        protected HubPaths HubPaths { get; set; }

        public KnowledgeLibraryRepository(ILogger<KnowledgeLibraryRepository> logger, IHubRepositoryClient hubRepositoryClient, IKnowledgeLibraryContentRepository knowledgeLibraryContentRepository)
        {
            _logger = logger;
            _hubRepositoryClient = hubRepositoryClient;
            _knowledgeLibraryContentRepository = knowledgeLibraryContentRepository;
            HubPaths = new HubPaths(_hubRepositoryClient.Path);
        }

        public async Task<List<BalsamKnowledgeLibrary>> ListKnowledgeLibraries()
        {
            var knowledgeLibraries = new List<BalsamKnowledgeLibrary>();

            var kbPath = HubPaths.GetKnowledgeLibrariesPath();
            _logger.LogDebug("kbPath: " + kbPath);
            foreach (var knowledgelibraryFile in Directory.GetFiles(kbPath))
            {
                try
                {
                    var knowledgeLibrary = JsonConvert.DeserializeObject<BalsamKnowledgeLibrary>(await File.ReadAllTextAsync(knowledgelibraryFile));
                    if (knowledgeLibrary != null)
                    {
                        knowledgeLibraries.Add(knowledgeLibrary);
                    }
                    else
                    {
                        _logger.LogWarning($"Could not parse properties file for knowledgelibrary {knowledgelibraryFile}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error with deserialization of file");
                }
            }
            return knowledgeLibraries;
        }

        public async Task<BalsamKnowledgeLibrary> GetKnowledgeLibrary(string knowledgeLibraryId)
        {
            return (await ListKnowledgeLibraries()).First(kb => kb.Id == knowledgeLibraryId);
        }

        public async Task<List<BalsamRepoFile>> GetRepositoryFileTree(string knowledgeLibraryId)
        {
            var knowledgeLibrary = await GetKnowledgeLibrary(knowledgeLibraryId);

            return await _knowledgeLibraryContentRepository.GetFileTree(knowledgeLibraryId, knowledgeLibrary.RepositoryUrl);
        }

        public string GetRepositoryFilePath(string repositoryId, string fileId)
        {
            return _knowledgeLibraryContentRepository.GetFilePath(repositoryId, fileId);
        }

        public async Task<string> GetZippedResource(string repositoryId, string repositoryUrl, string fileId)
        {
            return await _knowledgeLibraryContentRepository.GetZippedResource(repositoryId, repositoryUrl, fileId);
        }
    }
}
