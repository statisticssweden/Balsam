﻿using Balsam.Api.Extensions;
using Balsam.Application.Authorization;
using Balsam.Interfaces;
using Balsam.Model;
using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Balsam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspaceController : BalsamApi.Server.Controllers.WorkspaceApiController
    {
        private readonly ILogger<WorkspaceController> _logger;
        private readonly IProjectService _projectService;
        private readonly IWorkspaceService _workspaceService;
        private readonly WorkspaceAuthorization _workspaceAuthorization;


        public WorkspaceController(ILogger<WorkspaceController> logger, IProjectService projectService, IWorkspaceService workspaceService)
        {
            _logger = logger;
            _projectService = projectService;
            _workspaceService = workspaceService;
            _workspaceAuthorization = new WorkspaceAuthorization(); //TODO: Use interface
        }

        public async override Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest? createWorkspaceRequest)
        {
            if (createWorkspaceRequest is null)
            {
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.BadRequest, Title = "Missing parameters", Detail = "Missing input parameter(s)" });
            }

            try
            {
                var project = await _projectService.GetProject(createWorkspaceRequest.ProjectId);

                var username = this.User.GetUserName();
                var mail = this.User.GetEmail();

                if (!_workspaceAuthorization.CanUserCreateWorkspace(this.User, project))
                {
                    _logger.LogInformation($"User {username} not authorized to crate workspace in project {project.Id}.");
                    return Unauthorized(new Problem() { Status = (int)HttpStatusCode.Unauthorized, Type = "Unauthorized", Title = "User cannot create workspace" });
                }

                var workspace = await _workspaceService.CreateWorkspace(createWorkspaceRequest.ProjectId, createWorkspaceRequest.BranchId, createWorkspaceRequest.Name, createWorkspaceRequest.TemplateId, username, mail);

                if (workspace == null)
                {
                    return BadRequest(new Problem() { Status = (int)HttpStatusCode.BadRequest, Type = "Could not create workspace", Title = "Could not create workspace due to error" });
                }

                return Ok(new WorkspaceCreatedResponse() { Id = workspace.Id, Name = workspace.Name, ProjectId = createWorkspaceRequest.ProjectId, BranchId = createWorkspaceRequest.BranchId, Url = workspace.Url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create workspace");
            }

            return BadRequest(new Problem() { Status = (int)HttpStatusCode.InternalServerError, Type = "Could not create workspace", Title = "Internal error when created error" });
        }


        public async override Task<IActionResult> DeleteWorkspace([FromRoute(Name = "workspaceId"), Required] string workspaceId, [FromQuery(Name = "projectId")] string? projectId, [FromQuery(Name = "branchId")] string? branchId)
        {
            if (workspaceId is null)
            {
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.BadRequest, Title = "Missing parameters", Detail = "Missing input parameter(s)" });
            }

            try
            {
                var userName = this.User.GetUserName();
                var project = await _projectService.GetProject(projectId);
                var workspace = await _workspaceService.GetWorkspace(projectId, userName, workspaceId);

                if (!_workspaceAuthorization.CanUserDeleteWorkspace(this.User, project, workspace))
                {
                    _logger.LogInformation($"User {userName} not authorized to delete workspace {workspace.Id}.");
                    return Unauthorized(new Problem() { Status = ((int)HttpStatusCode.Unauthorized), Type = "Authentication error", Title = "User cannot delete workspace" });
                }

                await _workspaceService.DeleteWorkspace(projectId, branchId, workspaceId, userName);

                return Ok($"workspace {workspaceId} deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete workspace");
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.BadRequest, Type = "Could not delete workspace", Title = "Could not delete workspace due to error" });
            }
        }


        public async override Task<IActionResult> ListTemplates()
        {
            var workspaceTemplates = await _workspaceService.ListWorkspaceTemplates();
            var templates = workspaceTemplates
                .Select(x => x.ToTemplate());
            
            if (templates.Any())
            {
                return await Task.FromResult<IActionResult>(Ok(templates));
            }

            return BadRequest(new Problem
            {
                Detail = "No workspace templates found in hub repository",
                Status = (int)HttpStatusCode.ExpectationFailed,
                Title = "No workspace templates found"
            });
        }

        public async override Task<IActionResult> ListWorkspaces([FromQuery(Name = "projectId")] string? projectId, [FromQuery(Name = "branchId")] string? branchId, [FromQuery(Name = "all")] bool? all)
        {
            //TODO: Rename and inverse "all" to "ownedByUser" 

            if (branchId != null && projectId is null)
            {
                return BadRequest(new Problem() { Status = (int)HttpStatusCode.BadRequest, Type = "Parameter error", Title = "If BranchId is specified a ProjectId must also be specified" });
            }

            List<BalsamWorkspace> workspaces;
            if (projectId != null)
            {
                var username = this.User.GetUserName();
                var project = await _projectService.GetProject(projectId);

                if (!_workspaceAuthorization.CanUserGetWorkspaces(this.User, project))
                {
                    //TODO: Return empty list?
                    _logger.LogInformation($"User {username} not authorized to get workspaces for project {project.Id}.");
                    return Unauthorized(new Problem() { Status = (int)HttpStatusCode.Unauthorized, Type = "Not authorized", Title = "You are not a member of the project" });
                }

                if (all ?? true)
                {
                    if (branchId is null)
                    {
                        workspaces = await _workspaceService.GetWorkspacesByProject(projectId);
                    }
                    else
                    {
                        workspaces = await _workspaceService.GetWorkspacesByProjectAndBranch(projectId, branchId);
                    }
                }
                else
                {
                    if (branchId is null)
                    {
                        workspaces = await _workspaceService.GetWorkspacesByProjectAndUser(projectId, username);
                    }
                    else
                    {
                        workspaces = await _workspaceService.GetWorkspacesByProjectBranchAndUser(projectId, branchId, username);
                    }
                }
            }
            else
            {
                if (all ?? true)
                {
                    workspaces = await _workspaceService.GetWorkspaces();
                }
                else
                {
                    var projects = await _projectService.GetProjects(false);
                    var groups = this.User.GetGroups().Select(g => g.Value);
                    var userId = this.User.GetUserName();
                    var projectIds = projects.Where(p => groups.Contains(p.Oidc.GroupName)).Select(p => p.Id).ToList();
                    workspaces = await _workspaceService.GetWorkspacesByUser(userId, projectIds);
                }
            }

            return base.Ok(workspaces.Select(w => w.ToWorkspace())
                            .OrderBy(w => w.Name).ToArray());
        }

    }
}
