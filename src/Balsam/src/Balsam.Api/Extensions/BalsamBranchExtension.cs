using Balsam.Model;
using BalsamApi.Server.Models;

namespace Balsam.Api.Extensions
{
    public static class BalsamBranchExtension
    {
        public static Branch ToBranch(this BalsamBranch b)
        {
            return new Branch()
            {
                Id = b.Id,
                Description = b.Description,
                Name = b.Name,
                IsDefault = b.IsDefault
            };
        }
    }
}
