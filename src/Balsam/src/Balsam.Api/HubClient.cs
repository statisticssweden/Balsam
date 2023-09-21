using Balsam.Api.Models;
using BalsamApi.Server.Models;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Balsam.Api
{
    public class HubClient
    {
        private readonly CapabilityOptions _git;
        private readonly CapabilityOptions _s3;
        private readonly CapabilityOptions _authentication;
        private readonly S3Client _s3Client;
        private readonly GitClient _gitClient;
        private readonly HubRepositoryClient _hubRepositoryClient;
        private readonly IMemoryCache _memoryCache;



        public HubClient(IOptionsSnapshot<CapabilityOptions> capabilityOptions, IMemoryCache memoryCach, HubRepositoryClient hubRepoClient, S3Client s3Client, GitClient gitClient)
        {
            _memoryCache = memoryCach;
            _s3Client = s3Client;
            _gitClient = gitClient;
            _hubRepositoryClient = hubRepoClient;

            _git = capabilityOptions.Get(Capabilities.Git);
            _s3 = capabilityOptions.Get(Capabilities.S3);
            _authentication = capabilityOptions.Get(Capabilities.Authentication);
        }

        private List<BalsamProject> GetProjects()
        {
            var projects = new List<BalsamProject>();
            var hubPath = Path.Combine(_hubRepositoryClient.Path, "hub");

            foreach (var projectPath in Directory.GetDirectories(hubPath))
            {
                var propsFile = Path.Combine(projectPath, "properties.json");
                var project = JsonConvert.DeserializeObject<BalsamProject>(System.IO.File.ReadAllText(propsFile));
                projects.Add(project);
            }
            return projects;
        }

        private bool ProjectExisits(string projectName)
        {
            var projects = GetProjects();
            if (projects.FirstOrDefault(p => p.Name == projectName) == null)
            {
                return false;
            }
            return true;
        }

        public async Task<BalsamProject> CreateProject(string preferredName)
        {
            //Check if there is a program with the same name.
            if (ProjectExisits(preferredName))
            {
                return null;
            }

            var project = new BalsamProject();
            project.Id = Guid.NewGuid().ToString();
            project.Name = preferredName;

            string programPath = Path.Combine(_hubRepositoryClient.Path, "hub", project.Id);
            DirectoryUtil.AssureDirectoryExists(programPath);

            var tasks = new List<Task>();
            Task<S3Data> s3Task = null;
            Task<GitData> gitTask = null;

            //TODO Implement
            if (_authentication.Enabled)
            {
                //TODO call CreateRole in the OidcProvider
                //TODO call AddUserToRolein the OidcProvider
            }

            if (_git.Enabled)
            {
                gitTask = _gitClient.CreateRepository(preferredName);
                tasks.Add(gitTask);
            }

            if (_s3.Enabled)
            {
                s3Task = _s3Client.CreateBucket(preferredName);
                tasks.Add(s3Task);
            }

            //wait for all task to finish
            await Task.WhenAll(tasks);

            //fetch and sstore the results for each provider
            if (s3Task != null)
            {
                project.S3 = s3Task.Result;
            }

            if (gitTask != null)
            {
                project.Git = gitTask.Result;
            }

            string propPath = Path.Combine(programPath, "properties.json");

            _hubRepositoryClient.PullChanges();
            // serialize JSON to a string and then write string to a file
            await System.IO.File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(project));
            _hubRepositoryClient.PersistChanges($"New program with id {project.Id}");
            return project;
        }
    }
}
