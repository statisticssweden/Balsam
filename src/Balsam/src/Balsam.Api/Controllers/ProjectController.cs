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
    [Authorize]
    public class ProjectController : BalsamApi.Server.Controllers.ProjectApiController
    {

        private static ProjectListResponse projects;


        static ProjectController()
        {
            projects = new ProjectListResponse();

            projects.Projects = new List<Project>();

            var project = new Project() { Id = "001", Name = "Projekt 1", Description = "Lore Ipsum", Branches = new List<Branch>(), GitUrl= "https://balsam-gitlab-pilot.tanzu.scb.intra/resources/kopia-av-undersokningsmall_08e94f8d-fed0-444e-a6d0-b945c189a612.git" };
            project.Branches.Add(new Branch() { Id = "main", Name = "main", Description = "Lore Ipsum", IsDefault = true });
            project.Branches.Add(new Branch() { Id = "feature", Name = "feature", Description = "New feature branch", IsDefault = false });
            projects.Projects.Add(project);
        }

        public override Task<IActionResult> CreateBranch([FromRoute(Name = "projectId"), Required] string projectId, [FromBody] CreateBranchRequest? createBranchRequest)
        {
            var project = projects.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project is null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new Problem() { Status = 400, Title = "Project not found" }));
            }
            if (createBranchRequest is null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new Problem() { Status = 400, Title = "Parameter error" }));
            }
            project.Branches.Add(new Branch() { Id = createBranchRequest.Name, Name = createBranchRequest.Name, Description = createBranchRequest.Description, IsDefault = false });

            var resp = new BranchCreatedResponse() { Id = createBranchRequest.Name, Name = createBranchRequest.Name, ProjectId = project.Id };


            return Task.FromResult<IActionResult>(Ok(resp));

        }


        public override Task<IActionResult> CreateProject([FromBody] CreateProjectRequest? createProjectRequest)
        {
            if (createProjectRequest is null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new Problem() { Status = 400, Title = "Parameter error" }));
            }

            var project = projects.Projects.FirstOrDefault(p => p.Name == createProjectRequest.Name);
            if (!(project is null))
            {
                return Task.FromResult<IActionResult>(BadRequest(new Problem() { Status = 400, Title = "Duplicate name" }));
            }

            project = new Project() { Id = Guid.NewGuid().ToString(), Name = createProjectRequest.Name, Description = createProjectRequest.Description, Branches = new List<Branch>() { new Branch() { Id = createProjectRequest.BranchName, Name = createProjectRequest.Name, IsDefault = true, Description = "Default branch" } }  };

            projects.Projects.Add(project);

            var resp = new ProjectCreatedResponse() { Id = project.Id, Name = project.Name };


            return Task.FromResult<IActionResult>(Ok(resp));
        }

        public override Task<IActionResult> GetFiles([FromRoute(Name = "projectId"), Required] string projectId, [FromRoute(Name = "branchId"), Required] string branchId)
        {
            var files = new List<BalsamApi.Server.Models.File>();
            files.Add(new BalsamApi.Server.Models.File()
            {
                Type = BalsamApi.Server.Models.File.TypeEnum.FileEnum,
                Name = "README.md",
                Path = "/README.md",
                ContentUrl = "https://balsam-gitlab-pilot.tanzu.scb.intra/resources/kopia-av-undersokningsmall_08e94f8d-fed0-444e-a6d0-b945c189a612/-/raw/378ad7447653d9f13118c21b9ce1fadc0c24b1c6/README.md"
            });


            files.Add(new BalsamApi.Server.Models.File()
            {
                Type = BalsamApi.Server.Models.File.TypeEnum.FileEnum,
                Name = "sps.url",
                Path = "/scb.se.url",
                ContentUrl = "https://balsam-gitlab-pilot.tanzu.scb.intra/resources/kopia-av-undersokningsmall_08e94f8d-fed0-444e-a6d0-b945c189a612/-/raw/378ad7447653d9f13118c21b9ce1fadc0c24b1c6/Resources/Statistikproduktionsst%C3%B6d.url"
            });

            files.Add(new BalsamApi.Server.Models.File()
            {
                Type = BalsamApi.Server.Models.File.TypeEnum.FolderEnum,
                Name = "Resources",
                Path = "/Resouces",
                ContentUrl = ""
            });
            files.Add(new BalsamApi.Server.Models.File()
            {
                Type = BalsamApi.Server.Models.File.TypeEnum.FolderEnum,
                Name = "Kodning",
                Path = "/Resouces/Kodning",
                ContentUrl = ""
            });
            files.Add(new BalsamApi.Server.Models.File()
            {
                Type = BalsamApi.Server.Models.File.TypeEnum.FileEnum,
                Name = "README.md",
                Path = "/Resouces/Kodning/README.md",
                ContentUrl = "https://balsam-gitlab-pilot.tanzu.scb.intra/resources/kopia-av-undersokningsmall_08e94f8d-fed0-444e-a6d0-b945c189a612/-/raw/378ad7447653d9f13118c21b9ce1fadc0c24b1c6/README.md"
            });

            return Task.FromResult<IActionResult>(Ok(files.ToArray()));
        }

        //[Authorize]
        public override Task<IActionResult> GetProject([FromRoute(Name = "projectId"), Required] string projectId)
        {

            var project = projects.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project is null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new Problem() { Status = 400, Title = "Project not found" }));
            }

            var projectResponse = new ProjectResponse() { Id = project.Id, Name = project.Name, Branches = project.Branches, Description = project.Description, GitUrl = "https://github.com/statisticssweden/Balsam.git" };

            return Task.FromResult<IActionResult>(Ok(projectResponse));
        }

        //[Authorize]
        public override Task<IActionResult> ListProjects([FromQuery(Name = "all")] bool? all)
        {
            return Task.FromResult<IActionResult>(Ok(projects));
        }


    }
}
