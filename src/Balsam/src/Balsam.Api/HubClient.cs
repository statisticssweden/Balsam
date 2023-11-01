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
using RocketChatChatProviderApiClient.Api;
using HandlebarsDotNet;
using File = System.IO.File;
using Microsoft.AspNetCore.Mvc;

namespace Balsam.Api
{
    public class HubClient
    {
        private readonly CapabilityOptions _git;
        private readonly CapabilityOptions _s3;
        private readonly CapabilityOptions _authentication;
        private readonly CapabilityOptions _chat;
        private readonly IBucketApi _s3Client;
        private readonly IGroupApi _oidcClient;
        private readonly HubRepositoryClient _hubRepositoryClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IRepositoryApi _repositoryApi;
        private readonly ILogger<HubClient> _logger;
        private readonly IAreaApi _chatClient;
        private readonly IUserApi _gitUserClient;


        public HubClient(ILogger<HubClient> logger, IOptionsSnapshot<CapabilityOptions> capabilityOptions, IMemoryCache memoryCache, HubRepositoryClient hubRepoClient, IBucketApi s3Client, IRepositoryApi reposiotryApi, IGroupApi oidcClient, IAreaApi chatClient, IUserApi gitUserClient)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _s3Client = s3Client;
            _oidcClient = oidcClient;
            _chatClient = chatClient;
            _gitUserClient = gitUserClient;
           
            _hubRepositoryClient = hubRepoClient;

            _git = capabilityOptions.Get(Capabilities.Git);
            _s3 = capabilityOptions.Get(Capabilities.S3);
            _authentication = capabilityOptions.Get(Capabilities.Authentication);
            _chat = capabilityOptions.Get(Capabilities.Chat);
            _repositoryApi = reposiotryApi;

        }

        static HubClient()
        {
            Handlebars.RegisterHelper("curlies", (writer, context, parameters) =>
            {
                if (parameters.Length == 1 && parameters.At<bool>(0) == true)
                {
                    writer.Write("{{");
                }
                else
                {
                    writer.Write("}}");
                }

            });
        }

