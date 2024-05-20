using Balsam.Utility;
using Newtonsoft.Json;
using Balsam.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;

namespace Balsam.Repositories
{

    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly IHubRepositoryClient _hubRepositoryClient;
        private readonly ILogger<WorkspaceRepository> _logger;
        protected HubPaths HubPaths { get; set; }

        public WorkspaceRepository(IHubRepositoryClient hubRepositoryClient, ILogger<WorkspaceRepository> logger)
        {
            _hubRepositoryClient = hubRepositoryClient;
            _logger = logger;
            HubPaths = new HubPaths(_hubRepositoryClient.Path);
        }

        public async Task<BalsamWorkspace?> CreateWorkspace(BalsamWorkspace workspace)
        {
            var projectPath = HubPaths.GetProjectPath(workspace.ProjectId);

            if (!Directory.Exists(projectPath))
            {
                DirectoryUtil.AssureDirectoryExists(projectPath);
            }

            //var workspacePath = Path.Combine(projectPath, "workspaces", workspace.Id);
            var workspacePath = HubPaths.GetWorkspacePath(workspace.ProjectId, workspace.BranchId, workspace.Owner, workspace.Id);

            //var user = new UserInfo(userName, userMail, gitPAT);
            var propPath = Path.Combine(workspacePath, "properties.json");

            _logger.LogDebug("Pulling changes");
            _hubRepositoryClient.PullChanges();
            DirectoryUtil.AssureDirectoryExists(workspacePath);
            // serialize JSON to a string and then write string to a file
            await File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(workspace));
            _hubRepositoryClient.PersistChanges($"New workspace with id {workspace.Id}");

