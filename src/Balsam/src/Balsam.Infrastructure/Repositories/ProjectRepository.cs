using Balsam.Model;
using Balsam.Utility;
using Newtonsoft.Json;
using File = System.IO.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Balsam.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IHubRepositoryClient _hubRepositoryClient;
        private readonly ILogger<ProjectRepository> _logger;
        private readonly CapabilityOptions _git;

        protected HubPaths HubPaths { get; set; }

        public ProjectRepository(IOptionsSnapshot<CapabilityOptions> capabilityOptions, ILogger<ProjectRepository> logger, IHubRepositoryClient hubRepositoryClient)
        {
            _git = capabilityOptions.Get(Capabilities.Git);
            _logger = logger;
            _hubRepositoryClient = hubRepositoryClient;
            HubPaths = new HubPaths(_hubRepositoryClient.Path);
        }

        public async Task<List<BalsamProject>> GetProjects(bool includeBranches = true)
        {
            var projects = new List<BalsamProject>();
            //var hubPath = Path.Combine(_hubRepositoryClient.Path, "hub");
            var hubPath = HubPaths.GetHubPath();

            foreach (var projectPath in Directory.GetDirectories(hubPath))
            {
                var propsFile = Path.Combine(projectPath, "properties.json");
                var project = JsonConvert.DeserializeObject<BalsamProject>(await File.ReadAllTextAsync(propsFile));
                if (project != null)
                {
                    if (includeBranches)
                    {
                        project.Branches = await GetBranches(project.Id);
                    }
                    projects.Add(project);
                }
                else
                {
                    _logger.LogWarning($"Could not parse properties file {propsFile}");
                }
            }
            return projects;
        }

        //TODO:Move to other repository
        public async Task<FileContent> GetFile(string projectId, string branchId, string fileId)
        {
            var project = await GetProject(projectId);
            var branch = await GetBranch(projectId, branchId);
            if (project is null || branch is null || project.Git is null)
            {
                throw new ArgumentException("File not found");
            }

            try
            {
                
                //TODO:Use Git provider API...
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_git.ServiceLocation}/repos/{project.Git.Id}/branches/{branch.GitBranch}/files/{fileId}");

                var httpClient = new HttpClient();

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
                    return new FileContent(data, response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Could not read files from repository";
                _logger.LogError(ex, errorMessage);
                throw new ApplicationException(errorMessage);
            }

            return null;
        }

        public async Task<BalsamProject?> GetProject(string projectId, bool includeBranches = true)
        {
            //var projectPath = Path.Combine(_hubRepositoryClient.Path, "hub", projectId);
            var projectPath = HubPaths.GetProjectPath(projectId);

            var propsFile = Path.Combine(projectPath, "properties.json");

            if (!File.Exists(propsFile))
            {
                var errorMessage = $"Project with project id {projectId} does not exist";
                _logger.LogError(errorMessage);
                return null;
            }

            var project = JsonConvert.DeserializeObject<BalsamProject>(await File.ReadAllTextAsync(propsFile));

            if (project == null)
            {
                var errorMessage = $"Project with project id {projectId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            if (includeBranches)
            {
                project.Branches = await GetBranches(project.Id);
            }

            return project!;
        }

        private async Task<List<BalsamBranch>> GetBranches(string projectId)
        {
            var branches = new List<BalsamBranch>();

            var projectPath = HubPaths.GetProjectPath(projectId);
            if (!Directory.Exists(projectPath))
            {
                var errorMessage = $"Project with project id {projectId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            foreach (var branchPath in Directory.GetDirectories(projectPath))
            {
                var propsFile = Path.Combine(branchPath, "properties.json");
                var branch = JsonConvert.DeserializeObject<BalsamBranch>(await File.ReadAllTextAsync(propsFile));
                if (branch != null)
                {
                    branches.Add(branch);
                }
                else
                {
                    _logger.LogWarning($"Could not parse properties file {propsFile}");
                }
            }

            //var gitBranches = await _repositoryApi.GetBranchesAsync(repositoryId);

            //var gitBranchesWithoutBalsamBranch = gitBranches.Where(gitBranch => !branches.Any(b => b.GitBranch == gitBranch));

            //foreach (var gitBranch in gitBranchesWithoutBalsamBranch)
            //{
            //    var newBalsamBranch = await CreateBalsamBranchFromGitBranch(projectId, gitBranch);
            //    if (newBalsamBranch != null)
            //    {
            //        branches.Add(newBalsamBranch);
            //    }
            //    else
            //    {
            //        _logger.LogWarning($"Could not create BalsamBranch from git-branch {gitBranch} for project {projectId}");
            //    }
            //}

            return branches;
        }


        public async Task<BalsamBranch?> GetBranch(string projectId, string branchId)
        {
            var propsFile = Path.Combine(HubPaths.GetBranchPath(projectId, branchId), "properties.json");

            if (!File.Exists(propsFile))
            {
                return null;
            }

            var branch = JsonConvert.DeserializeObject<BalsamBranch>(await File.ReadAllTextAsync(propsFile));
            return branch;
        }


        public async Task<bool> ProjectExists(string projectName)
        {
            var projects = await GetProjects(false); //TODO: Optimize
            if (projects.FirstOrDefault(p => p.Name == projectName) == null)
            {
                return false;
            }
            return true;
        }

        public async Task<BalsamProject?> CreateProject(BalsamProject project, string defaultBranchName)
        {
            _logger.LogDebug($"create project information");

            var id = NameUtil.SanitizeName(project.Name);

            project.Id = id;

            await AddOrUpdate(project);

            var branch = await CreateBranch(project, defaultBranchName, project.Description, true);

            if (branch != null)
            {
                _logger.LogInformation($"Default Balsam branch {defaultBranchName} created");
                project.Branches.Add(branch);
            }
            else
            {
                throw new ApplicationException($"Could not create branch for project {project.Id}.");
            }

            return project;
        }

        private async Task AddOrUpdate(BalsamProject project)
        {
            string projectPath = HubPaths.GetProjectPath(project.Id);

            _hubRepositoryClient.PullChanges();
            _logger.LogDebug($"Assure path exists {projectPath}");

            DirectoryUtil.AssureDirectoryExists(projectPath);

            string propPath = Path.Combine(projectPath, "properties.json");
            // serialize JSON to a string and then write string to a file
            await File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(project));
            _hubRepositoryClient.PersistChanges($"New program with id {project.Id}");
        }

        public async Task UpdateProject(BalsamProject project)
        {
            await AddOrUpdate(project);
        }


        public async Task<BalsamBranch?> CreateBranch(string projectId, string branchName, string description)
        {
            var project = await GetProject(projectId, false);

            if (project is null)
            {
                throw new ArgumentException($"Project with id {projectId} does not exist", projectId);
            }

            var createdBranch = await CreateBranch(project, branchName, description, false);
            return createdBranch;
        }

        private async Task<BalsamBranch?> CreateBranch(BalsamProject project, string branchName, string description, bool isDefault = false)
        {
            var branchId = NameUtil.SanitizeName(branchName);
            var branchPath = HubPaths.GetBranchPath(project.Id, branchId);

            DirectoryUtil.AssureDirectoryExists(branchPath);

            var branch = new BalsamBranch()
            {
                Id = branchId,
                Name = branchName,
                Description = description,
                IsDefault = isDefault,
                GitBranch = branchName
            };

            var propPath = Path.Combine(branchPath, "properties.json");

            _hubRepositoryClient.PullChanges();
            await File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(branch));
            _hubRepositoryClient.PersistChanges($"Branch {branchName} created for project {project.Name}");

            return branch;
        }

        public async Task<BalsamBranch?> CreateBalsamBranchFromGitBranch(string projectId, string gitBranchName)
        {
            var project = await GetProject(projectId, false);

            if (project is null || project.Git is null)
            {
                return null;
            }

            var createdBranch = await CreateBranch(project, gitBranchName, "", false);

            return createdBranch;
        }

        public async Task DeleteBranch(string projectId, string branchId)
        {
            var branchPath = HubPaths.GetBranchPath(projectId, branchId);

            var propsFile = Path.Combine(branchPath, "properties.json");

            _hubRepositoryClient.PullChanges();

            if (File.Exists(propsFile))
            {
                await Task.Run(() => File.Delete(propsFile));
            }

            _hubRepositoryClient.PersistChanges($"Branch {branchId} deleted for project {projectId}");

        }

        public async Task DeleteProject(string projectId)
        {
            var projectPath = Path.Combine(_hubRepositoryClient.Path, "hub", projectId);

            _hubRepositoryClient.PullChanges();

            var propPath = Path.Combine(projectPath, "properties.json");

            if (!Directory.Exists(projectPath))
            {
                return;
            }

            if (File.Exists(propPath))
            {
                await Task.Run(() => File.Delete(propPath));
            }

            _hubRepositoryClient.PersistChanges($"Project {projectId} deleted");
        }

    
    }
}
