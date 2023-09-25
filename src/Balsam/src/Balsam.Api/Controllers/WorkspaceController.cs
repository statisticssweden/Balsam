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

        private static List<Workspace> workspaces = new List<Workspace>();

        public override Task<IActionResult> CreateKnowledgeLibrary([FromBody] CreateKnowledgeLibraryRequest? createKnowledgeLibraryRequest)
        {
            throw new NotImplementedException();
        }

        public override Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest? createWorkspaceRequest)
        {
            var workspace = new Workspace() { Id = Guid.NewGuid().ToString(), ProjectId = createWorkspaceRequest.ProjectId, BranchId = createWorkspaceRequest.BranchId, Name = createWorkspaceRequest.Name, TemplateId = createWorkspaceRequest.TemplateId };
            workspaces.Add(workspace);
            var resp = new WorkspaceCreatedResponse() { Id = workspace.Id, ProjectId = workspace.ProjectId, BranchId = workspace.BranchId, Name = workspace.Name };
            return Task.FromResult<IActionResult>(Ok(resp));
        }

        public override Task<IActionResult> DeleteWorkspace([FromRoute(Name = "workspaceId"), Required] string workspaceId)
        {
            var workspace = workspaces.FirstOrDefault(w => w.Id == workspaceId);

            if (workspace is null) {
                return Task.FromResult<IActionResult>(BadRequest(new Problem() { Status = 400, Title = "Can not find workspace" }));
            }

            workspaces.Remove(workspace);

            return Task.FromResult<IActionResult>(Ok());
        }

        public override Task<IActionResult> GetWorkspace([FromQuery(Name = "projectId")] string? projectId, [FromQuery(Name = "branchId")] string? branchId, [FromQuery(Name = "all")] bool? all)
        {
            if (projectId != null)
            {
                if (branchId != null)
                {
                    return Task.FromResult<IActionResult>(Ok(workspaces.Where(w => w.ProjectId == projectId && w.BranchId == branchId).ToArray()));
                }else
                {
                    return Task.FromResult<IActionResult>(Ok(workspaces.Where(w => w.ProjectId == projectId).ToArray()));
                }
            }
            return Task.FromResult<IActionResult>(Ok(workspaces.ToArray()));

        }

        public override Task<IActionResult> ListTemplates()
        {
            var templates = new List<Template>();

            templates.Add(new Template() { Id = "jyp_pytorch", Name = "jupyter with pytorch", Description = "jupyter with pytorch 2.0 cpu only" });
            templates.Add(new Template() { Id = "jyp_scikit", Name = "jupyter with scikit learn ", Description = "jupyter with scikit-learn 1.2" });

            return Task.FromResult<IActionResult>(Ok(templates.ToArray()));
        }
    }
}
