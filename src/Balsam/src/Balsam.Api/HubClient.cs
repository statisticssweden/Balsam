using Balsam.Api.Models;
using BalsamApi.Server.Models;
using GitProviderApiClient.Api;
using GitProviderApiClient.Model;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Model;
using System.Threading.Tasks;

namespace Balsam.Api
{
    public class HubClient
    {
        private readonly CapabilityOptions _git;
        private readonly CapabilityOptions _s3;
        private readonly CapabilityOptions _authentication;
        private readonly BucketApi _s3Client;
        private readonly HubRepositoryClient _hubRepositoryClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IRepositoryApi _repositoryApi;



        public HubClient(IOptionsSnapshot<CapabilityOptions> capabilityOptions, IMemoryCache memoryCach, HubRepositoryClient hubRepoClient, BucketApi s3Client, IRepositoryApi reposiotryApi)
        {
            _memoryCache = memoryCach;
            _s3Client = s3Client;
           
            _hubRepositoryClient = hubRepoClient;

            _git = capabilityOptions.Get(Capabilities.Git);
            _s3 = capabilityOptions.Get(Capabilities.S3);
            _authentication = capabilityOptions.Get(Capabilities.Authentication);
            _repositoryApi = reposiotryApi;
        }

        public async Task<List<BalsamProject>> GetProjects()
        {
            var projects = new List<BalsamProject>();
            var hubPath = Path.Combine(_hubRepositoryClient.Path, "hub");

            foreach (var projectPath in Directory.GetDirectories(hubPath))
            {
                var propsFile = Path.Combine(projectPath, "properties.json");
                var project = JsonConvert.DeserializeObject<BalsamProject>(await System.IO.File.ReadAllTextAsync(propsFile));
                if (project != null)
                {
                    project.Branches = await ReadBranches(projectPath);
                    projects.Add(project);
                }
            }
            return projects;
        }

        private async Task<List<BalsamBranch>> ReadBranches(string projectPath)
        {
            var branches = new List<BalsamBranch>();

            if (System.IO.Directory.Exists(projectPath))
            {
                return branches;
            }

            foreach (var branchPath in Directory.GetDirectories(projectPath))
            {
                var propsFile = Path.Combine(projectPath, "properties.json");
                var branch = JsonConvert.DeserializeObject<BalsamBranch>(await System.IO.File.ReadAllTextAsync(propsFile));
                branches.Add(branch);
            }
            return branches;
        }

        private async Task<bool> ProjectExists(string projectName)
        {
            var projects = await GetProjects();
            if (projects.FirstOrDefault(p => p.Name == projectName) == null)
            {
                return false;
            }
            return true;
        }

        public async Task<BalsamProject> CreateProject(string preferredName, string description, string defaultBranchName)
        {
            //Check if there is a program with the same name.
            if (await ProjectExists(preferredName))
            {
                return null;
            }

            var project = new BalsamProject();
            project.Id = Guid.NewGuid().ToString();
            project.Name = preferredName;

            string programPath = Path.Combine(_hubRepositoryClient.Path, "hub", project.Id);
            DirectoryUtil.AssureDirectoryExists(programPath);

            var tasks = new List<Task>();
            Task<BucketCreatedResponse> s3Task = null;
        Task<RepositoryCreatedResponse> gitTask = null;

        //TODO Implement
        if (_authentication.Enabled)
            {
                //TODO call CreateRole in the OidcProvider
                //TODO call AddUserToRolein the OidcProvider
            }

            if (_git.Enabled)
            {

                gitTask = _repositoryApi.CreateRepositoryAsync(new CreateRepositoryRequest( preferredName,description, defaultBranchName));
                tasks.Add(gitTask);
            }

            if (_s3.Enabled)
            {
                s3Task = _s3Client.CreateBucketAsync(new S3ProviderApiClient.Model.CreateBucketRequest(preferredName));
                tasks.Add(s3Task);
            }

            //wait for all task to finish
            await Task.WhenAll(tasks);

            //fetch and stores the results for each provider
            if (s3Task != null)
            {
                var s3Data = new S3Data() { BucketName = s3Task.Result.Name };
                project.S3 = s3Data;
            }

            if (gitTask != null)
            {
                var data = new GitData() { Name = gitTask.Result.Name, Path = gitTask.Result.Path };
                project.Git = data;
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
