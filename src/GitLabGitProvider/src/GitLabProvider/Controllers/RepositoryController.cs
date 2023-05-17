using GitProvider.Controllers;
using GitProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GitLabProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryController : RepositoryApiController
    {
        public override IActionResult CreateBranch([FromRoute(Name = "repository"), Required] string repository, [FromQuery(Name = "preferredName")] string? preferredName)
        {
            return Ok(new BranchCreatedResponse() { Repository = repository, PreferredName = preferredName, Name = preferredName });
        }

        public override IActionResult CreateRepo([FromQuery(Name = "preferredName"), Required] string preferredName)
        {
            return Ok(new RepositoryCreatedResponse() { Name = preferredName, PreferredName = preferredName, Path = @$"https:\\my-git.local\{preferredName}.git"});
        }
    }
}
