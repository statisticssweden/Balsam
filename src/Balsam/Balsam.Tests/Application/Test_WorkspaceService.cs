using Balsam.Model;
using Balsam.Repositories;
using Balsam.Services;
using Balsam.Tests.Helpers;
using GitProviderApiClient.Api;
using GitProviderApiClient.Model;
using Microsoft.Extensions.Logging;

//using Castle.Core.Logging;
using Moq;
using S3ProviderApiClient.Api;
using S3ProviderApiClient.Model;

namespace Balsam.Tests.Application;

public class Test_WorkspaceService
{

    static int workspaceUniqueId = 0;

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async void Test_CreateWorkspace(bool gitEnabled, bool s3Enabled)
    {
        var loggerMock = new Mock<ILogger<WorkspaceService>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(gitEnabled, s3Enabled, false, false);
        var bucketApiMock = new Mock<IBucketApi>();
        var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
        var workspaceGitOpsRepositoryMock = new Mock<IWorkspaceGitOpsRepository>();
        var userApiMock = new Mock<IUserApi>();
        var projectRepositoryMock = new Mock<IProjectRepository>();

        var workspaceTemplate = TestHelpers.NewWorkspaceTemplate("template");
        var project = TestHelpers.NewBalsamProject("project");
        var branch = TestHelpers.NewBalsamBranch("branch", true);

        project.Branches.Add(branch);

        string projectId = project.Id;
        string templateId = workspaceTemplate.Id;
        string branchId = branch.Id;
        string workspaceName = "workspace";
        string userName = "userName1";
        string userMail = "x@y.z";


        workspaceRepositoryMock.Setup(m => m.GetWorkspaceTemplate(It.Is<string>(id => id == workspaceTemplate.Id)))
                                .ReturnsAsync(() => workspaceTemplate);

        

        workspaceRepositoryMock.Setup(m => m.GenerateWorkspaceId(It.IsAny<string>()))
                                .Returns((string name) =>
                                {
                                    return $"{name}{workspaceUniqueId++}";
                                });

        workspaceGitOpsRepositoryMock.Setup(m => m.GetWorkspaceUrl(It.IsAny<string>(),
                                                                It.IsAny<string>(),
                                                                It.IsAny<string>(),
                                                                It.IsAny<string>(),
                                                                It.IsAny<string>()))
                                        .ReturnsAsync((string projectId, string branchId, string userName, string workspaceId, string urlConfigFile) =>
                                        {
                                            return $"http://balsam/{projectId}/{branchId}/{userName}/{workspaceId}"; //Mock-Url
                                        });


        projectRepositoryMock.Setup(m => m.GetProject(It.Is<string>(id => id == project.Id),
                                                        It.IsAny<bool>()))
                                .ReturnsAsync(() => project);


        userApiMock.Setup(m => m.CreatePATAsync(It.Is<string>(id => id == userName),
                                                    It.IsAny<int>(),
                                                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string userName, int operationIndex, CancellationToken cancellationToken) =>
                    {
                        return new UserPATCreatedResponse(userName, "213465486");
                    });


        bucketApiMock.Setup(m => m.CreateAccessKeyAsync(It.IsAny<string>(),
                                                            It.IsAny<int>(),
                                                            It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => new AccessKeyCreatedResponse("6846846464", "adfsdfasfasdf"));

        var workspaceService = new WorkspaceService(capabilityOptionsSnapshotMock.Object,
                                                          workspaceRepositoryMock.Object,
                                                          loggerMock.Object,
                                                          bucketApiMock.Object,
                                                          workspaceGitOpsRepositoryMock.Object,
                                                          userApiMock.Object,
                                                          projectRepositoryMock.Object);


        var createdWorkspace = await workspaceService.CreateWorkspace(projectId, branchId, workspaceName, templateId, userName, userMail);



        Assert.NotNull(createdWorkspace);

        Assert.Equal(projectId, createdWorkspace.ProjectId);
        Assert.Equal(branchId, createdWorkspace.BranchId);
        Assert.Equal(workspaceName, createdWorkspace.Name);
        Assert.Equal(templateId, createdWorkspace.TemplateId);
        Assert.Equal(userName, createdWorkspace.Owner);

        Assert.False(string.IsNullOrEmpty(createdWorkspace.Id));

        Assert.False(string.IsNullOrEmpty(createdWorkspace.Url));
        Assert.Contains("http://", createdWorkspace.Url);



        workspaceRepositoryMock.Verify(m => m.CreateWorkspace(It.Is<BalsamWorkspace>(w => w.Id == createdWorkspace.Id)),
                                                                Times.Once);

        workspaceGitOpsRepositoryMock.Verify(m => m.CreateWorkspaceManifests(It.IsAny<BalsamProject>(),
                                                                                It.IsAny<BalsamBranch>(),
                                                                                It.IsAny<BalsamWorkspace>(),
                                                                                It.IsAny<UserInfo>(),
                                                                                It.IsAny<string>()),
                                                                                Times.Once);

        Times userApiCallTimes = gitEnabled ? Times.Once() : Times.Never();
        userApiMock.Verify(m => m.CreatePATAsync(It.IsAny<string>(),
                                                            It.IsAny<int>(),
                                                            It.IsAny<CancellationToken>()), userApiCallTimes);



    }

    [Fact]
    public async void Test_DeleteWorkspaces()
    {
        var loggerMock = new Mock<ILogger<WorkspaceService>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(false, false, false, false);
        var bucketApiMock = new Mock<IBucketApi>();
        var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
        var workspaceGitOpsRepositoryMock = new Mock<IWorkspaceGitOpsRepository>();
        var userApiMock = new Mock<IUserApi>();
        var projectRepositoryMock = new Mock<IProjectRepository>();

        var workspace = TestHelpers.NewBalsamWorkspace("workspace");
        

        var workspaceService = new WorkspaceService(capabilityOptionsSnapshotMock.Object,
                                                          workspaceRepositoryMock.Object,
                                                          loggerMock.Object,
                                                          bucketApiMock.Object,
                                                          workspaceGitOpsRepositoryMock.Object,
                                                          userApiMock.Object,
                                                          projectRepositoryMock.Object);


        await workspaceService.DeleteWorkspace(workspace.ProjectId, workspace.BranchId, workspace.Id, workspace.Owner);

        workspaceRepositoryMock.Verify(m => m.DeleteWorkspace(It.Is<string>(projectId => projectId == workspace.ProjectId),
                                                                It.Is<string>(branchId => branchId == workspace.BranchId),
                                                                It.Is<string>(id => id == workspace.Id),
                                                                It.Is<string>(userName => userName == workspace.Owner)), Times.Once);

        workspaceGitOpsRepositoryMock.Verify(m => m.DeleteWorkspaceManifests(It.Is<string>(projectId => projectId == workspace.ProjectId),
                                                                It.Is<string>(branchId => branchId == workspace.BranchId),
                                                                It.Is<string>(id => id == workspace.Id),
                                                                It.Is<string>(userName => userName == workspace.Owner)), Times.Once);

    }
}