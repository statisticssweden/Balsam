﻿using Balsam.Api.Models;
using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using LibGit2Sharp;
using GitProviderApiClient.Api;

namespace Balsam.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : BalsamApi.Server.Controllers.ProjectApiController
    {
        private readonly HubClient _hubClient;
        private readonly ILogger<ProjectController> _logger;
        private readonly KnowledgeLibraryClient _knowledgeLibraryClient;
        private readonly IRepositoryApi _repositoryApi;


        public ProjectController(IOptionsSnapshot<CapabilityOptions> capabilityOptions, ILogger<ProjectController> logger, HubClient hubClient, KnowledgeLibraryClient knowledgeLibraryClient, IRepositoryApi reposiotryApi)
        {
            _hubClient = hubClient;
            _logger = logger;
            capabilityOptions.Get(Capabilities.Git);
            capabilityOptions.Get(Capabilities.Authentication);
            _knowledgeLibraryClient = knowledgeLibraryClient;
            _repositoryApi = reposiotryApi;
        }

        public async override Task<IActionResult> CreateBranch([FromRoute(Name = "projectId"), Required] string projectId, [FromBody] CreateBranchRequest? createBranchRequest)
        {
            if (createBranchRequest is null)
            {
                return BadRequest(new Problem() { Status = 400, Title = "Parameter error", Detail = "Missing parameters" });
            }

            BranchCreatedResponse branchCreatedResponse;
            try
            {
                var branch = await _hubClient.CreateBranch(projectId, createBranchRequest.FromBranch, createBranchRequest.Name, createBranchRequest.Description);
                if (branch == null)
                {
                    return BadRequest(new Problem() { Status = 400, Title = "Could not create branch", Detail = "Branch could not be created" });
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
                return BadRequest(new Problem() { Status = 400, Title = "Could not create branch", Detail = "Branch could not be created" });
            }

            return Ok(branchCreatedResponse);
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
                BalsamProject? project = await _hubClient.CreateProject(createProjectRequest.Name, createProjectRequest.Description, createProjectRequest.BranchName, username, createProjectRequest.SourceLocation);

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
                if (files is null)
                {
                    return BadRequest(new Problem() { Status = 400, Type = "Fetch problem", Title = "Could not fetch files for repository branch" });
                }
                return Ok(files.Select(f => new BalsamApi.Server.Models.RepoFile()
                {
                    Name = f.Name,
                    Path = f.Path,
                    Type = f.Type == GitProviderApiClient.Model.RepoFile.TypeEnum.File ? BalsamApi.Server.Models.RepoFile.TypeEnum.FileEnum : BalsamApi.Server.Models.RepoFile.TypeEnum.FolderEnum,
                    ContentUrl = f.ContentUrl,
                    Id = f.Id
                }).ToArray());

            }
            catch (Exception ex)
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
                evt.Branches = balsamProject.Branches.Select(b => new BalsamApi.Server.Models.Branch() { Id = b.Id, Description = b.Description, Name = b.Name, IsDefault = b.IsDefault }).ToList();
                evt.AuthGroup = balsamProject.Oidc.GroupName;

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
                Branches = MapBranches(project.Branches),
                AuthGroup = project.Oidc.GroupName,
                GitUrl = project.Git?.Path
            }).OrderBy(p => p.Name).ToList();
        }

        private List<BalsamApi.Server.Models.Branch> MapBranches(List<BalsamBranch> branches)
        {
            return branches.Select(branch => new BalsamApi.Server.Models.Branch()
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

        public async override Task<IActionResult> DeleteBranch([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId)
        {
            try
            {
                var project = await _hubClient.GetProject(projectId);
                var branch = await _hubClient.GetBranch(projectId, branchId);

                if (project is null || branch is null)
                {
                    return BadRequest(new Problem() { Status = 404, Type = "Project/branch not found", Detail = "Can not find the project/branch" });

                }
                else if (User.Claims.FirstOrDefault(x => x.Type == "groups" && x.Value == project.Oidc?.GroupName) is null)
                {
                    return Unauthorized(new Problem() { Status = 401, Type = "Unauthorized", Detail = "User is not authorized to delete the branch" });
                }

                await _hubClient.DeleteBranch(projectId, branchId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete branch");
                return BadRequest(new Problem() { Status = 400, Type = "Could not delete branch", Detail = "Could not delete branch, internal error" });
            }

            return Ok();
        }

        public async override Task<IActionResult> DeleteProject([FromRoute(Name = "projectId"), Required] string projectId)
        {
            try
            {
                var project = await _hubClient.GetProject(projectId);

                if (project is null )
                {
                    return BadRequest(new Problem() { Status = 404, Type = "Project not found", Detail = "Can not find the project" });
                    
                } else if (User.Claims.FirstOrDefault(x => x.Type == "groups" && x.Value == project.Oidc?.GroupName) is null)
                {
                    return Unauthorized(new Problem() { Status = 401, Type = "Unauthorized", Detail = "User is not authorized to delete the project" });
                }

                await _hubClient.DeleteProject(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete project");
                return BadRequest(new Problem() { Status = 400, Type = "Could not delete project", Detail = "Could not delete project, internal error" });
            }

            return Ok();
        }

        public async override Task<IActionResult> CopyFromKnowleadgeLibrary([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId, [FromQuery(Name = "libraryId"), Required] string libraryId, [FromQuery(Name = "fileId"), Required] string fileId)
        {
            try
            {
                var project = await _hubClient.GetProject(projectId);
                var branch = await _hubClient.GetBranch(projectId, branchId);
                if (project is null || branch is null)
                {
                    return BadRequest(new Problem() { Status = 404, Type = "Project/branch not found", Detail = "Can not find the project/branch" });
                }

                var knowledgeLibrary = (await _hubClient.ListKnowledgeLibraries()).FirstOrDefault(kb => kb.Id == libraryId);

                if (knowledgeLibrary is null)
                {
                    return BadRequest(new Problem() { Status = 404, Title = "Knowledge library not found", Detail = "Knowledge library not found" });
                }

                var zipFile = _knowledgeLibraryClient.GetZippedResource(libraryId, knowledgeLibrary.RepositoryUrl, fileId);
            
                var stream = System.IO.File.OpenRead(zipFile);
                await _repositoryApi.AddResourceFilesAsync(project.Git?.Id??"", branch.GitBranch, stream);
                stream.Close();
                System.IO.File.Delete(zipFile);
                
                _logger.LogInformation("File/directory copied from knowledge library");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not copy file/directory");
                return BadRequest(new Problem() { Status = 400, Title = "Could not copy file/directory", Detail = "Could not copy file/directory" });
            }
        }
    }
}
