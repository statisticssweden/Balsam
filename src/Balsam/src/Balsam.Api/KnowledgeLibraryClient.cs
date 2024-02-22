using BalsamApi.Server.Models;
using LibGit2Sharp;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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

        public string GetRepositoryFilePath(string repositoryId, string fileId)
        {
            string relativePath = Encoding.UTF8.GetString(Convert.FromBase64String(fileId));

            if (relativePath.Contains(".."))
            {
                throw new ArgumentException("Invalid file id");
            }

            string filePath = Path.Combine(Path.GetTempPath(), "kb", repositoryId, relativePath);

            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File not found");
            }

            return filePath;

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
            repoFile.Id = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(repoFile.Path));
            repoFile.Type = RepoFile.TypeEnum.FileEnum;
            return repoFile;
        }

        RepoFile CreateRepoFileFromDirectory(string path, int relativePathPosition)
        {
            var repoFile = new RepoFile();
            repoFile.Path = path.Substring(relativePathPosition);
            repoFile.Name = Path.GetFileName(path);
            repoFile.Id = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(repoFile.Path)); 
            repoFile.Type = RepoFile.TypeEnum.FolderEnum;
            return repoFile;
        }
    }
}
