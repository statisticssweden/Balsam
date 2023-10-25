using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Balsam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : BalsamApi.Server.Controllers.WorkspaceApiController
    {
        private readonly ILogger<WorkspaceController> _logger;
        private readonly HubClient _hubClient;

        public WorkspaceController(ILogger<WorkspaceController> logger, HubClient hubClient)
        {
            _logger = logger;
            _hubClient = hubClient;
        }

        public override Task<IActionResult> CreateKnowledgeLibrary([FromBody] CreateKnowledgeLibraryRequest? createKnowledgeLibraryRequest)
        {
            throw new NotImplementedException();
        }

        public override Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest? createWorkspaceRequest)
        {
            throw new NotImplementedException();
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
