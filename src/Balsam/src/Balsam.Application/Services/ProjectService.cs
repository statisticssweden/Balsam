using Balsam.Repositories;
using Balsam.Model;
using Microsoft.Extensions.Logging;
using S3ProviderApiClient.Api; //TODO: Split up generation of model and implementation and reference model.
using OidcProviderApiClient.Api;
using GitProviderApiClient.Api;
using RocketChatChatProviderApiClient.Api;
using Balsam.Interfaces;
using GitProviderApiClient.Model;
using Microsoft.Extensions.Options;
using OidcProviderApiClient.Model;
using S3ProviderApiClient.Model;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Balsam.Application.Extensions;

namespace Balsam.Services
{
    public class ProjectService : IProjectService
    {
        private readonly CapabilityOptions _git;
        private readonly CapabilityOptions _s3;
        private readonly CapabilityOptions _authentication;
        private readonly CapabilityOptions _chat;
        private readonly IBucketApi _s3Client;
        private readonly IGroupApi _oidcClient; //TODO: Rename IGroupAPI
        private readonly IRepositoryApi _repositoryApi;
        private readonly ILogger<ProjectService> _logger;
        private readonly IAreaApi _chatClient; //TODO: Rename IAreaApi
        private readonly IProjectRepository _projectRepository; 
        private readonly IProjectGitOpsRepository _projectGitOpsRepository;
        private readonly IKnowledgeLibraryService _knowledgeLibraryService;

        public ProjectService(ILogger<ProjectService> logger,
                            IOptionsSnapshot<CapabilityOptions> capabilityOptions,
                            IBucketApi s3Client,
                            IRepositoryApi reposiotryApi,
                            IGroupApi oidcClient,
                            IAreaApi chatClient,
                            IProjectRepository projectRepository,
                            IProjectGitOpsRepository projectGitOpsRepository,
                            IKnowledgeLibraryService knowledgeLibraryService)
        {
            _logger = logger;
            _s3Client = s3Client;
            _oidcClient = oidcClient; 
            _chatClient = chatClient;
            _projectRepository = projectRepository;
            _projectGitOpsRepository = projectGitOpsRepository;
            _knowledgeLibraryService = knowledgeLibraryService;

            _git = capabilityOptions.Get(Capabilities.Git);
            _s3 = capabilityOptions.Get(Capabilities.S3);
            _authentication = capabilityOptions.Get(Capabilities.Authentication);
            _chat = capabilityOptions.Get(Capabilities.Chat);
            _repositoryApi = reposiotryApi;

        }

        static ProjectService()
        {

        }

        public async Task<List<BalsamProject>> GetProjects(bool includeBranches = true)
        {
            return await _projectRepository.GetProjects(includeBranches);
        }

        public async Task<BalsamProject?> GetProject(string projectId, bool includeBranches = true)
        {
            return await _projectRepository.GetProject(projectId, includeBranches);
        }

        public async Task<BalsamBranch?> GetBranch(string projectId, string branchId)
        {
            return await _projectRepository.GetBranch(projectId, branchId);
        }

        public async Task<bool> ProjectExists(string preferredName)
        {
            return await _projectRepository.ProjectExists(preferredName);
        }

