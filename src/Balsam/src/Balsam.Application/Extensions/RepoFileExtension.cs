using Balsam.Model;
using GitProviderApiClient.Model;

namespace Balsam.Application.Extensions
{
    public static class RepoFileExtension
    {
        public static BalsamRepoFile ToBalsamRepoFile(this RepoFile repoFile)
        {
            var balsamRepoFile = new BalsamRepoFile
            {
                Id = repoFile.Id,
                ContentUrl = repoFile.ContentUrl,
                Name = repoFile.Name,
                Path = repoFile.Path,

            };
            
            switch (repoFile.Type)
            {
                case RepoFile.TypeEnum.File:
                    balsamRepoFile.Type = BalsamRepoFile.TypeEnum.FileEnum;
                    break;
                case RepoFile.TypeEnum.Folder:
                    balsamRepoFile.Type = BalsamRepoFile.TypeEnum.FolderEnum;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return balsamRepoFile;
        }
    }
}
