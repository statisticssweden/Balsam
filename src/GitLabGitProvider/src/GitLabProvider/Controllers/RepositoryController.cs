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

        public override IActionResult CreateBranch([FromRoute(Name = "repositoryId"), Required] string repositoryId, [FromBody] CreateBranchRequest? createBranchRequest)
        {
            if (_gitLabClient.CreateBranch(createBranchRequest.Name, repositoryId).Result)
            {
                //TODO Fix response
                return Ok(new BranchCreatedResponse() { Id = "TODO Fix", Name = "TODO Fix" });
            }
            return BadRequest(new Problem() { Type = "404", Title = "Could not create branch" });
        }


        public override IActionResult CreateRepository([FromBody] CreateRepositoryRequest? createRepositoryRequest)
        {
            //TODO default branch name 
            var repoInfo = _gitLabClient.CreateProjectRepo(createRepositoryRequest.Name).Result;
            if (repoInfo != null)
            {
                //TODO fix response
                return Ok(new RepositoryCreatedResponse() { /*Name = repoInfo.Name, PreferredName = preferredName, Path = repoInfo.Url, Id = repoInfo.Id */});
            }
            return BadRequest(new Problem() { Type = "404", Title = "Could not create repository" });
        }
    }
}