        public async Task<BalsamProject?> CreateProject(string preferredName, string description, string defaultBranchName, string username, string? sourceLocation = null)
        {
            //Check if there is a program with the same name.

            _logger.LogDebug("Check for duplicate names");
            if (await _projectRepository.ProjectExists(preferredName))
            {
                string errorMessage = $"Could not create project {preferredName}, due to name duplication";
                _logger.LogInformation(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            _logger.LogDebug($"create project information");
            var project = new BalsamProject(preferredName, description);

            project = await _projectRepository.CreateProject(project, defaultBranchName);

            _logger.LogDebug($"Begin call service providers");

            if (_authentication.Enabled)
            {
                _logger.LogDebug($"Begin call OpenIdConnect");
                var oidcData = await _oidcClient.CreateGroupAsync(new CreateGroupRequest(project.Id));
                await _oidcClient.AddUserToGroupAsync(oidcData.Id, new AddUserToGroupRequest(username));
                project.Oidc = new OidcData(oidcData.Id, oidcData.Name);
                _logger.LogInformation($"Group {project.Oidc.GroupName}({project.Oidc.GroupId}) created");

                if (project.Oidc == null)
                {
                    throw new Exception("Could not parse oidc data");
                }
            }

            if (_git.Enabled)
            {
                _logger.LogDebug($"Begin call Git");
                var gitData = await _repositoryApi.CreateRepositoryAsync(new CreateRepositoryRequest(preferredName, description, defaultBranchName));
                //defaultBranchName = gitData.DefaultBranchName;
                project.Git = new GitData() { Id = gitData.Id, Name = gitData.Name, Path = gitData.Path, SourceLocation = sourceLocation };
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

            await _projectRepository.UpdateProject(project);

            await _projectGitOpsRepository.CreateProjectManifests(project);

            _logger.LogInformation($"Project {project.Name}({project.Id}) created");
            return project;
        }

        public async Task<BalsamBranch?> CreateBranch(string projectId, string fromBranchId, string branchName, string description)
        {
            var project = await GetProject(projectId);
            var fromBranch = await GetBranch(projectId, fromBranchId);

            if (fromBranch is null)
            {
                var errorMessage = $"Project with id {projectId} does not have a branch with id {fromBranchId}";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, fromBranchId);
            }

            if (project is null)
            {
                var errorMessage = $"Project with id {projectId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, projectId);
            }

            if (project.Git is null)
            {
                var errorMessage = $"Git-information not set for project with id {projectId}";
                _logger.LogError(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            if (_git.Enabled)
            {
                var response = await _repositoryApi.CreateBranchAsync(project.Git.Id, new CreateBranchRequest(branchName, fromBranch.GitBranch));
                branchName = response.Name;
            }

            return await _projectRepository.CreateBranch(projectId, branchName, description);
        }

        public async Task<List<BalsamRepoFile>?> GetGitBranchFiles(string projectId, string branchId)
        {
            var project = await GetProject(projectId);

            if(!_git.Enabled)
            {
                return new List<BalsamRepoFile>();
            }

            if (project == null)
            {
                var errorMessage = $"Project with project id {projectId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            var branch = await GetBranch(projectId, branchId);

            if (branch == null)
            {
                var errorMessage = $"Branch with branch id {branchId} in project {projectId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            if (project.Git is null)
            {
                var errorMessage = $"Git definition missing for project {projectId}";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }


            return (await _repositoryApi.GetFilesInBranchAsync(project.Git.Id, branch.GitBranch))
                                    .Select(f => f.ToBalsamRepoFile())
                                    .ToList();
        }

        public async Task<FileContent> GetFile(string projectId, string branchId, string fileId)
        {
            //TODO: Use GitProvider instead...
            return await _projectRepository.GetFile(projectId, branchId, fileId);
        }

        public async Task DeleteBranch(string projectId, string branchId)
        {
            //var branchPath = Path.Combine(_hubRepositoryClient.Path, "hub", projectId, branchId);
            _logger.LogDebug($"Deleting branch {branchId} for project {projectId}");


            if (_git.Enabled)
            {
                var project = await GetProject(projectId);
                var branch = await GetBranch(projectId, branchId);

                //Asure that the id are correct
                if (project == null || branch == null) return;

                try
                {
                    _logger.LogDebug($"Deleting branch in git");
                    await _repositoryApi.DeleteRepositoryBranchAsync(project.Git?.Id ?? "", branch.GitBranch);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not delete git branch");
                }
            }

            _logger.LogDebug($"Deleting branch information");
            await _projectRepository.DeleteBranch(projectId, branchId);
            _logger.LogDebug($"Deleting branch manifests");

            await _projectGitOpsRepository.DeleteBranchManifests(projectId, branchId);

            _logger.LogInformation($"Deleted branch {branchId} for project {projectId}");
        }

        public async Task DeleteProject(string projectId)
        {
            var project = await GetProject(projectId);

            if (project == null)
            {
                return;
            }
                
            //TODO: Delete oidc group?

            if (_authentication.Enabled)
            {
                try
                {
                    await _oidcClient.DeleteGroupAsync(project.Oidc?.GroupId ?? "");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not delete oidc group");
                }
            }

            if (_git.Enabled)
            {
                try
                {
                    await _repositoryApi.DeleteRepositoryAsync(project.Git?.Id ?? "");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not delete git repository");
                }
            }

            if (_s3.Enabled)
            {
                try
                {
                    await _s3Client.DeleteBucketAsync(project.S3?.BucketName ?? "");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not delete s3 bucket");
                }
            }

            await _projectRepository.DeleteProject(projectId);
            await _projectGitOpsRepository.DeleteProjectManifests(projectId);

            _logger.LogInformation($"Project {projectId} deleted");
        }

        public async Task CopyFromKnowledgeLibrary(string projectId, string branchId,  string knowledgeLibraryId, string fileId)
        {
            var project = await GetProject(projectId);
            
            if (project is null)
            {
                var errorMessage = $"Project with id {projectId} does not exist";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, projectId);
            }

            if (project.Git is null)
            {
                var errorMessage = $"Git-information not set for project with id {projectId}";
                _logger.LogError(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            var branch = project.Branches.First(b => b.Id == branchId);
            var knowledgeLibrary = await _knowledgeLibraryService.GetKnowledgeLibrary(knowledgeLibraryId);
            var zipFile = await _knowledgeLibraryService.GetZippedResource(knowledgeLibraryId, knowledgeLibrary.RepositoryUrl, fileId);

            try
            {
                using var stream = System.IO.File.OpenRead(zipFile);
                await _repositoryApi.AddResourceFilesAsync(project.Git.Id, branch.GitBranch, stream);
            }
            finally
            {
                System.IO.File.Delete(zipFile);
            }
            
        }
    }
}
