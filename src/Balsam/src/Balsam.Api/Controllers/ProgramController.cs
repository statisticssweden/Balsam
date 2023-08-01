using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Balsam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController : BalsamApi.Server.Controllers.ProgramApiController
    {
        private static List<BalsamApi.Server.Models.Program> _programs;

        static ProgramController()
        {
            _programs = new List<BalsamApi.Server.Models.Program>();
            _programs.Add(new BalsamApi.Server.Models.Program() { Id = "P1", Name = "Demo", Projects = new List<Project>() });
        }

        public override IActionResult CreateProgram([FromQuery(Name = "preferredName"), Required] string preferredName, [FromQuery(Name = "test"), Required] string test)
        {
            //TODO Implement

            //Mock implementation
            var program = new BalsamApi.Server.Models.Program();
            program.Id = Guid.NewGuid().ToString();
            program.Name = preferredName;
            program.Projects = new List<Project>();
             _programs.Add(program);

            var evt = new CreatedResponse();
            evt.Id = program.Id;
            evt.Name = program.Name;
            
            return Ok(evt);
        }

        public override IActionResult CreateProject([FromRoute(Name = "programId"), Required] string programId, [FromQuery(Name = "preferredName"), Required] string preferredName)
        {
            //TODO Implement

            //Mock implementation
            var program = _programs.FirstOrDefault(p => p.Id.Equals(programId, StringComparison.OrdinalIgnoreCase));
            if (program is null) {
                return BadRequest(new Problem() { Status = 400, Title = "No program" });
            }

            var project = new Project();
            project.Id = Guid.NewGuid().ToString();
            project.Name = preferredName;
            project.Workspaces = new List<Workspace>();
            program.Projects.Add(project);

            var evt = new CreatedResponse();
            evt.Id = project.Id;
            evt.Name = project.Name;

            return Ok(evt);
        }

        public override IActionResult CreateWorkspace([FromRoute(Name = "programId"), Required] string programId, [FromRoute(Name = "projectId"), Required] string projectId, [FromQuery(Name = "preferredName"), Required] string preferredName)
        {
            //TODO Implement

            //Mock implementation
            var program = _programs.FirstOrDefault(p => p.Id.Equals(programId, StringComparison.OrdinalIgnoreCase));
            if (program is null)
            {
                return BadRequest(new Problem() { Status = 400, Title = "None existing program" });
            }

            var project = program.Projects.FirstOrDefault(p => p.Id.Equals(projectId, StringComparison.OrdinalIgnoreCase));
            if (project is null)
            {
                return BadRequest(new Problem() { Status = 400, Title = "None existing project" });
            }

            var workspace = new Workspace();
            workspace.Id = Guid.NewGuid().ToString();
            workspace.Name = preferredName;
            project.Workspaces.Add(workspace);

            var evt = new CreatedResponse();
            evt.Id = project.Id;
            evt.Name = project.Id;

            return Ok(evt);
        }

        public override IActionResult DeleteWorkspace([FromRoute(Name = "programId"), Required] string programId, [FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "workspaceId"), Required] string workspaceId)
        {
            //TODO Implement

            //Mock implementation
            var program = _programs.FirstOrDefault(p => p.Id.Equals(programId, StringComparison.OrdinalIgnoreCase));
            if (program is null)
            {
                return BadRequest(new Problem() { Status = 400, Title = "None existing program" });
            }

            var project = program.Projects.FirstOrDefault(p => p.Id.Equals(projectId, StringComparison.OrdinalIgnoreCase));
            if (project is null)
            {
                return BadRequest(new Problem() { Status = 400, Title = "None existing project" });
            }

            var workspace = project.Workspaces.FirstOrDefault(p => p.Id.Equals(workspaceId, StringComparison.OrdinalIgnoreCase));

            if (workspace is null)
            {
                return BadRequest(new Problem() { Status = 400, Title = "None existing workspace" });
            }
            
            project.Workspaces.Remove(workspace);

            return Ok(project);

        }

        public override IActionResult ListProgram()
        {
            //TODO Implement

            //Mock implementation
            var evt = new ProgramsListResponse();
            evt.Programs = _programs;

            return Ok(evt);
            
        }

        public override IActionResult ListTemplates()
        {
            var templates = new List<Template>();

            templates.Add(new Template() {Id = "JYP_PyTorch", Name = "Jupyter with PyTorch", Description = "Jupyter with PyTorch 2.0 CPU only"} );
            templates.Add(new Template() { Id = "JYP_SciKit", Name = "Jupyter with Scikit Learn ", Description = "Jupyter with scikit-learn 1.2" });

            return Ok(templates.ToArray());
        }
    }
}
