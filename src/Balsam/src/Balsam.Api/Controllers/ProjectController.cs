using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using GitProviderApiClient.Api;
using Balsam.Model;
using Balsam.Interfaces;
using Balsam.Application.Authorization;
using System.Net;
using Balsam.Api.Extensions;

namespace Balsam.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : BalsamApi.Server.Controllers.ProjectApiController
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;
        private readonly IKnowledgeLibraryService _knowledgeLibraryService;
        private readonly IRepositoryApi _repositoryApi;
        private readonly ProjectAuthorization _projectAuthorization;

        public ProjectController(IOptionsSnapshot<CapabilityOptions> capabilityOptions,
                                    ILogger<ProjectController> logger,
                                    IProjectService projectService,
                                    IKnowledgeLibraryService knowledgeLibraryService,
                                    IRepositoryApi reposiotryApi)
        {
            _projectService = projectService;
            _logger = logger;
            capabilityOptions.Get(Capabilities.Git);
            capabilityOptions.Get(Capabilities.Authentication);
            _knowledgeLibraryService = knowledgeLibraryService;
            _repositoryApi = reposiotryApi;
            _projectAuthorization = new ProjectAuthorization(); //TODO: Use interface
        }

        public async override Task<IActionResult> CreateBranch([FromRoute(Name = "projectId"), Required] string projectId, [FromBody] CreateBranchRequest? createBranchRequest)
        {
            if (createBranchRequest is null)
            {
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.BadRequest, Title = "Parameter error", Detail = "Missing parameters" });
            }

            BranchCreatedResponse branchCreatedResponse;
            try
            {
                var project = await _projectService.GetProject(projectId);

                if (!_projectAuthorization.CanUserCreateBranch(this.User, project))
                {
                    return Unauthorized(new Problem() { Status = (int)HttpStatusCode.Unauthorized, Type = "Unauthorized", Title = "User cannot create branch" });
                }

                var branch = await _projectService.CreateBranch(projectId, createBranchRequest.FromBranch, createBranchRequest.Name, createBranchRequest.Description ?? "");
                if (branch == null)
                {
                    return BadRequest(new Problem() { Status = (int)HttpStatusCode.BadRequest, Title = "Could not create branch", Detail = "Branch could not be created" });
                }

                branchCreatedResponse = new BranchCreatedResponse
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    ProjectId = projectId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create branch");
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.InternalServerError, Title = "Could not create branch", Detail = "Branch could not be created" });
            }

            return Ok(branchCreatedResponse);
        }

        public async override Task<IActionResult> CreateProject([FromBody] CreateProjectRequest? createProjectRequest)
        {
            if (createProjectRequest is null)
            {
                return BadRequest(new Problem() { Title = "Parameters missing", Status = (int)HttpStatusCode.BadRequest, Type = "Missing parameters" });
            }
            _logger.LogInformation("Reading user information");
            var username = this.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            //_logger.LogInformation($"The user is {username}");
            try
            {
                BalsamProject? project = await _projectService.CreateProject(createProjectRequest.Name, createProjectRequest.Description, createProjectRequest.BranchName, username, createProjectRequest.SourceLocation);

                if (project == null)
                {
                    return BadRequest(new Problem() { Title = "Project with that name already exists", Status = (int)HttpStatusCode.BadRequest, Type = "Project duplication" });
                }

                var evt = new ProjectCreatedResponse();
                evt.Id = project.Id;
                evt.Name = project.Name;

                return Ok(evt);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create project");
                return BadRequest(new Problem() { Title = "Internal error", Status = (int)HttpStatusCode.InternalServerError, Type = "Internal error" });
            }
        }

        public async override Task<IActionResult> GetFiles([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId)
        {
            try
            {
                var files = await _projectService.GetGitBranchFiles(projectId, branchId);

                return Ok(files.Select(f => f.ToRepoFile()).ToArray());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not fetch files");
            }
            return BadRequest(new Problem() { Status = (int)HttpStatusCode.InternalServerError, Type = "Fetch problem", Title = "Could not fetch files for repository branch" });
        }

        public async override Task<IActionResult> GetProject([FromRoute(Name = "projectId"), Required] string projectId)
        {
            try
            {
                var project = await _projectService.GetProject(projectId);

                ProjectResponse response = project.ToProjectResponse();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get project");
                return BadRequest(new Problem() { Title = "Project can not be loaded", Status = (int)HttpStatusCode.BadRequest, Type = "Can not load project" });
            }
        }

        public override async Task<IActionResult> ListProjects([FromQuery(Name = "all")] bool? all)
        {
            var listAll = all ?? true;
            _logger.LogDebug($"Hit ListProject viewAll={listAll}", listAll);

            var projects = await _projectService.GetProjects();

            if (!listAll)
            {
                var userGroups = User.Claims.Where(x => x.Type == "groups");
                projects = projects.Where(x => userGroups.Any(o => string.Equals(o.Value, x.Oidc?.GroupName, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            var projectListResponse = new ProjectListResponse
            {
                Projects = projects.Select(project => project.ToProject())
                            .OrderBy(p => p.Name)
                            .ToList()
            };

            return Ok(projectListResponse);
        }


        public async override Task<IActionResult> GetFile([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId, [FromRoute(Name = "fileId"), Required] string fileId)
        {
            try
            {
                //TODO: Authorize get content?
                
                var fileContent = await _projectService.GetFile(projectId, branchId, fileId);

                if (fileContent.Content != null)
                {
                    Response.Headers.Add("content-disposition", "inline");

                    var fileContentResult = new FileContentResult(fileContent.Content, fileContent.Mediatype);
                    return fileContentResult;
                }
                else
                {
                    var errorMessage = "Could not read file content";
                    _logger.LogError(errorMessage);
                    return NotFound(new Problem() { Status = (int)HttpStatusCode.NotFound, Type = "File read error", Detail = errorMessage });
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Could not get file";
                _logger.LogError(ex, errorMessage);
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.InternalServerError, Type = "Get file error", Detail = errorMessage });

            }
        }

        public async override Task<IActionResult> DeleteBranch([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId)
        {
            try
            {
                var project = await _projectService.GetProject(projectId);
                var branch = await _projectService.GetBranch(projectId, branchId);

                if (project is null || branch is null)
                {
                    return NotFound(new Problem() { Status = (int)HttpStatusCode.NotFound, Type = "Project/branch not found", Detail = "Can not find the project/branch" });

                }
                else if (!_projectAuthorization.CanUserDeleteBranch(this.User, project))
                {
                    return Unauthorized(new Problem() { Status = (int)HttpStatusCode.Unauthorized, Type = "Unauthorized", Detail = "User is not authorized to delete the branch" });
                }

                await _projectService.DeleteBranch(projectId, branchId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete branch");
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.InternalServerError, Type = "Could not delete branch", Detail = "Could not delete branch, internal error" });
            }

            return Ok();
        }

        public async override Task<IActionResult> DeleteProject([FromRoute(Name = "projectId"), Required] string projectId)
        {
            try
            {
                var project = await _projectService.GetProject(projectId);

                if (project is null)
                {
                    return NotFound(new Problem() { Status = (int)HttpStatusCode.NotFound, Type = "Project not found", Detail = "Can not find the project" });

                }
                else if (!_projectAuthorization.CanUserDeleteProject(this.User, project))
                {
                    return Unauthorized(new Problem() { Status = (int)HttpStatusCode.Unauthorized, Type = "Unauthorized", Detail = "User is not authorized to delete the project" });
                }

                await _projectService.DeleteProject(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete project");
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.InternalServerError, Type = "Could not delete project", Detail = "Could not delete project, internal error" });
            }

            return Ok();
        }

        public async override Task<IActionResult> CopyFromKnowleadgeLibrary([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId, [FromQuery(Name = "libraryId"), Required] string libraryId, [FromQuery(Name = "fileId"), Required] string fileId)
        {
            try
            {
                var project = await _projectService.GetProject(projectId);

                if (project is null)
                {
                    return NotFound(new Problem() { Status = (int)HttpStatusCode.NotFound, Type = "Project not found", Detail = "Can not find the project" });
                }

                if (!_projectAuthorization.CanUserEditBranch(this.User, project))
                {
                    return Unauthorized(new Problem() { Status = (int)HttpStatusCode.Unauthorized, Type = "Unauthorized", Detail = "User is not authorized to edit project" });
                }

                var knowledgeLibrary = _knowledgeLibraryService.GetKnowledgeLibrary(libraryId);

                if (knowledgeLibrary is null)
                {
                    return NotFound(new Problem() { Status = (int)HttpStatusCode.NotFound, Title = "Knowledge library not found", Detail = "Knowledge library not found" });
                }

                await _projectService.CopyFromKnowledgeLibrary(project.Id, branchId, libraryId, fileId);

                _logger.LogInformation("File/directory copied from knowledge library");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not copy file/directory");
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.InternalServerError, Title = "Could not copy file/directory", Detail = "Could not copy file/directory" });
            }
        }
    }
}
