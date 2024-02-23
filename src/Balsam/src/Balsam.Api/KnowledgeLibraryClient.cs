using BalsamApi.Server.Models;
using LibGit2Sharp;
using System.IO.Compression;
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

        public string GetZippedResource(string repositoryId, string repositoryUrl, string fileId)
        {

            //Make sure that the repository is cloned and up to date
            string localRepositoryPath = Path.Combine(Path.GetTempPath(), "kb", repositoryId);
            Repository repository = GetOrCreateRepository(localRepositoryPath, repositoryUrl);

            //Make sure that the file/directory exists in the repository
            string relativePath = Encoding.UTF8.GetString(Convert.FromBase64String(fileId));
            if (relativePath.Contains(".."))
            {
                throw new ArgumentException("Invalid file id");
            }

            string filePath = Path.Combine(Path.GetTempPath(), "kb", repositoryId, relativePath);

            string workPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());


            if (File.Exists(filePath))
            {
                File.Copy(filePath, Path.Combine(workPath, Path.GetFileName(filePath)));
            } else if (Directory.Exists(filePath))
            {
                CopyDirectory(filePath, workPath);
            }
            else
            {
                throw new ArgumentException("File/Directory not found");
            }

            string zipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");
            ZipFile.CreateFromDirectory(workPath, zipPath);

            Directory.Delete(workPath, true);

            return zipPath;

        }

        private static void CopyDirectory(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // Append the name of the source directory to the destination directory
            destDirName = Path.Combine(destDirName, dir.Name);

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
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
