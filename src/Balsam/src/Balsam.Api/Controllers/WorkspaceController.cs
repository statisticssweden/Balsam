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
        private readonly HubClient _hubClient;
        private readonly ILogger<ProjectController> _logger;

        public WorkspaceController(ILogger<ProjectController> logger, HubClient hubClient)
        {
            _logger = logger;
            _hubClient = hubClient; 
        }

        public override Task<IActionResult> CreateKnowledgeLibrary([FromBody] CreateKnowledgeLibraryRequest? createKnowledgeLibraryRequest)
        {
            throw new NotImplementedException();
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
            //var templates = new List<Template>();

            //templates.Add(new Template() { Id = "JYP_PyTorch", Name = "Jupyter with PyTorch", Description = "Jupyter with PyTorch 2.0 CPU only" });
            //templates.Add(new Template() { Id = "JYP_SciKit", Name = "Jupyter with Scikit Learn ", Description = "Jupyter with scikit-learn 1.2" });

            //return Ok(templates.ToArray());
            throw new NotImplementedException();
        }
    }
}
