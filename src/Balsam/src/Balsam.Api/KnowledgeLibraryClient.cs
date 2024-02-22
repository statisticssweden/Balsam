using BalsamApi.Server.Models;
using LibGit2Sharp;

namespace Balsam.Api
{
    public class KnowledgeLibraryClient
    {
        public List<RepoFile> GetRepositoryContent(string repositoryId, string repositoryUrl)
        {
            string localRepositoryPath = Path.Combine(Path.GetTempPath(), "kb", repositoryId);

            Repository repository = GetOrCreateRepository(localRepositoryPath, repositoryUrl);

            var repositoryContents = new List<RepoFile>();

            AddContents(localRepositoryPath, localRepositoryPath.Length + 1, repositoryContents);

            return repositoryContents;
        }

        private Repository GetOrCreateRepository(string localRepositoryPath, string repositoryUrl)
        {
            if (Directory.Exists(localRepositoryPath))
            {
                //Pull repository changes
                var pullOptions = new PullOptions();
                pullOptions.FetchOptions = new FetchOptions();
                var repository = new Repository(localRepositoryPath);
                Commands.Pull(repository, new Signature("x", "x@x.com", DateTimeOffset.Now), pullOptions);
                return repository;
            }
            else
            {
                //Clone repository
                Repository.Clone(repositoryUrl, localRepositoryPath);
                return new Repository(localRepositoryPath);
            }
        }

        void AddContents(string path, int relativePathPosition, List<RepoFile> contents)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                contents.Add(CreateRepoFileFromDirectory(directory, relativePathPosition));
                AddContents(directory, relativePathPosition, contents);
            }
            foreach (var file in Directory.GetFiles(path))
            {
                contents.Add(CreateRepoFileFromFile(file, relativePathPosition));
            }
        }

        RepoFile CreateRepoFileFromFile(string path, int relativePathPosition)
        {
            var repoFile = new RepoFile();
            repoFile.Path = path.Substring(relativePathPosition);
            repoFile.Name = Path.GetFileName(path);
            repoFile.Id = Guid.NewGuid().ToString();
            repoFile.Type = RepoFile.TypeEnum.FileEnum;
            return repoFile;
        }

        RepoFile CreateRepoFileFromDirectory(string path, int relativePathPosition)
        {
            var repoFile = new RepoFile();
            repoFile.Path = path.Substring(relativePathPosition);
            repoFile.Name = Path.GetFileName(path);
            repoFile.Id = Guid.NewGuid().ToString();
            repoFile.Type = RepoFile.TypeEnum.FolderEnum;
            return repoFile;
        }
    }
}
