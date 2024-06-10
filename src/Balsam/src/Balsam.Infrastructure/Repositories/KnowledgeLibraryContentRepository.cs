using Balsam.Model;
using LibGit2Sharp;
using System.IO.Compression;
using System.Text;

namespace Balsam.Repositories
{
    public class KnowledgeLibraryContentRepository : IKnowledgeLibraryContentRepository
    {
        public async Task<List<BalsamRepoFile>> GetFileTree(string repositoryId, string repositoryUrl)
        {
            string localRepositoryPath = GetLocalRepositoryPath(repositoryId);

            await CloneOrPull(localRepositoryPath, repositoryUrl);

            var repositoryContents = new List<BalsamRepoFile>();

            await GetFileTree(localRepositoryPath, localRepositoryPath.Length + 1, repositoryContents);

            return repositoryContents;
        }

        private static string GetLocalRepositoryPath(string repositoryId)
        {
            return Path.Combine(Path.GetTempPath(), "kb", repositoryId);
        }

        public string GetFilePath(string repositoryId, string fileId)
        {
            string relativePath = Encoding.UTF8.GetString(Convert.FromBase64String(fileId));

            if (relativePath.Contains(".."))
            {
                throw new ArgumentException("Invalid file id");
            }

            string localRepositoryPath = GetLocalRepositoryPath(repositoryId);

            string filePath = Path.Combine(localRepositoryPath, relativePath);

            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File not found");
            }

            return filePath;

        }
        
        /// <summary>
        /// Zips a resoruce (file or directory) and returns a path to the ziped file
        /// </summary>
        public async Task<string> GetZippedResource(string repositoryId, string repositoryUrl, string fileId)
        {
            //Make sure that the repository is cloned and up to date
            string localRepositoryPath = GetLocalRepositoryPath(repositoryId);

            await CloneOrPull(localRepositoryPath, repositoryUrl);

            //Make sure that the file/directory exists in the repository
            string relativePath = Encoding.UTF8.GetString(Convert.FromBase64String(fileId));
            if (relativePath.Contains(".."))
            {
                throw new ArgumentException("Invalid file id");
            }

            
            string filePath = Path.Combine(localRepositoryPath, relativePath);

            string workPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(workPath);


            if (File.Exists(filePath))
            {
                File.Copy(filePath, Path.Combine(workPath, Path.GetFileName(filePath)));
            }
            else if (Directory.Exists(filePath))
            {
                await CopyDirectory(filePath, workPath);
            }
            else
            {
                throw new ArgumentException("File/Directory not found");
            }

            string zipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

            await Task.Run(() =>
            {
                ZipFile.CreateFromDirectory(workPath, zipPath, CompressionLevel.SmallestSize, false);
            });

            Directory.Delete(workPath, true);

            return zipPath;
        }

        private static async Task CopyDirectory(string sourceDirName, string destDirName)
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

            await Task.Run(() =>
            {
                // If the destination directory doesn't exist, create it.       
                Directory.CreateDirectory(destDirName);

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(tempPath, false);
                }
            });

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                await CopyDirectory(subdir.FullName, tempPath);
            }
        }


        private async Task CloneOrPull(string localRepositoryPath, string repositoryUrl)
        {
            if (Directory.Exists(localRepositoryPath))
            {
                //Pull repository changes
                var pullOptions = new PullOptions();
                pullOptions.FetchOptions = new FetchOptions();
                
                await Task.Run(() =>
                {
                    using (var repository = new Repository(localRepositoryPath))
                    {
                        Commands.Pull(repository, new Signature("x", "x@x.com", DateTimeOffset.Now), pullOptions);
                    }
                });

            }
            else
            {
                //Clone repository
                await Task.Run(() =>
                {
                    Repository.Clone(repositoryUrl, localRepositoryPath);
                });
            }
        }

        private async Task GetFileTree(string path, int relativePathPosition, List<BalsamRepoFile> contents)
        {

            foreach (var directory in Directory.GetDirectories(path))
            {
                //Ignore .git folder
                if (directory.Substring(relativePathPosition).StartsWith(".git")) { continue; }

                contents.Add(CreateBalsamRepoFile(directory, relativePathPosition, BalsamRepoFile.TypeEnum.FolderEnum));
                await GetFileTree(directory, relativePathPosition, contents);
            }

            await Task.Run(() =>
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    contents.Add(CreateBalsamRepoFile(file, relativePathPosition, BalsamRepoFile.TypeEnum.FileEnum));
                }
            });
        }

        private static BalsamRepoFile CreateBalsamRepoFile(string path, int relativePathPosition, BalsamRepoFile.TypeEnum type)
        {
            var repoFile = new BalsamRepoFile();
            repoFile.Path = path.Substring(relativePathPosition);
            repoFile.Name = Path.GetFileName(path);
            repoFile.Id = Convert.ToBase64String(Encoding.UTF8.GetBytes(repoFile.Path));
            repoFile.Type = type;

            return repoFile;
        }
    }
}
