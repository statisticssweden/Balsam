using Balsam.Interfaces;
using Balsam.Model;
using Balsam.Repositories;
using Balsam.Services;
using Balsam.Tests.Helpers;
using GitProviderApiClient.Api;
using GitProviderApiClient.Model;
using Microsoft.Extensions.Logging;

//using Castle.Core.Logging;
using Moq;
using OidcProviderApiClient.Api;
using OidcProviderApiClient.Model;
using RocketChatChatProviderApiClient.Api;
using RocketChatChatProviderApiClient.Model;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Model;
using CreateBranchRequest = GitProviderApiClient.Model.CreateBranchRequest;

namespace Balsam.Tests.Application;

public class Test_ProjectService
{
    [Theory]
    [InlineData(true, false, false, false)]
    [InlineData(false, true, true, false)] //atuthentication must be enabled for S3 to work
    [InlineData(false, false, true, false)]
    [InlineData(false, false, false, true)]
    public async void Test_CreateProject(bool gitEnabled, bool s3Enabled, bool authenticationEnabled, bool chatEnabled)
    {
        var loggerMock = new Mock<ILogger<ProjectService>>();


        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(gitEnabled, s3Enabled, authenticationEnabled, chatEnabled);


        var bucketApiMock = new Mock<IBucketApi>();

        var s3BucketName = "givenBucketName";
        bucketApiMock.Setup(m => m.CreateBucketAsync(It.IsAny<CreateBucketRequest?>(),
                                                        It.IsAny<int>(),
                                                        It.IsAny<CancellationToken>()))
                            .ReturnsAsync((CreateBucketRequest request, int operationIndex, CancellationToken cancellationToken)
                                                => new BucketCreatedResponse(request.Name, s3BucketName));

        var repositoryApiMock = new Mock<IRepositoryApi>();
        var gitId = "456";
        var gitName = "givenGitName";
        var gitPath = "path/to/repository";
        var gitDefaultBranch = "givenDefaultBranch";
        repositoryApiMock.Setup(m => m.CreateRepositoryAsync(It.IsAny<CreateRepositoryRequest?>(),
                                                                It.IsAny<int>(),
                                                                It.IsAny<CancellationToken>()))
                            .ReturnsAsync((CreateRepositoryRequest request, int operationIndex, CancellationToken cancellationToken)
                                                            => new RepositoryCreatedResponse(gitId,
                                                                    request.Name,
                                                                    gitName,
                                                                    gitPath,
                                                                    gitDefaultBranch));



        var groupApiMock = new Mock<IGroupApi>();

        var groupName = "GivenGroupName";
        var groupId = "123";
        groupApiMock.Setup(m => m.CreateGroupAsync(It.IsAny<CreateGroupRequest?>(),
                                                It.IsAny<int>(),
                                                It.IsAny<CancellationToken>()))
                        .ReturnsAsync((CreateGroupRequest request, int operationIndex, CancellationToken cancellationToken)
                                        => new GroupCreatedResponse(groupName, groupId));


        var areaApiMock = new Mock<IAreaApi>();
        var chatId = "987";
        var chatName = "GivenChatName";

        areaApiMock.Setup(m => m.CreateAreaAsync(It.IsAny<CreateAreaRequest?>(),
                                                It.IsAny<int>(),
                                                It.IsAny<CancellationToken>()))
                    .ReturnsAsync((CreateAreaRequest request, int operationIndex, CancellationToken cancellationToken)
                                    => new AreaCreatedResponse(chatName, chatId));

        var projectRepositoryMock = new Mock<IProjectRepository>();
        var projectId = "646";
        projectRepositoryMock.Setup(m => m.CreateProject(It.IsAny<BalsamProject>(), It.IsAny<string>()))
                                .ReturnsAsync((BalsamProject balsamProject, string defaultBranchName) =>
                                {
                                    balsamProject.Id = projectId;
                                    return balsamProject;
                                });



        var projectGitOpsRepositoryMock = new Mock<IProjectGitOpsRepository>();
        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();


        var projectService = new ProjectService(loggerMock.Object,
                                            capabilityOptionsSnapshotMock.Object,
                                            bucketApiMock.Object,
                                            repositoryApiMock.Object,
                                            groupApiMock.Object,
                                            areaApiMock.Object,
                                            projectRepositoryMock.Object,
                                            projectGitOpsRepositoryMock.Object,
                                            knowledgeLibraryServiceMock.Object);

        var projectName = "Project";
        var defaultBranchName = "defaultBranchName";
        var projectDescription = "My test project";

        var project = await projectService.CreateProject(projectName, projectDescription, defaultBranchName, "userName", null);

        Assert.NotNull(project);
        Assert.Equal(projectId, project.Id);
        Assert.Equal(projectName, project.Name);
        Assert.Equal(projectDescription, project.Description);



        //Assert OIDC
        if (authenticationEnabled)
        {
            Assert.NotNull(project.Oidc);
            Assert.Equal(groupName, project.Oidc.GroupName);
            Assert.Equal(groupId, project.Oidc.GroupId);
        }

        Times authenticationExpectedCallTimes = authenticationEnabled ? Times.Once() : Times.Never();

        groupApiMock.Verify(m => m.AddUserToGroupAsync(It.IsAny<string>(),
                                                    It.IsAny<AddUserToGroupRequest?>(),
                                                    It.IsAny<int>(),
                                                    It.IsAny<CancellationToken>()),
                                                    authenticationExpectedCallTimes);

        //Assert S3

        if (s3Enabled)
        {
            Assert.NotNull(project.S3);
            Assert.Equal(s3BucketName, project.S3.BucketName);
        }

        Times s3ExpectedCallTimes = s3Enabled ? Times.Once() : Times.Never();

        bucketApiMock.Verify(m => m.CreateBucketAsync(It.Is((CreateBucketRequest? request) => request != null),
                                                        It.IsAny<int>(),
                                                        It.IsAny<CancellationToken>()),
                                                        s3ExpectedCallTimes);


        //Assert Git
        if (gitEnabled)
        {
            Assert.NotNull(project.Git);
            Assert.Equal(gitId, project.Git.Id);
            Assert.Equal(gitName, project.Git.Name);
            Assert.Equal(gitPath, project.Git.Path);


        }

        Times gitExpectedCallTimes = gitEnabled ? Times.Once() : Times.Never();

        repositoryApiMock.Verify(m => m.CreateRepositoryAsync(It.Is((CreateRepositoryRequest? request) => request != null),
                                                            It.IsAny<int>(),
                                                            It.IsAny<CancellationToken>()),
                                                            gitExpectedCallTimes);


        //Assert Chat (AreaApi)
        if (chatEnabled)
        {
            Assert.NotNull(project.Chat);
            Assert.Equal(chatId, project.Chat.Id);
            Assert.Equal(chatName, project.Chat.Name);
        }

        Times chatExpectedCallTimes = chatEnabled ? Times.Once() : Times.Never();

        areaApiMock.Verify(m => m.CreateAreaAsync(It.Is((CreateAreaRequest? request) => request != null),
                                                        It.IsAny<int>(),
                                                        It.IsAny<CancellationToken>()),
                                                        chatExpectedCallTimes);

        //Assert projectRepository
        projectRepositoryMock.Verify(m => m.CreateProject(It.Is<BalsamProject>(p => p.Name == project.Name
                                                                                       && p.Description == project.Description),
                                                          It.Is<string>(name => name == defaultBranchName)));


        //Assert GitOpsService
        projectGitOpsRepositoryMock.Verify(m => m.CreateProjectManifests(It.Is<BalsamProject>(p => p.Id == project.Id
                                                                                                   && p.Name == project.Name)));


    }

