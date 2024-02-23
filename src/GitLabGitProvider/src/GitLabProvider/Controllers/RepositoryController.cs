using GitLabProvider.Client;
using GitLabProvider.Configuration;
using GitProvider.Controllers;
using GitProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
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
            if (!(createRepositoryRequest is null)) {
                var branchName = SantitazeBranchName(createRepositoryRequest.DefaultBranchName);
                var repoInfo =  await _gitLabClient.CreateProjectRepo(createRepositoryRequest.Name, createRepositoryRequest.Description, branchName);
                if (repoInfo != null)
                {
                    return Ok(new RepositoryCreatedResponse() { Name = repoInfo.Name, PreferredName = createRepositoryRequest.Name, Path = repoInfo.Url, Id = repoInfo.Id, DefaultBranchName = branchName });
                }
            }
            return BadRequest(new Problem() { Type = "404", Title = "Could not create repository" });
        }

        public async override Task<IActionResult> GetFile([FromRoute(Name = "repositoryId"), Required] string repositoryId, [FromRoute(Name = "branchId"), Required] string branchId, [FromRoute(Name = "fileId"), Required] string fileId)
        {
            var file = await _gitLabClient.GetFile(repositoryId, branchId, fileId);

            if (file != null)
            {
                Response.Headers.Add("content-disposition", "inline");
                return file;
            }

            return BadRequest(new Problem() { Status = 404, Type = "file not found", Detail = "Can not find the file" });

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

            var filesResponse = files.Select(f => new GitProvider.Models.RepoFile()
            {
                Id = f.id,
                Name = f.name,
                Path = f.path,
                Type = string.Compare(f.type, "blob", true) == 0 ? GitProvider.Models.RepoFile.TypeEnum.FileEnum : GitProvider.Models.RepoFile.TypeEnum.FolderEnum,
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

        public async override Task<IActionResult> DeleteRepository([FromRoute(Name = "repositoryId"), Required] string repositoryId)
        {
            try
            {
                bool deleted = await _gitLabClient.DeleteRepository(repositoryId);
                if (deleted)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete repository");
            }

            return BadRequest(new Problem { Type = "Could not delete repository", Detail = "Could not delete repository internal error" });
        }

        public override async Task<IActionResult> DeleteRepositoryBranch([FromRoute(Name = "repositoryId"), Required] string repositoryId, [FromRoute(Name = "branchId"), Required] string branchId)
        {
            try
            {
                bool deleted = await _gitLabClient.DeleteBranch(repositoryId, branchId);
                if (deleted)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete repository branch");
            }

            return BadRequest(new Problem { Type = "Could not delete repository brach ", Detail = "Could not delete repository branch internal error" });
        }

        public async override Task<IActionResult> AddResourceFiles([FromRoute(Name = "repositoryId"), Required] string repositoryId, [FromRoute(Name = "branchId"), Required] string branchId, [FromBody] Stream? body)
        {

            if (body is null)
            {
                return BadRequest(new Problem { Type = "Could not add resource files", Detail = "Could not add resource files internal error" });
            }

            string zipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

            try
            {
                using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write))
                {
                    await body.CopyToAsync(fileStream);
                }

                string workPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                ZipFile.ExtractToDirectory(zipPath, workPath);

                await _gitLabClient.AddResourceFiles(repositoryId, branchId, workPath);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not add resource files");
                return BadRequest(new Problem { Type = "Could not add resource files", Detail = "Could not add resource files internal error" });
            }
            finally
            {
                if (System.IO.File.Exists(zipPath))
                {
                    System.IO.File.Delete(zipPath);
                }
                //TODO: Delete workPath directory
            }

            return Ok();
        }
    }
}
