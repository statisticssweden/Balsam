using Balsam.Api.Models;
using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Balsam.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : BalsamApi.Server.Controllers.ProjectApiController
    {

        private readonly CapabilityOptions _git;
        private readonly CapabilityOptions _authentication;
        private readonly HubClient _hubClient;


        public ProjectController(IOptionsSnapshot<CapabilityOptions> capabilityOptions, HubClient hubClient)
        {
            _hubClient = hubClient;

            _git = capabilityOptions.Get(Capabilities.Git);
            _authentication = capabilityOptions.Get(Capabilities.Authentication);
        }

        public override Task<IActionResult> CreateBranch([FromRoute(Name = "projectId"), Required] string projectId, [FromBody] CreateBranchRequest? createBranchRequest)
        {
            throw new NotImplementedException();
        }


        public async override Task<IActionResult> CreateProject([FromBody] CreateProjectRequest? createProjectRequest)
        {
            BalsamProject project = await _hubClient.CreateProject(createProjectRequest.Name);

            if (project == null)
            {
                return BadRequest(new Problem() { Title = "Project with that name already exists", Status = 400, Type = "Project duplication" });
            }

            var evt = new ProjectCreatedResponse();
            evt.Id = project.Id;
            evt.Name = project.Name;

            return Ok(evt);
        }

        public override Task<IActionResult> GetFiles([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public override Task<IActionResult> GetProject([FromRoute(Name = "projectId"), Required] string projectId)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public override async Task<IActionResult> ListProjects([FromQuery(Name = "all")] bool? all)
        {
            var projects = await _hubClient.GetProjects();
            var projectListResponse = new ProjectListResponse();
            ///TODO Make sure that Description is set on CreateProject
            projectListResponse.Projects = MapProject(projects);
            
            return Ok(projectListResponse);
        }

        private List<Project> MapProject(List<BalsamProject> projects)
        {
            return projects.Select(project => new Project() {   Id = project.Id, 
                                                                Name = project.Name, 
                                                                Description = project.Description, 
                                                                Branches = MapBranches(project.Branches) }).ToList();
        }

        private List<Branch> MapBranches(List<BalsamBranch> branches)
        {
            return branches.Select(branch => new Branch() { Id = branch.Id, 
                                                            Description = branch.Description, 
                                                            Name = branch.Name, 
                                                            IsDefault = branch.IsDefault }).ToList();
        }


    }
}