    [Theory]
    [InlineData(true, false, false, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    [InlineData(false, false, false, true)]

    public async void Test_DeleteProject(bool gitEnabled, bool s3Enabled, bool authenticationEnabled, bool chatEnabled)
    {
        var loggerMock = new Mock<ILogger<ProjectService>>();

        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(gitEnabled, s3Enabled, authenticationEnabled, chatEnabled);

        var bucketApiMock = new Mock<IBucketApi>();
        var repositoryApiMock = new Mock<IRepositoryApi>();
        var groupApiMock = new Mock<IGroupApi>();
        var areaApiMock = new Mock<IAreaApi>();
        var projectRepositoryMock = new Mock<IProjectRepository>();

        var project = TestHelpers.NewBalsamProject("Project");


        projectRepositoryMock.Setup(m => m.GetProject(It.IsAny<string>(), It.IsAny<bool>()))
                                .ReturnsAsync(() => project);


        var projectGitOpsRepositoryMock = new Mock<IProjectGitOpsRepository>();
        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();

        var projectService = new ProjectService(loggerMock.Object,
                                            capabilityOptionsSnapshotMock.Object,
                                            bucketApiMock.Object,
                                            repositoryApiMock.Object,
                                            groupApiMock.Object,
                                            areaApiMock.Object,
                                            projectRepositoryMock.Object,
                                            projectGitOpsRepositoryMock.Object, 
                                            knowledgeLibraryServiceMock.Object);



        await projectService.DeleteProject(project.Id);

        Times gitExpectedCallTimes = gitEnabled ? Times.Once() : Times.Never();

        repositoryApiMock.Verify(m => m.DeleteRepositoryAsync(It.Is<string>(id => id == project.Git.Id),
                                                                It.IsAny<int>(),
                                                                It.IsAny<CancellationToken>()),
                                                            gitExpectedCallTimes);

        Times authenticationExpectedCallTimes = authenticationEnabled ? Times.Once() : Times.Never();

        groupApiMock.Verify(m => m.DeleteGroupAsync(It.Is<string>(groupId => groupId == project.Oidc.GroupId),
                                                                It.IsAny<int>(),
                                                                It.IsAny<CancellationToken>()),
                                                            authenticationExpectedCallTimes);


        Times s3ExpectedCallTimes = s3Enabled ? Times.Once() : Times.Never();

        bucketApiMock.Verify(m => m.DeleteBucketAsync(It.Is<string>(bucketId => bucketId == project.S3.BucketName),
                                                                It.IsAny<int>(),
                                                                It.IsAny<CancellationToken>()),
                                                            s3ExpectedCallTimes);


        projectRepositoryMock.Verify(m => m.DeleteProject(It.Is<string>(id => id == project.Id)));

        projectGitOpsRepositoryMock.Verify(m => m.DeleteProjectManifests(It.Is<string>(id => id == project.Id)));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async void Test_CreateBranch(bool gitEnabled)
    {
        var loggerMock = new Mock<ILogger<ProjectService>>();

        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(gitEnabled, false, false, false);


        var bucketApiMock = new Mock<IBucketApi>();
        var groupApiMock = new Mock<IGroupApi>();
        var areaApiMock = new Mock<IAreaApi>();

        var projectRepositoryMock = new Mock<IProjectRepository>();

        var project = TestHelpers.NewBalsamProject("Project");

        projectRepositoryMock.Setup(m => m.GetProject(It.Is<string>(id => id == project.Id), It.IsAny<bool>()))
                                .ReturnsAsync((string id, bool includeBranches) => project);

        var fromBranch = TestHelpers.NewBalsamBranch("FromBranch", true);

        projectRepositoryMock.Setup(m => m.GetBranch(It.Is<string>(id => id == project.Id),
                                                        It.Is<string>(branchId => branchId == fromBranch.Id)))
                                .ReturnsAsync(() => fromBranch);



        var repositoryApiMock = new Mock<IRepositoryApi>();
        var newBranchName = "NewBranch";
        var newBranchDescription = "NewDescription";

        repositoryApiMock.Setup(m => m.CreateBranchAsync(It.Is<string>(repositoryId => repositoryId == project.Git.Id),
                                                            It.Is<CreateBranchRequest?>(request => request != null
                                                                        && request.Name == newBranchName
                                                                        && request.FromBranch == fromBranch.GitBranch),
                                                            It.IsAny<int>(),
                                                            It.IsAny<CancellationToken>()))
                                                .ReturnsAsync((string repositoryId, CreateBranchRequest request, int operationIndex, CancellationToken cancellationToken) 
                                                        => new GitProviderApiClient.Model.BranchCreatedResponse(project.Git.Id, "123", request.Name));


        var projectGitOpsRepositoryMock = new Mock<IProjectGitOpsRepository>();
        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();

        var projectService = new ProjectService(loggerMock.Object,
                                            capabilityOptionsSnapshotMock.Object,
                                            bucketApiMock.Object,
                                            repositoryApiMock.Object,
                                            groupApiMock.Object,
                                            areaApiMock.Object,
                                            projectRepositoryMock.Object,
                                            projectGitOpsRepositoryMock.Object,
                                            knowledgeLibraryServiceMock.Object);




        await projectService.CreateBranch(project.Id, fromBranch.Id, newBranchName, newBranchDescription);


        projectRepositoryMock.Verify(m => m.CreateBranch(It.Is<string>(id => id == project.Id),
                                                            It.Is<string>(branchName => branchName == newBranchName),
                                                            It.Is<string>(description => description == newBranchDescription)));



        Times gitExpectedCallTimes = gitEnabled ? Times.Once() : Times.Never();

        repositoryApiMock.Verify(m => m.CreateBranchAsync(It.Is<string>(repositoryId => repositoryId == project.Git.Id),
                                                            It.Is((GitProviderApiClient.Model.CreateBranchRequest? request) =>
                                                                                                            request != null
                                                                                                            && request.Name == newBranchName
                                                                                                            && request.FromBranch == fromBranch.GitBranch),
                                                            It.IsAny<int>(),
                                                            It.IsAny<CancellationToken>()),
                                                            gitExpectedCallTimes);


    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async void Test_DeleteBranch(bool gitEnabled)
    {
        var loggerMock = new Mock<ILogger<ProjectService>>();

        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(gitEnabled, false, false, false);

        var bucketApiMock = new Mock<IBucketApi>();
        var groupApiMock = new Mock<IGroupApi>();
        var areaApiMock = new Mock<IAreaApi>();

        var projectRepositoryMock = new Mock<IProjectRepository>();

        var project = TestHelpers.NewBalsamProject("Project");

        projectRepositoryMock.Setup(m => m.GetProject(It.Is<string>(id => id == project.Id), It.IsAny<bool>()))
                                .ReturnsAsync(() => project);

        var branch = TestHelpers.NewBalsamBranch("Branch", false);

        projectRepositoryMock.Setup(m => m.GetBranch(It.Is<string>(id => id == project.Id),
                                                        It.Is<string>(branchId => branchId == branch.Id)))
                                .ReturnsAsync(() => branch);



        var repositoryApiMock = new Mock<IRepositoryApi>();
        var projectGitOpsRepositoryMock = new Mock<IProjectGitOpsRepository>();
        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();


        var projectService = new ProjectService(loggerMock.Object,
                                            capabilityOptionsSnapshotMock.Object,
                                            bucketApiMock.Object,
                                            repositoryApiMock.Object,
                                            groupApiMock.Object,
                                            areaApiMock.Object,
                                            projectRepositoryMock.Object,
                                            projectGitOpsRepositoryMock.Object, 
                                            knowledgeLibraryServiceMock.Object);


        await projectService.DeleteBranch(project.Id, branch.Id);


        Times gitExpectedCallTimes = gitEnabled ? Times.Once() : Times.Never();

        repositoryApiMock.Verify(m => m.DeleteRepositoryBranchAsync(It.Is<string>(id => id == project.Git.Id),
                                                                   It.Is<string>(branchId => branchId == branch.GitBranch),
                                                                   It.IsAny<int>(),
                                                                   It.IsAny<CancellationToken>()),
                                                                   gitExpectedCallTimes);


        projectRepositoryMock.Verify(m => m.DeleteBranch(It.Is<string>(id => id == project.Id),
                                                                   It.Is<string>(branchId => branchId == branch.Id)));


        projectGitOpsRepositoryMock.Verify(m => m.DeleteBranchManifests(It.Is<string>(id => id == project.Id),
                                                                        It.Is<string>(branchId => branchId == branch.Id)));
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async void Test_GetGitBranchFiles(bool gitEnabled)
    {
        var loggerMock = new Mock<ILogger<ProjectService>>();

        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(gitEnabled, false, false, false);

        var bucketApiMock = new Mock<IBucketApi>();
        var groupApiMock = new Mock<IGroupApi>();
        var areaApiMock = new Mock<IAreaApi>();

        var projectRepositoryMock = new Mock<IProjectRepository>();

        var project = TestHelpers.NewBalsamProject("Project");

        projectRepositoryMock.Setup(m => m.GetProject(It.Is<string>(id => id == project.Id), It.IsAny<bool>()))
                                .ReturnsAsync(() => project);

        var branch = TestHelpers.NewBalsamBranch("Branch", false);

        projectRepositoryMock.Setup(m => m.GetBranch(It.Is<string>(id => id == project.Id),
                                                        It.Is<string>(branchId => branchId == branch.Id)))
                                .ReturnsAsync(() => branch);

        var repositoryApiMock = new Mock<IRepositoryApi>();
        var repoFile = TestHelpers.NewRepoFile("folder", GitProviderApiClient.Model.RepoFile.TypeEnum.Folder);
        var repoFolder = TestHelpers.NewRepoFile("file", GitProviderApiClient.Model.RepoFile.TypeEnum.File);

        repositoryApiMock.Setup(m => m.GetFilesInBranchAsync(It.Is<string>(id => id == project.Git.Id),
                                                                It.Is<string>(branchId => branchId == branch.GitBranch),
                                                                It.IsAny<int>(),
                                                                It.IsAny<CancellationToken>()))
                                .ReturnsAsync((string repositoryId, string branchId, int operationIndex, CancellationToken ct) 
                                        => new List<GitProviderApiClient.Model.RepoFile>
                                        {
                                            repoFile,
                                            repoFolder
                                        });
                                
        var projectGitOpsRepositoryMock = new Mock<IProjectGitOpsRepository>();
        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();

        var projectService = new ProjectService(loggerMock.Object,
                                            capabilityOptionsSnapshotMock.Object,
                                            bucketApiMock.Object,
                                            repositoryApiMock.Object,
                                            groupApiMock.Object,
                                            areaApiMock.Object,
                                            projectRepositoryMock.Object,
                                            projectGitOpsRepositoryMock.Object, 
                                            knowledgeLibraryServiceMock.Object);

        var files = await projectService.GetGitBranchFiles(project.Id, branch.Id);

        Assert.NotNull(files);

        if (gitEnabled)
        {

            var actualFile = files.First(f => f.Id == repoFile.Id);
            var actualFolder = files.First(f => f.Id == repoFolder.Id);

            TestHelpers.AssertBalsamRepoFile(repoFile, actualFile);
            TestHelpers.AssertBalsamRepoFile(repoFolder, actualFolder);

        }

        Times gitExpectedCallTimes = gitEnabled ? Times.Once() : Times.Never();

        repositoryApiMock.Verify(m => m.GetFilesInBranchAsync(It.Is<string>(id => id == project.Git.Id),
                                                                   It.Is<string>(branchId => branchId == branch.GitBranch),
                                                                   It.IsAny<int>(),
                                                                   It.IsAny<CancellationToken>()),
                                                                   gitExpectedCallTimes);

    }


}