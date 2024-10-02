using Balsam.Model;
using BalsamApi.Server.Models;

namespace Balsam.Api.Extensions
{
    public static class BalsamRepoFileExtension
    {
        public static RepoFile ToRepoFile(this BalsamRepoFile repoFile)
        {
            var file = new RepoFile
            {
                Id = repoFile.Id,
                Name = repoFile.Name,
                Path = repoFile.Path,
                ContentUrl = repoFile.ContentUrl,
            };

            switch (repoFile.Type)
            {
                case BalsamRepoFile.TypeEnum.FileEnum:
                    file.Type = RepoFile.TypeEnum.FileEnum;
                    break;
                case BalsamRepoFile.TypeEnum.FolderEnum:
                    file.Type = RepoFile.TypeEnum.FolderEnum;
                    break;
                default:
                    throw new ApplicationException("Could not convert type " + repoFile.Type.ToString());
            }

            return file;
        }
    }
}
