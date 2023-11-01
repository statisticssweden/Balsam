﻿using GitLabProvider.Client;
using GitLabProvider.Configuration;
using GitProvider.Controllers;
using GitProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace GitLabProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryController : RepositoryApiController
    {
        private readonly IGitLabClient _gitLabClient;
        private readonly string _baseUrl;
        private readonly ILogger<RepositoryController> _logger;

        public RepositoryController(IOptions<ApiOptions> options, IGitLabClient gitLabClient, ILogger<RepositoryController> logger)
        {
            _gitLabClient = gitLabClient;
            _logger = logger;
            _baseUrl = options.Value.BaseUrl;
        }

        public async override Task<IActionResult> CreateBranch([FromRoute(Name = "repositoryId"), Required] string repositoryId, [FromBody] CreateBranchRequest? createBranchRequest)
        {
            if (createBranchRequest is null)
            {
                return BadRequest(new Problem() { Type = "404", Title = "Parameter erros", Detail = "Missing parameters" });
            }

            var branchName = SantitazeBranchName(createBranchRequest.Name);
            var fromBranch = createBranchRequest.FromBranch;

            if (await _gitLabClient.CreateBranch(repositoryId, fromBranch, branchName))
            {
                return Ok(new BranchCreatedResponse() { Id = branchName, Name = branchName });
            }
            return BadRequest(new Problem() { Type = "404", Title = "Could not create branch" });
        }


        public async override Task<IActionResult> CreateRepository([FromBody] CreateRepositoryRequest? createRepositoryRequest)
        {
            //TODO return branchname
            if (!(createRepositoryRequest is null)) { 
                var repoInfo =  await _gitLabClient.CreateProjectRepo(createRepositoryRequest.Name, createRepositoryRequest.Description, SantitazeBranchName(createRepositoryRequest.DefaultBranchName));
                if (repoInfo != null)
                {
                    return Ok(new RepositoryCreatedResponse() { Name = repoInfo.Name, PreferredName = createRepositoryRequest.Name, Path = repoInfo.Url, Id = repoInfo.Id });
                }
            }
            return BadRequest(new Problem() { Type = "404", Title = "Could not create repository" });
        }

        public async override Task<IActionResult> GetFilesInBranch([FromRoute(Name = "repositoryId"), Required] string repositoryId, [FromRoute(Name = "branchId"), Required] string branchId)
        {
            List<GitLabTreeFile> files;
            try
            {
                files = await _gitLabClient.GetFiles(repositoryId, branchId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cloud not fetch file list");
                return BadRequest(new Problem { Type = "Could not list files", Detail = "Could not fetch file list" });

            }

            var filesResponse = files.Select(f => new GitProvider.Models.File()
            {
                Name = f.name,
                Path = f.path,
                Type = string.Compare(f.type, "blob", true) == 0 ? GitProvider.Models.File.TypeEnum.FileEnum : GitProvider.Models.File.TypeEnum.FolderEnum,
                ContentUrl = $"{_baseUrl}/api/v4/projects/{repositoryId}/repository/files/{Uri.EscapeDataString(f.path)}/raw?ref={branchId}"
            }); 

            return Ok(filesResponse.ToArray());
        }

        private static string SantitazeBranchName(string branchName)
        {
            var name = branchName.Trim(); //Remove whitespaces before and after valid charaters
            name = Regex.Replace(branchName, @"\s+", "-"); // replaces whitespace with hypen 

            return name;
        }
    }
}
