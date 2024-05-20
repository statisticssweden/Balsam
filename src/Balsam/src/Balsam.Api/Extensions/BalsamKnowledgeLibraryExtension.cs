using Balsam.Model;
using BalsamApi.Server.Models;

namespace Balsam.Api.Extensions
{
    public static class BalsamKnowledgeLibraryExtension
    {
        public static KnowledgeLibrary ToKnowledgeLibrary(this BalsamKnowledgeLibrary bkl)
        {
            return new KnowledgeLibrary
            {
                Name = bkl.Name,
                RepositoryUrl = bkl.RepositoryUrl,
                Description = bkl.Description,
                Id = bkl.Id,
                RepositoryFriendlyUrl = bkl.RepositoryFriendlyUrl,
            };
        }
    }
}
