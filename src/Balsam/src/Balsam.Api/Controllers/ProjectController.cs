using Balsam.Api.Models;
using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Balsam.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : BalsamApi.Server.Controllers.ProjectApiController
    {
        private readonly HubClient _hubClient;
        private readonly ILogger<ProjectController> _logger;


        public ProjectController(IOptionsSnapshot<CapabilityOptions> capabilityOptions, ILogger<ProjectController> logger, HubClient hubClient)
        {
            _hubClient = hubClient;
            _logger = logger;
            capabilityOptions.Get(Capabilities.Git);
            capabilityOptions.Get(Capabilities.Authentication);
        }

        public override Task<IActionResult> CreateBranch([FromRoute(Name = "projectId"), Required] string projectId, [FromBody] CreateBranchRequest? createBranchRequest)
        {
            throw new NotImplementedException();
        }


        public async override Task<IActionResult> CreateProject([FromBody] CreateProjectRequest? createProjectRequest)
        {
            if (createProjectRequest is null)
            {
                return BadRequest(new Problem() { Title = "Parameters missing", Status = 400, Type = "Missing parameters" });
            }
            _logger.LogInformation("Reading user information");
            var username = this.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            _logger.LogInformation($"The user is {username}");
            try
            {
                BalsamProject? project = await _hubClient.CreateProject(createProjectRequest.Name, createProjectRequest.Description, createProjectRequest.BranchName, username);

                if (project == null)
                {
                    return BadRequest(new Problem() { Title = "Project with that name already exists", Status = 400, Type = "Project duplication" });
                }

                var evt = new ProjectCreatedResponse();
                evt.Id = project.Id;
                evt.Name = project.Name;

                return Ok(evt);

            }
            catch (Exception ex)
            {
                _logger.LogError("Could not create project", ex);
                return BadRequest(new Problem() { Title = "Internal error", Status = 400, Type = "Internal error" });
            }
        }

        public async override Task<IActionResult> GetFiles([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId)
        {
            try
            {
                var files = await _hubClient.GetGitBranchFiles(projectId, branchId);
                if (files is null )
                {
                    return BadRequest(new Problem() { Status = 400, Type = "Fetch problem", Title = "Could not fetch files for repository branch" });
                }
                return Ok(files.Select(f => new BalsamApi.Server.Models.File() { Name  = f.Name, Path = f.Path, Type = f.Type == GitProviderApiClient.Model.File.TypeEnum.File?BalsamApi.Server.Models.File.TypeEnum.FileEnum: BalsamApi.Server.Models.File.TypeEnum.FolderEnum, ContentUrl = f.ContentUrl }).ToArray());

            } catch (Exception ex)
            {
                _logger.LogError(ex, "Could not fetch files");
            }
            return BadRequest(new Problem() { Status = 400, Type = "Fetch problem", Title = "Could not fetch files for repository branch" });
        }


        public async override Task<IActionResult> GetProject([FromRoute(Name = "projectId"), Required] string projectId)
        {
            try
            {
                var balsamProject = await _hubClient.GetProject(projectId);

                if (balsamProject is null)
                {
                    return BadRequest(new Problem() { Title = "Project with given id can not be found", Status = 400, Type = "Can not find project" });
                }

                var evt = new ProjectResponse();
                evt.Id = balsamProject.Id;
                evt.Name = balsamProject.Name;
                evt.Description = balsamProject.Description;
                evt.GitUrl = balsamProject.Git is null ? "" : balsamProject.Git.Path;
                evt.Branches = balsamProject.Branches.Select(b => new Branch() { Id = b.Id, Description = b.Description, Name = b.Name, IsDefault = b.IsDefault }).ToList();

                return Ok(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError("", ex);
                return BadRequest(new Problem() { Title = "Project can not be loaded", Status = 400, Type = "Can not load project" });
            }
        }


        public override async Task<IActionResult> ListProjects([FromQuery(Name = "all")] bool? all)
        {
            var listAll = all ?? true;
            _logger.LogDebug($"Hit ListProject viewAll={listAll}", listAll);

            var projects = await _hubClient.GetProjects();

            if (!listAll)
            {
                var userGroups = User.Claims.Where(x => x.Type == "groups");
                projects = projects.Where(x => userGroups.Any(o => string.Equals(o.Value, x.Oidc?.GroupName, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            var projectListResponse = new ProjectListResponse
            {
                Projects = MapProject(projects)
            };

            return Ok(projectListResponse);
        }

        private List<Project> MapProject(List<BalsamProject> projects)
        {
            return projects.Select(project => new Project()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Branches = MapBranches(project.Branches)
            }).ToList();
        }

        private List<Branch> MapBranches(List<BalsamBranch> branches)
        {
            return branches.Select(branch => new Branch()
            {
                Id = branch.Id,
                Description = branch.Description,
                Name = branch.Name,
                IsDefault = branch.IsDefault
            }).ToList();
        }

        public async override Task<IActionResult> GetFile([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId, [FromRoute(Name = "fileId"), Required] string fileId)
        {
            var file = await _hubClient.GetFile(projectId, branchId, fileId);

            if (file != null)
            {
                Response.Headers.Add("content-disposition", "inline");
                return file;
            }

            return BadRequest(new Problem() { Status = 404, Type = "file not found", Detail = "Can not find the file" });

        }
    }
}
