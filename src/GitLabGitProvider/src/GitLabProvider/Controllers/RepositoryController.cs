using GitLabProvider.Client;
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
        IGitLabClient _gitLabClient;

        public RepositoryController(IGitLabClient gitLabClient)
        {
            _gitLabClient = gitLabClient;
        }
        public override IActionResult CreateBranch([FromRoute(Name = "repository"), Required] string repository, [FromQuery(Name = "preferredName")] string? preferredName)
        {
            if (_gitLabClient.CreateBranch(preferredName, repository).Result)
            {
                return Ok(new BranchCreatedResponse() { Repository = repository, PreferredName = preferredName, Name = preferredName });
            }
            return BadRequest(new Problem() { Type = "404", Title = "Could not create branch"});
        }

        public override IActionResult CreateRepo([FromQuery(Name = "preferredName"), Required] string preferredName)
        {
            var repoInfo = _gitLabClient.CreateProjectRepo(preferredName).Result;
            if (repoInfo != null)
            {
                return Ok(new RepositoryCreatedResponse() { Name = repoInfo.Name, PreferredName = preferredName, Path = repoInfo.Url, Id = repoInfo.Id });
            }
            return BadRequest(new Problem() { Type = "404", Title = "Could not create repository" });
        }
    }
}
