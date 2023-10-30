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

        public override Task<IActionResult> DeleteWorkspace([FromRoute(Name = "workspaceId"), Required] string workspaceId)
        {
            throw new NotImplementedException();
        }

        public override Task<IActionResult> GetWorkspace([FromQuery(Name = "projectId")] string? projectId, [FromQuery(Name = "branchId")] string? branchId, [FromQuery(Name = "all")] bool? all)
        {
            throw new NotImplementedException();
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
    }
}
