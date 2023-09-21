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
            //var templates = new List<Template>();

            //templates.Add(new Template() { Id = "JYP_PyTorch", Name = "Jupyter with PyTorch", Description = "Jupyter with PyTorch 2.0 CPU only" });
            //templates.Add(new Template() { Id = "JYP_SciKit", Name = "Jupyter with Scikit Learn ", Description = "Jupyter with scikit-learn 1.2" });

            //return Ok(templates.ToArray());
            throw new NotImplementedException();
        }
    }
}
