using Balsam.Api.Models;
using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Balsam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspaceController : BalsamApi.Server.Controllers.WorkspaceApiController
    {
        private readonly ILogger<WorkspaceController> _logger;
        private readonly HubClient _hubClient;

        public WorkspaceController(ILogger<WorkspaceController> logger, HubClient hubClient)
        {
            _logger = logger;
            _hubClient = hubClient;
        }

        public async override Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest? createWorkspaceRequest)
        {
            if (createWorkspaceRequest is null)
            {
                return BadRequest(new Problem() { Status = 400, Title = "Missing parameters", Detail = "Missing input parameter(s)" });
            }
            var username = this.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            var mail = this.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            try
            {
                var workspace = await _hubClient.CreateWorkspace(createWorkspaceRequest.ProjectId, createWorkspaceRequest.BranchId, createWorkspaceRequest.Name, createWorkspaceRequest.TemplateId, username, mail);

                if (workspace == null)
                {
                    return BadRequest(new Problem() { Status = 400, Type = "Could not create workspace", Title = "Could not create workspace due to error" });
                }

                return Ok(new WorkspaceCreatedResponse() { Id = workspace.Id, Name = workspace.Name, ProjectId = createWorkspaceRequest.ProjectId, BranchId = createWorkspaceRequest.BranchId});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create workspace");
            }
            return BadRequest(new Problem() { Status = 400, Type = "Could not create workspace", Title = "Internal error when created error" });
           
        }


        public async override Task<IActionResult> DeleteWorkspace([FromRoute(Name = "workspaceId"), Required] string workspaceId, [FromQuery(Name = "projectId")] string? projectId, [FromQuery(Name = "branchId")] string? branchId)
        {
            if (workspaceId is null)
            {
                return BadRequest(new Problem() { Status = 400, Title = "Missing parameters", Detail = "Missing input parameter(s)" });
            }
            var userName = this.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            try
            {
                var workspace = await _hubClient.DeleteWorkspace(projectId, branchId, workspaceId, userName);

                if (workspace == null)
                {
                    return BadRequest(new Problem() { Status = 400, Type = "Could not delete workspace", Title = "Could not create workspace due to error" });
                }

                return Ok($"workspace {workspaceId} deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete workspace");
            }
            return BadRequest(new Problem() { Status = 400, Type = "Could not delete workspace", Title = "Internal error when created error" });

        }


        public override Task<IActionResult> ListTemplates()
        {
            var workspaceTemplates = _hubClient.ListWorkspaceTemplates();
            var templates = workspaceTemplates
                .Select(x => new Template { Id = x.Id, Name = x.Name, Description = x.Description });
            if (templates.Any())
            {
                return Task.FromResult<IActionResult>(Ok(templates));
            }

            return Task.FromResult<IActionResult>(BadRequest(new Problem
            {
                Detail = "No workspace templates found in hub repository",
                Status = 417,
                Title = "No workspace template found"
            }));
        }

        public async override Task<IActionResult> ListWorkspaces([FromQuery(Name = "projectId")] string? projectId, [FromQuery(Name = "branchId")] string? branchId, [FromQuery(Name = "all")] bool? all)
        {
            if (branchId != null && projectId is null)
            {
                return BadRequest(new Problem() { Status = 400, Type = "Parameter error", Title = "If BranchId is specified a ProjectId must also be specified"});
            }
            List<BalsamWorkspace> workspaces;
            if (projectId != null)
            {
                if (all??true)
                {
                    if (branchId is null)
                    {
                        workspaces = await _hubClient.GetWorkspacesByProject(projectId);
                    }
                    else
                    {
                        workspaces = await _hubClient.GetWorkspacesByProjectAndBranch(projectId, branchId);
                    }
                }
                else
                {
                    var project = await _hubClient.GetProject(projectId);

                    if (this.User.Claims.FirstOrDefault(c => c.Type == "groups" && c.Value == project.Oidc.GroupId) is null)
                    {
                        return BadRequest(new Problem() { Status = 400, Type = "Not authorized", Title = "You are not a member of the project" });
                    }

                    if (branchId is null)
                    {
                        workspaces = await _hubClient.GetWorkspacesByProjectAndUser(projectId, this.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value);
                    }
                    else
                    {
                        workspaces = await _hubClient.GetWorkspacesByProjectBranchAndUser(projectId, branchId, this.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value);
                    }
                }
            }
            else
            {
                if (all ?? true)
                {
                    workspaces = await _hubClient.GetWorkspaces();
                }
                else
                {
                    var projects = await _hubClient.GetProjects(false);
                    var groups = this.User.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList();
                    var projectIds = projects.Where(p => groups.Contains(p.Oidc.GroupName)).Select(p => p.Id).ToList();
                    workspaces = await _hubClient.GetWorkspacesByUser(this.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value, projectIds);
                }
            }
            return Ok(workspaces.Select(w => new Workspace() { Id = w.Id, Name = w.Name, ProjectId = w.ProjectId, BranchId = w.BranchId, TemplateId = w.TemplateId, Url = w.Url}).ToArray());
        }
    }
}
