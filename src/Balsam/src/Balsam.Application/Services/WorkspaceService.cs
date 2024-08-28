using Balsam.Interfaces;
using Balsam.Model;
using Balsam.Repositories;
using GitProviderApiClient.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Model;

namespace Balsam.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly CapabilityOptions _git;
        private readonly CapabilityOptions _s3;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ILogger<WorkspaceService> _logger;
        private readonly IBucketApi _s3Client;
        private readonly IUserApi _gitUserClient;
        private readonly IProjectRepository _projectRepository; //TODO: User ProjectService instead?
        private readonly IWorkspaceGitOpsRepository _workspaceGitOpsService; //TODO: Make and use interface

        public WorkspaceService(IOptionsSnapshot<CapabilityOptions> capabilityOptions,
                                IWorkspaceRepository workspaceRepository,
                                ILogger<WorkspaceService> logger,
                                IBucketApi s3Client,
                                IWorkspaceGitOpsRepository workspaceGitOpsService,
                                IUserApi gitUserClient,
                                IProjectRepository projectRepository)
        {
            _git = capabilityOptions.Get(Capabilities.Git);
            _s3 = capabilityOptions.Get(Capabilities.S3);    
            _workspaceRepository = workspaceRepository;
            _logger = logger;
            _s3Client = s3Client;
            _workspaceGitOpsService = workspaceGitOpsService;
            _gitUserClient = gitUserClient;
            _projectRepository = projectRepository;
        }


        public async Task<BalsamWorkspace?> CreateWorkspace(string projectId, string branchId, string name, string templateId, string userName, string userMail)
        {
            var workspaceId = _workspaceRepository.GenerateWorkspaceId(name);
            var workspace = new BalsamWorkspace(workspaceId, name, templateId, projectId, branchId, userName);

            var template = await _workspaceRepository.GetWorkspaceTemplate(workspace.TemplateId);

            if (template != null)
            {
                workspace.Url = await _workspaceGitOpsService.GetWorkspaceUrl(projectId, branchId, userName, workspaceId, template.UrlConfig);
                _logger.LogDebug("Workspace url: " + workspace.Url);
                _logger.LogDebug("Template config file: " + template.UrlConfig);
            }
            else
            {
                var errorMessage = $"Template with id {templateId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            var createdWorkspace = await _workspaceRepository.CreateWorkspace(workspace);

            
            await CreateWorkspaceManifests(projectId, branchId, templateId, userName, userMail, createdWorkspace);

            _logger.LogInformation("Workspace created");
            return workspace;
        }

        private async Task CreateWorkspaceManifests(string projectId, string branchId, string templateId, string userName, string userMail, BalsamWorkspace workspace)
        {
            var project = await _projectRepository.GetProject(projectId);

            if (project == null)
            {
                var errorMessage = $"Project with project id {projectId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            var gitPAT = string.Empty;

            if (_git.Enabled)
            {
                var patResponse = await _gitUserClient.CreatePATAsync(userName);
                gitPAT = patResponse.Token;
                _logger.LogInformation($"Git PAT created");
            }

            var user = new UserInfo(userName, userMail, gitPAT);

            var token = new AccessKeyCreatedResponse("", "");
            if (_s3.Enabled && project.S3 != null)
            {
                token = await _s3Client.CreateAccessKeyAsync(project.S3.BucketName);

                var s3Token = new S3Token(token.AccessKey, token.SecretKey);
                user.S3 = s3Token;
            }

            _logger.LogDebug($"Copying manifests");

            var branch = project.Branches.FirstOrDefault(b => b.Id == branchId);

            if (branch != null)
            {
                await _workspaceGitOpsService.CreateWorkspaceManifests(project, branch, workspace, user, templateId);
            }
            else
            {
                var errorMessage = $"Branch with branch id {branchId} in project {projectId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            _logger.LogInformation("Manifests persisted");
        }

        public async Task DeleteWorkspace(string projectId, string branchId, string workspaceId, string userName)
        {
            await _workspaceRepository.DeleteWorkspace(projectId, branchId, workspaceId, userName);

            await _workspaceGitOpsService.DeleteWorkspaceManifests(projectId, branchId, workspaceId, userName);

        }

        public async Task<List<WorkspaceTemplate>> ListWorkspaceTemplates()
        {
            _logger.LogDebug("Start ListWorkspaceTemplates");

            var workspaceTemplates = await _workspaceRepository.ListWorkspaceTemplates();

            if (workspaceTemplates.Count() == 0)
            {
                _logger.LogInformation("No workspace template folder found!");

            }
            _logger.LogDebug("End ListWorkspaceTemplates");

            return workspaceTemplates;
        }

        public async Task<List<BalsamWorkspace>> GetWorkspaces()
        {
            return await _workspaceRepository.GetWorkspaces();
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByUser(string userId, List<string> projectIds)
        {
            return await _workspaceRepository.GetWorkspacesByUser(userId, projectIds);
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByProject(string projectId)
        {
            return await _workspaceRepository.GetWorkspacesByProject(projectId);
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByProjectAndBranch(string projectId, string branchId)
        {
            return await _workspaceRepository.GetWorkspacesByProjectAndBranch(projectId, branchId);
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByProjectAndUser(string projectId, string userId)
        {
            return await _workspaceRepository.GetWorkspacesByProjectAndUser(projectId, userId);
        }

        public async Task<List<BalsamWorkspace>> GetWorkspacesByProjectBranchAndUser(string projectId, string branchId, string userId)
        {
            return await _workspaceRepository.GetWorkspacesByProjectBranchAndUser(projectId, branchId, userId);
        }

        public async Task<BalsamWorkspace?> GetWorkspace(string projectId, string userId, string workspaceId)
        {
            return await _workspaceRepository.GetWorkspace(projectId, userId, workspaceId);
        }
    }
}