        public async Task<List<BalsamProject>> GetProjects(bool includeBranches = true)
        {
            var projects = new List<BalsamProject>();
            var hubPath = Path.Combine(_hubRepositoryClient.Path, "hub");

            foreach (var projectPath in Directory.GetDirectories(hubPath))
            {
                var propsFile = Path.Combine(projectPath, "properties.json");
                var project = JsonConvert.DeserializeObject<BalsamProject>(await File.ReadAllTextAsync(propsFile));
                if (project != null)
                {
                    if (includeBranches)
                    {
                        project.Branches = await ReadBranches(projectPath);
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

        public async Task<BalsamProject?> GetProject(string projectId, bool includeBranches = true)
        {
            var projectPath = Path.Combine(_hubRepositoryClient.Path, "hub", projectId);
            var propsFile = Path.Combine(projectPath, "properties.json");

            if (!File.Exists(propsFile))
            {
                return null;
            }
            var project = JsonConvert.DeserializeObject<BalsamProject>(await File.ReadAllTextAsync(propsFile));
            if (project != null && includeBranches)
            {
                project.Branches = await ReadBranches(projectPath);
            }
            return project;
        }

        public async Task<BalsamProject?> GetProject(string projectId)
        {
            var projectPath = Path.Combine(_hubRepositoryClient.Path, "hub", projectId);

            if (!Directory.Exists(projectPath))
            {
                return null;
            }

            var propsFile = Path.Combine(projectPath, "properties.json");
            var project = JsonConvert.DeserializeObject<BalsamProject>(await File.ReadAllTextAsync(propsFile));
            if (project != null)
            {
                project.Branches = await ReadBranches(projectPath);
            }

            return project;
        }

        private async Task<List<BalsamBranch>> ReadBranches(string projectPath)
        {
            var branches = new List<BalsamBranch>();

            if (!Directory.Exists(projectPath))
            {
                return branches;
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
            return branches;
        }

        private async Task<BalsamBranch?> GetBranch(string projectId, string branchId)
        {
            var propsFile = Path.Combine(_hubRepositoryClient.Path, "hub", projectId, branchId, "properties.json");

            if (!File.Exists(propsFile))
            {
                return null;
            }

            var branch = JsonConvert.DeserializeObject<BalsamBranch>(await File.ReadAllTextAsync(propsFile));
            return branch;
        }

        private async Task<bool> ProjectExists(string projectName)
        {
            var projects = await GetProjects(false);
            if (projects.FirstOrDefault(p => p.Name == projectName) == null)
            {
                return false;
            }
            return true;
        }

        public async Task<BalsamProject?> CreateProject(string preferredName, string description, string defaultBranchName, string username)
        {
            //Check if there is a program with the same name.
            _logger.LogDebug("Chack for duplicate names");
            if (await ProjectExists(preferredName))
            {
                _logger.LogInformation($"Could not create project {preferredName}, due to name dublication");
                return null;
            }

            _logger.LogDebug($"create project information");
            var project = new BalsamProject(SanitizeName(preferredName), preferredName, description);
            string projectPath = Path.Combine(_hubRepositoryClient.Path, "hub", project.Id);

            _logger.LogDebug($"Assure path exists {projectPath}");

            DirectoryUtil.AssureDirectoryExists(projectPath);

            _logger.LogDebug($"Begin call service providers");

            if (_authentication.Enabled)
            {
                _logger.LogDebug($"Begin call OpenIdConnect");
                var oidcData = await _oidcClient.CreateGroupAsync(new CreateGroupRequest(project.Id));
                await _oidcClient.AddUserToGroupAsync(oidcData.Id, new AddUserToGroupRequest(username));
                project.Oidc = new OidcData(oidcData.Id, oidcData.Name);
                _logger.LogInformation($"Group {project.Oidc.GroupName}({project.Oidc.GroupId}) created");
            }

            if (project.Oidc == null)
            {
                throw new Exception("Could not parse oidc data");
            }

            if (_git.Enabled)
            {
                _logger.LogDebug($"Begin call Git");
                var gitData = await _repositoryApi.CreateRepositoryAsync(new CreateRepositoryRequest(preferredName, description, defaultBranchName));
                defaultBranchName = gitData.DefaultBranchName;
                project.Git = new GitData() { Id = gitData.Id, Name = gitData.Name, Path = gitData.Path };
                _logger.LogInformation($"Git repository {project.Git.Name} created");
            }

            if (_s3.Enabled)
            {
                _logger.LogDebug($"Begin call S3");
                var s3Data = await _s3Client.CreateBucketAsync(new CreateBucketRequest(preferredName, project.Oidc.GroupName));
                project.S3 = new S3Data() { BucketName = s3Data.Name };
                _logger.LogInformation($"Bucket {project.S3.BucketName} created");
            }

            if (_chat.Enabled)
            {
                _logger.LogDebug("Begin the call to chatprovider");
                var chatData = await _chatClient.CreateAreaAsync(new RocketChatChatProviderApiClient.Model.CreateAreaRequest(preferredName));
                project.Chat = new ChatData(chatData.Id, chatData.Name);
                _logger.LogInformation($"Channel created named {chatData.Name}");
            }

            string propPath = Path.Combine(projectPath, "properties.json");

            if (await CreateBranch(project, "", defaultBranchName, description, true) != null)
            {
                _logger.LogInformation($"Default Balsam branch {defaultBranchName} created");
            }

            _hubRepositoryClient.PullChanges();
            // serialize JSON to a string and then write string to a file
            await File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(project));

            await CreateProjectManifests(project, projectPath);
            _hubRepositoryClient.PersistChanges($"New program with id {project.Id}");
            _logger.LogInformation($"Project {project.Name}({project.Id}) created");
            return project;
        }

        public async Task<BalsamWorkspace?> CreateWorkspace(string projectId, string branchId, string name, string templateId,  string userName, string userMail)
        {
            var branchPath = Path.Combine(_hubRepositoryClient.Path, "hub", projectId, branchId);

            if (!System.IO.Directory.Exists(branchPath))
            {
                return null;
            }

            var workspace = new BalsamWorkspace(CreateWorkspaceId(name), name, templateId);

            var workspacePath = Path.Combine(branchPath, userName, workspace.Id);

            DirectoryUtil.AssureDirectoryExists(workspacePath);


            var project = await GetProject(projectId);
            var branch = project.Branches.FirstOrDefault(b => b.Id == branchId);
            string gitPAT = "";
            if (_git.Enabled)
            {
                var patResponse = await _gitUserClient.CreatePATAsync(userName);
                gitPAT = patResponse.Token;
            }
            var user = new UserInfo(userName, userMail, gitPAT);

            string propPath = Path.Combine(workspacePath, "properties.json");
            _hubRepositoryClient.PullChanges();
            // serialize JSON to a string and then write string to a file
            await System.IO.File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(workspace));
            await CreateWorkspaceManifests(project, branch, workspace, user, workspacePath, templateId);
            _hubRepositoryClient.PersistChanges($"New workspace with id {project.Id}");

            return workspace;
        }

        public async Task<string> DeleteWorkspace(string projectId, string branchId, string workspaceId, string userName)
        {
            _hubRepositoryClient.PullChanges();
            var branchPath = Path.Combine(_hubRepositoryClient.Path, "hub", projectId, branchId);

            if (!System.IO.Directory.Exists(branchPath))
            {
                return null;
            }

            var workspacePath = Path.Combine(branchPath, userName, workspaceId);

            DirectoryUtil.AssureDirectoryExists(workspacePath);

            if (System.IO.Directory.Exists(workspacePath))
            {
                EmptyDirectory(workspacePath);
                System.IO.Directory.Delete(workspacePath, true);
            }

            _hubRepositoryClient.PersistChanges($"Deleted workspace with id {workspaceId}");
            return "workspace deleted";
        }
        private async Task CreateWorkspaceManifests(BalsamProject project, BalsamBranch branch, BalsamWorkspace workspace, UserInfo user, string workspacePath, string templateId)
        {
            var context = new WorkspaceContext(project, branch, workspace, user);
            await CreateManifests(context, workspacePath, "workspaces" + Path.DirectorySeparatorChar +  templateId);
        }

        private async Task CreateProjectManifests(BalsamProject project, string projectPath)
        {
            var context = new ProjectContext() { Project = project };

            await CreateManifests(context, projectPath, "projects");
        }

        private async Task CreateManifests(BalsamContext context, string destinationPath, string templateName)
        {
            var templatePath = Path.Combine(_hubRepositoryClient.Path, "templates", templateName);

            foreach (var file in Directory.GetFiles(templatePath, "*.yaml"))
            {
                var source = await System.IO.File.ReadAllTextAsync(file);

                var template = Handlebars.Compile(source);

                var result = template(context);

                var destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(file));

                await System.IO.File.WriteAllTextAsync(destinationFilePath, result);
            }

        }

        public async Task<BalsamBranch?> CreateBranch(string projectId, string fromBranch, string branchName, string description)
        {

            var project = await GetProject(projectId, false);
     
            if (project is null || project.Git is null)
            {
                return null;
            }
            

            var response = await _repositoryApi.CreateBranchAsync(project.Git.Id, new CreateBranchRequest(branchName, fromBranch));
            branchName = response.Name;


            return await CreateBranch(project, branchName, description, false);
        }

        private async Task<BalsamBranch?> CreateBranch(BalsamProject project, string branchName, string description, bool isDefault = false)
        {
            var branchId = SanitizeName(branchName);
            var branchPath = Path.Combine(_hubRepositoryClient.Path, "hub", project.Id, branchId);

            DirectoryUtil.AssureDirectoryExists(branchPath);

            if (project.S3 is null || string.IsNullOrEmpty(project.S3.BucketName))
            {
                return null;
            }

            //TODO do we ned to save the response information?
            await _s3Client.CreateFolderAsync(project.S3.BucketName, new CreateFolderRequest(branchName));
            _logger.LogInformation($"Folder {branchName} created in bucket {project.S3.BucketName}.");

            var branch = new BalsamBranch()
            {
                Id = branchId,
                Name = branchName,
                Description = description,
                IsDefault = isDefault,
                GitBranch = branchName
            };

            var propPath = Path.Combine(branchPath, "properties.json");
            await File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(branch));

            return branch;
        }

        public async Task<List<GitProviderApiClient.Model.RepoFile>?> GetGitBranchFiles(string projectId, string branchId)
        {
            var project = await GetProject(projectId);
            var branch = await GetBranch(projectId, branchId);
            if (project is null || branch is null || project.Git is null)
            {
                return null;
            }
            return _repositoryApi.GetFilesInBranch(project.Git.Id, branch.Name);
        }

        public async Task<FileContentResult?> GetFile(string projectId, string branchId, string fileId)
        {
            var project = await GetProject(projectId);
            var branch = await GetBranch(projectId, branchId);
            if (project is null || branch is null || project.Git is null)
            {
                return null;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_git.ServiceLocation}/repos/{project.Git.Id}/branches/{branch.GitBranch}/files/{fileId}");

                var httpClient = new HttpClient();

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
                    return new FileContentResult(data, response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not read files from repository");
            }

            return null;
        }


        private string SanitizeName(string name)
        {
            var crc32 = new Crc32();

            crc32.Append(System.Text.Encoding.ASCII.GetBytes(name));
            var hash = crc32.GetCurrentHash();
            var crcHash = string.Join("", hash.Select(b => b.ToString("x2").ToLower()).Reverse());

            name = name.ToLower(); //Only lower charachters allowed
            name = name.Replace(" ", "-"); //replaces spaches with hypen
            name = Regex.Replace(name, @"[^a-z0-9\-]", ""); // make sure that only a-z or digit or hypen removes all other characters
            name = name.Substring(0, Math.Min(50 - crcHash.Length, name.Length)) + "-" + crcHash; //Assures max size of 50 characters

            return name;

        }

        private static void EmptyDirectory(string directory)
        {
            var di = new DirectoryInfo(directory);
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private static string CreateWorkspaceId(string name)
        {
            var crc32 = new Crc32();

            crc32.Append(System.Text.Encoding.ASCII.GetBytes(name + Guid.NewGuid().ToString()));
            var hash = crc32.GetCurrentHash();
            var crcHash = string.Join("", hash.Select(b => b.ToString("x2").ToLower()).Reverse());

            name = name.ToLower(); //Only lower charachters allowed
            name = name.Replace(" ", "-"); //replaces spaches with hypen
            name = Regex.Replace(name, @"[^a-z0-9\-]", ""); // make sure that only a-z or digit or hypen removes all other characters
            name = name.Substring(0, Math.Min(50 - crcHash.Length, name.Length)) + "-" + crcHash; //Assures max size of 50 characters

            return name;
        }

        public IEnumerable<WorkspaceTemplate> ListWorkspaceTemplates()
        {
            _logger.LogDebug("Start ListWorkspaceTemplates");
            var workspaceTemplatePath = Path.Combine(_hubRepositoryClient.Path, "templates", "workspaces");

            var workspaceTemplates = new List<WorkspaceTemplate>();

            if (!Directory.Exists(workspaceTemplatePath))
            {
                _logger.LogInformation("No workspace template folder found!");
                return workspaceTemplates;
            }

            foreach (var directory in Directory.GetDirectories(workspaceTemplatePath))
            {
                var id = new DirectoryInfo(directory).Name;
                var fileName = Path.Combine(directory, "properties.json");
                var jsonString = File.ReadAllText(fileName);
                var template = JsonConvert.DeserializeObject<WorkspaceTemplate>(jsonString);

                if (template == null) continue;

                template.Id = id;
                workspaceTemplates.Add(template);
            }

            _logger.LogDebug("End ListWorkspaceTemplates");
            return workspaceTemplates;
        }
    }
}