            _logger.LogInformation("Workspace created");
            return workspace;
        }

        public async Task<WorkspaceTemplate?> GetWorkspaceTemplate(string templateId)
        {
            //var templatesPaths = HubPaths.GetTemplatesPath();
            var workspaceTemplatePath = HubPaths.GetWorkspaceTemplatesPath(templateId);

            var templateFilePath = Path.Combine(workspaceTemplatePath, "properties.json");

            //TODO: Pull?

            if (!File.Exists(templateFilePath))
            {
                return null;
            }

            var content = await File.ReadAllTextAsync(templateFilePath);

            return JsonConvert.DeserializeObject<WorkspaceTemplate>(content);
        }

        public async Task DeleteWorkspace(string projectId, string branchId, string workspaceId, string userName)
        {
            _hubRepositoryClient.PullChanges();
            //var workspacePath = Path.Combine(_hubRepositoryClient.Path, "hub", projectId, branchId, userName, workspaceId);
            var workspacePath = HubPaths.GetWorkspacePath(projectId, branchId, userName, workspaceId);

            var propPath = Path.Combine(workspacePath, "properties.json");

            if (!Directory.Exists(workspacePath))
            {
                return;
            }

            if (File.Exists(propPath))
            {
                await Task.Run(() => File.Delete(propPath));
            }

            _hubRepositoryClient.PersistChanges($"Deleted workspace with id {workspaceId}");
        }
        public async Task<BalsamWorkspace?> GetWorkspace(string projectId, string userId, string workspaceId)
        {
            var workspaces = await GetWorkspacesByProjectAndUser(projectId, userId);

            return workspaces.FirstOrDefault(w => w.Id == workspaceId);
        }

        public string GenerateWorkspaceId(string name)
        {
            return NameUtil.SanitizeName(name);
        }

        public async Task<List<WorkspaceTemplate>> ListWorkspaceTemplates()
        {
            var workspaceTemplatesPath = HubPaths.GetWorkspacesTemplatesPath();

            if (!Directory.Exists(workspaceTemplatesPath))
            {
                return new List<WorkspaceTemplate>();
            }

            var workspaceTemplates = await Task.Run(() =>
            {
                var workspaceTemplates = new List<WorkspaceTemplate>();
                foreach (var directory in Directory.GetDirectories(workspaceTemplatesPath))
                {
                    var id = new DirectoryInfo(directory).Name;
                    var fileName = Path.Combine(directory, "properties.json");
                    var jsonString = File.ReadAllText(fileName);
                    var template = JsonConvert.DeserializeObject<WorkspaceTemplate>(jsonString);

                    if (template == null) continue;

                    template.Id = id;
                    workspaceTemplates.Add(template);
                }
                return workspaceTemplates;
            });
            

            return workspaceTemplates;
        }

        public async Task<List<BalsamWorkspace>> GetWorkspaces()
        {
            var hubPath = HubPaths.GetHubPath();

            var workspaces = new List<BalsamWorkspace>();

            //TODO: Gör oberoende av katalogstruktur
            foreach (var projectPath in Directory.GetDirectories(hubPath))
            {
                foreach (var branchPath in Directory.GetDirectories(projectPath))
                {
                    foreach (var userPath in Directory.GetDirectories(branchPath))
                    {
                        foreach (var workspacePath in Directory.GetDirectories(userPath))
                        {
                            var propsFile = Path.Combine(workspacePath, "properties.json");

                            if (File.Exists(propsFile))
                            {
                                var content = await File.ReadAllTextAsync(propsFile);
                                var workspace = JsonConvert.DeserializeObject<BalsamWorkspace>(content);

                                if (workspace != null)
                                {
                                    workspaces.Add(workspace);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            return workspaces;
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByUser(string userId, List<string> projectIds)
        {
            var workspaces = new List<BalsamWorkspace>();

            foreach (var projectId in projectIds)
            {
                var projectPath = HubPaths.GetProjectPath(projectId);

                //TODO: Gör oberoende av katalogstruktur?
                if (Directory.Exists(projectPath))
                {
                    foreach (var branchPath in Directory.GetDirectories(projectPath))
                    {
                        var userPath = Path.Combine(branchPath, userId);

                        if (Directory.Exists(userPath))
                        {
                            foreach (var workspacePath in Directory.GetDirectories(userPath))
                            {
                                var propsFile = Path.Combine(workspacePath, "properties.json");
                                var workspace = JsonConvert.DeserializeObject<BalsamWorkspace>(await File.ReadAllTextAsync(propsFile));

                                if (workspace == null) continue;
                                workspaces.Add(workspace);
                            }
                        }
                    }
                }
            }
            return workspaces;
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByProject(string projectId)
        {

            var workspaces = new List<BalsamWorkspace>();

            var projectPath = HubPaths.GetProjectPath(projectId);
            //TODO: Gör oberoende av katalogstruktur
            if (Directory.Exists(projectPath))
            {
                foreach (var branchPath in Directory.GetDirectories(projectPath))
                {
                    foreach (var userPath in Directory.GetDirectories(branchPath))
                    {
                        foreach (var workspacePath in Directory.GetDirectories(userPath))
                        {
                            var propsFile = Path.Combine(workspacePath, "properties.json");
                            var workspace = JsonConvert.DeserializeObject<BalsamWorkspace>(await File.ReadAllTextAsync(propsFile));

                            if (workspace == null) continue;
                            workspaces.Add(workspace);
                        }
                    }
                }
            }
            return workspaces;
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByProjectAndBranch(string projectId, string branchId)
        {
            //var hubPath = Path.Combine(_hubRepositoryClient.Path, "hub");

            var workspaces = new List<BalsamWorkspace>();

            var branchPath = HubPaths.GetBranchPath(projectId, branchId);

            if (Directory.Exists(branchPath))
            {
                foreach (var userPath in Directory.GetDirectories(branchPath))
                {
                    foreach (var workspacePath in Directory.GetDirectories(userPath))
                    {
                        var propsFile = Path.Combine(workspacePath, "properties.json");
                        var workspace = JsonConvert.DeserializeObject<BalsamWorkspace>(await File.ReadAllTextAsync(propsFile));

                        if (workspace == null) continue;
                        workspaces.Add(workspace);
                    }
                }
            }
            return workspaces;
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByProjectAndUser(string projectId, string userId)
        {
            //var hubPath = Path.Combine(_hubRepositoryClient.Path, "hub");

            var workspaces = new List<BalsamWorkspace>();

            //var projectPath = Path.Combine(hubPath, projectId);
            var projectPath = HubPaths.GetProjectPath(projectId);

            if (Directory.Exists(projectPath))
            {
                foreach (var branchPath in Directory.GetDirectories(projectPath))
                {
                    var userPath = Path.Combine(branchPath, userId);
                    if (Directory.Exists(userPath))
                    {
                        foreach (var workspacePath in Directory.GetDirectories(userPath))
                        {
                            var propsFile = Path.Combine(workspacePath, "properties.json");
                            var workspace = JsonConvert.DeserializeObject<BalsamWorkspace>(await File.ReadAllTextAsync(propsFile));

                            if (workspace == null) continue;
                            workspaces.Add(workspace);
                        }
                    }
                }
            }
            return workspaces;
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByProjectBranchAndUser(string projectId, string branchId, string userId)
        {
            var hubPath = HubPaths.GetHubPath();

            var workspaces = new List<BalsamWorkspace>();

            var userPath = Path.Combine(hubPath, projectId, branchId, userId);

            if (Directory.Exists(userPath))
            {
                foreach (var workspacePath in Directory.GetDirectories(userPath))
                {
                    var propsFile = Path.Combine(workspacePath, "properties.json");
                    var workspace = JsonConvert.DeserializeObject<BalsamWorkspace>(await File.ReadAllTextAsync(propsFile));

                    if (workspace == null) continue;
                    workspaces.Add(workspace);
                }
            }
            return workspaces;
        }
    }
}
