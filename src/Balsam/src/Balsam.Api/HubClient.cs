using Balsam.Api.Models;
using GitProviderApiClient.Api;
using GitProviderApiClient.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Model;
using OidcProviderApiClient.Api;
using OidcProviderApiClient.Model;
using System.Text.RegularExpressions;
using System.IO.Hashing;

namespace Balsam.Api
{
    public class HubClient
    {
        private readonly CapabilityOptions _git;
        private readonly CapabilityOptions _s3;
        private readonly CapabilityOptions _authentication;
        private readonly IBucketApi _s3Client;
        private readonly IGroupApi _oidcClient;
        private readonly HubRepositoryClient _hubRepositoryClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IRepositoryApi _repositoryApi;
        private readonly ILogger<HubClient> _logger;


        public HubClient(ILogger<HubClient> logger, IOptionsSnapshot<CapabilityOptions> capabilityOptions, IMemoryCache memoryCach, HubRepositoryClient hubRepoClient, IBucketApi s3Client, IRepositoryApi reposiotryApi, IGroupApi oidcClient)
        {
            _logger = logger;
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
                else
                {
                    _logger.LogWarning($"Could not parse properties file {propsFile}");
                }
            }
            return projects;
        }

        private async Task<List<BalsamBranch>> ReadBranches(string projectPath)
        {
            var branches = new List<BalsamBranch>();

            if (!System.IO.Directory.Exists(projectPath))
            {
                return branches;
            }

            foreach (var branchPath in Directory.GetDirectories(projectPath))
            {
                var propsFile = Path.Combine(branchPath, "properties.json");
                var branch = JsonConvert.DeserializeObject<BalsamBranch>(await System.IO.File.ReadAllTextAsync(propsFile));
                if (branch != null) { 
                    branches.Add(branch);
                }
                else
                {
                    _logger.LogWarning($"Could not parse properties file {propsFile}");
                }
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

        public async Task<BalsamProject?> CreateProject(string preferredName, string description, string defaultBranchName, string username)
        {
            //Check if there is a program with the same name.
            if (await ProjectExists(preferredName))
            {
                _logger.LogInformation($"Could not create project {preferredName}, due to name dublication");
                return null;
            }

            var project = new BalsamProject(SanitizeName(preferredName), preferredName,  description);
            string programPath = Path.Combine(_hubRepositoryClient.Path, "hub", project.Id);
            DirectoryUtil.AssureDirectoryExists(programPath);


            if (_authentication.Enabled)
            {
                var oidcData = await _oidcClient.CreateGroupAsync(new CreateGroupRequest(project.Id));
                await _oidcClient.AddUserToGroupAsync(oidcData.Id, new AddUserToGroupRequest(username));
                project.Oidc = new OidcData(oidcData.Id, oidcData.Name);
                _logger.LogInformation($"Group {project.Oidc.GroupName}({project.Oidc.GroupId}) created");
            }

            if (_git.Enabled)
            {
                var gitData = await _repositoryApi.CreateRepositoryAsync(new CreateRepositoryRequest( preferredName,description, defaultBranchName));
                project.Git = new GitData() { Name = gitData.Name, Path = gitData.Path };
                _logger.LogInformation($"Git repository {project.Git.Name} created");
            }

            if (_s3.Enabled)
            {
                var s3Data = await _s3Client.CreateBucketAsync(new S3ProviderApiClient.Model.CreateBucketRequest(preferredName));
                project.S3 = new S3Data() { BucketName = s3Data.Name };
                _logger.LogInformation($"Bucket {project.S3.BucketName} created");
            }

                
            string propPath = Path.Combine(programPath, "properties.json");

            if (await CreateBranch(project, defaultBranchName, description, true))
            {
                _logger.LogInformation($"Default Balsam branch {defaultBranchName} created");
            }

            _hubRepositoryClient.PullChanges();
            // serialize JSON to a string and then write string to a file
            await System.IO.File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(project));
            _hubRepositoryClient.PersistChanges($"New program with id {project.Id}");
            _logger.LogInformation($"Project {project.Name}({project.Id}) created");
            return project;
        }

        private async Task<bool> CreateBranch(BalsamProject project, string branchName, string description, bool isDefault = false)
        {
            var branchId = SanitizeName(branchName);
            string branchPath = Path.Combine(_hubRepositoryClient.Path, "hub", project.Id, branchId);

            DirectoryUtil.AssureDirectoryExists(branchPath);

            if (!isDefault) { 
                //TODO create a Git branch
            }

            if (project.S3 is null || string.IsNullOrEmpty(project.S3.BucketName))
            {
                return false;
            }

            //TODO do we ned to save the response information?
            await _s3Client.CreateFolderAsync(project.S3.BucketName, new CreateFolderRequest(branchName));
            _logger.LogInformation($"Folder {branchName} created in bucket {project.S3.BucketName}.");

            var branch = new BalsamBranch()
                                {Id = branchId,
                                 Name = branchName,
                                 Description = description,
                                 IsDefault = isDefault,
                                 GitBranch = branchName};

            string propPath = Path.Combine(branchPath, "properties.json");
            await System.IO.File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(branch));

            return true;
        }

        private static string SanitizeName(string name)
        {
            var crc32 = new Crc32();

            crc32.Append(System.Text.Encoding.ASCII.GetBytes(name));
            var hash = crc32.GetCurrentHash();
            var crcHash = string.Join("", hash.Select(b => b.ToString("x2").ToLower()).Reverse());
            
            name = name.ToLower(); //Only lower charachters allowed
            name = name.Replace(" ", "-"); //replaces spaches with hypen
            name = Regex.Replace(name, @"[^a-z0-9\-]", ""); // make sure that only a-z or digit or hypen removes all other characters
            name = name.Substring(0, 50 - crcHash.Length) + crcHash; //Assures max size of 50 characters

            return name;

        }
    }
}
