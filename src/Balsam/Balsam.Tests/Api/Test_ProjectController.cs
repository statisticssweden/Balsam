using Balsam.Api.Controllers;
using Balsam.Interfaces;
using Balsam.Model;
using Balsam.Tests.Helpers;
using BalsamApi.Server.Models;
using GitProviderApiClient.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

//using Castle.Core.Logging;
using Moq;
using System.Security.Claims;
using BranchCreatedResponse = BalsamApi.Server.Models.BranchCreatedResponse;
using CreateBranchRequest = BalsamApi.Server.Models.CreateBranchRequest;

namespace Balsam.Tests.Application;

public class Test_ProjectController
{

    [Fact]
    public async void Test_CreateProject()
    {
        var loggerMock = new Mock<ILogger<ProjectController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var projectServiceMock = new Mock<IProjectService>();

        string projectName = "projectName";


        projectServiceMock.Setup(m => m.CreateProject(It.IsAny<string>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>()))
                            .ReturnsAsync((string preferredName, string description, string defaultBranchName, string username, string? sourceLocation = null) =>
                            {
                                var project = TestHelpers.NewBalsamProject(preferredName, description);
                                project.Branches.Add(TestHelpers.NewBalsamBranch(defaultBranchName, true));
                                project.Oidc.GroupName = projectName;
                                return project;
                            });


        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();
        var repositoryApiMock = new Mock<IRepositoryApi>();

        var projectController = new ProjectController(capabilityOptionsSnapshotMock.Object,
                                                      loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      knowledgeLibraryServiceMock.Object,
                                                      repositoryApiMock.Object);

        var testUserName = "test_user";
        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", "test_group")
            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        projectController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };


        var createProjectRequest = new CreateProjectRequest
        {
            BranchName = "branchName",
            Description = "description",
            Name = "projectName",
        };

        var response = await projectController.CreateProject(createProjectRequest);

        projectServiceMock.Verify(m => m.CreateProject(It.Is<string>(name => name == createProjectRequest.Name),
                                                            It.Is<string>(description => description == createProjectRequest.Description),
                                                            It.Is<string>(branchName => branchName == createProjectRequest.BranchName),
                                                            It.Is<string>(userName => userName == testUserName),
                                                            It.Is<string?>(sourceLocation => sourceLocation == null)));

        Assert.NotNull(response);

        var okObjectResult = response as OkObjectResult;
        Assert.NotNull(okObjectResult);

        var projectResponse = okObjectResult.Value as ProjectCreatedResponse;
        Assert.NotNull(projectResponse);

        Assert.Equal(createProjectRequest.Name, projectResponse.Name);

    }


    [Fact]
    public async void Test_CreateProject_ProjectExist()
    {
        var loggerMock = new Mock<ILogger<ProjectController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var projectServiceMock = new Mock<IProjectService>();

        projectServiceMock.Setup(m => m.ProjectExists(It.IsAny<string>()))
                            .ReturnsAsync(() => true);



        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();
        var repositoryApiMock = new Mock<IRepositoryApi>();

        var projectController = new ProjectController(capabilityOptionsSnapshotMock.Object,
                                                      loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      knowledgeLibraryServiceMock.Object,
                                                      repositoryApiMock.Object);

        var testUserName = "test_user";
        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", "test_group")
            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        projectController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };


        var sameName = "projectName";

        var createProjectRequest = new CreateProjectRequest
        {
            BranchName = "branchName1",
            Description = "description1",
            Name = sameName,
        };


        var response = await projectController.CreateProject(createProjectRequest);

        projectServiceMock.Verify(m => m.CreateProject(It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string?>()),
                                                            Times.Never);

        Assert.NotNull(response as BadRequestObjectResult);
    }


    [Theory]
    [InlineData("projectGroup", "")]
    [InlineData("projectGroup", "otherGroup")]
    [InlineData("projectGroup", "projectGroup")]
    public async void Test_CrateBranch_With_Authentication(string projectGroupName, string userGroupName)
    {
        var loggerMock = new Mock<ILogger<ProjectController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var projectServiceMock = new Mock<IProjectService>();


        var project = TestHelpers.NewBalsamProject("project");
        project.Oidc.GroupName = projectGroupName;

        projectServiceMock.Setup(m => m.GetProject(It.IsAny<string>(),
                                                It.IsAny<bool>()))
                            .ReturnsAsync((string projectId, bool includeBranches) => project);

        projectServiceMock.Setup(m => m.CreateBranch(It.IsAny<string>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>()))
                            .ReturnsAsync((string projectId, string fromBranch, string branchName, string description)
                                            => TestHelpers.NewBalsamBranch(branchName, false, description));


        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();
        var repositoryApiMock = new Mock<IRepositoryApi>();

        var projectController = new ProjectController(capabilityOptionsSnapshotMock.Object,
                                                      loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      knowledgeLibraryServiceMock.Object,
                                                      repositoryApiMock.Object);

        var testUserName = "test_user";
        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", userGroupName)
            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        projectController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };


        var createBranchRequest = new CreateBranchRequest
        {
            FromBranch = "fromBranch",
            Name = "name",
            Description = "description"
        };

        var response = await projectController.CreateBranch(project.Id, createBranchRequest);

        bool userInGroup = projectGroupName == userGroupName;

        Times verifyCallTimes = userInGroup ? Times.Once() : Times.Never();


        projectServiceMock.Verify(m => m.CreateBranch(It.Is<string>(projectId => projectId == project.Id),
                                                            It.Is<string>(fromBranch => fromBranch == createBranchRequest.FromBranch),
                                                            It.Is<string>(branchName => branchName == createBranchRequest.Name),
                                                            It.Is<string>(description => description == createBranchRequest.Description)),
                                                            verifyCallTimes);

        Assert.NotNull(response);

        if (userInGroup)
        {
            var okObjectResult = response as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var branchResponse = okObjectResult.Value as BranchCreatedResponse;
            Assert.NotNull(branchResponse);

            Assert.Equal(createBranchRequest.Name, branchResponse.Name);
        }
        else
        {
            Assert.NotNull(response as UnauthorizedObjectResult);
        }

    }


    [Fact]
    public async void Test_GetFiles()
    {
        var loggerMock = new Mock<ILogger<ProjectController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var projectServiceMock = new Mock<IProjectService>();

        var project = TestHelpers.NewBalsamProject("project1");
        var branch = TestHelpers.NewBalsamBranch("branch1", true);
        project.Branches.Add(branch);

        var file1 = TestHelpers.NewBalsamRepoFile("file1", BalsamRepoFile.TypeEnum.FileEnum);
        var folder1 = TestHelpers.NewBalsamRepoFile("folder1", BalsamRepoFile.TypeEnum.FolderEnum);

        var files = new List<BalsamRepoFile>
        {
            file1,
            folder1
        };

        projectServiceMock.Setup(m => m.GetGitBranchFiles(It.IsAny<string>(),
                                                        It.IsAny<string>()))
                            .ReturnsAsync(() => files);


        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();
        var repositoryApiMock = new Mock<IRepositoryApi>();

        var projectController = new ProjectController(capabilityOptionsSnapshotMock.Object,
                                                      loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      knowledgeLibraryServiceMock.Object,
                                                      repositoryApiMock.Object);

        var response = await projectController.GetFiles(project.Id, branch.Id);

        projectServiceMock.Verify(m => m.GetGitBranchFiles(It.Is<string>(projectId => projectId == project.Id),
                                                            It.Is<string>(branchId => branchId == branch.Id)));

        Assert.NotNull(response);

        var okObjectResult = response as OkObjectResult;
        Assert.NotNull(okObjectResult);

        var actualFiles = okObjectResult.Value as IEnumerable<BalsamApi.Server.Models.RepoFile>;
        Assert.NotNull(actualFiles);

        TestHelpers.AssertRepoFile(file1, actualFiles.First(f => f.Id == file1.Id));
        TestHelpers.AssertRepoFile(folder1, actualFiles.First(f => f.Id == folder1.Id));

    }

    [Fact]
    public async void Test_GetFile()
    {
        var loggerMock = new Mock<ILogger<ProjectController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var projectServiceMock = new Mock<IProjectService>();

        var project = TestHelpers.NewBalsamProject("project1");
        var branch = TestHelpers.NewBalsamBranch("branch1", true);
        project.Branches.Add(branch);

        var expectedFileId = "file1";

        var byteArray = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        var mediaType = "application/octet-stream";

        var fileContent = new FileContent(byteArray, mediaType);


        projectServiceMock.Setup(m => m.GetFile(It.IsAny<string>(),
                                                    It.IsAny<string>(),
                                                    It.IsAny<string>()))
                            .ReturnsAsync(() => fileContent);


        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();
        var repositoryApiMock = new Mock<IRepositoryApi>();

        var projectController = new ProjectController(capabilityOptionsSnapshotMock.Object,
                                                      loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      knowledgeLibraryServiceMock.Object,
                                                      repositoryApiMock.Object);

        var testUserName = "test_user";
        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", "group1")
            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        projectController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        var response = await projectController.GetFile(project.Id, branch.Id, expectedFileId);

        projectServiceMock.Verify(m => m.GetFile(It.Is<string>(projectId => projectId == project.Id),
                                                    It.Is<string>(branchId => branchId == branch.Id),
                                                    It.Is<string>(fileId => fileId == expectedFileId)));

        Assert.NotNull(response);

        var actualFile = response as FileContentResult;
        Assert.NotNull(actualFile);

        Assert.Equal(byteArray, actualFile.FileContents);

    }

    [Theory]
    [InlineData("projectGroup", "")]
    [InlineData("projectGroup", "otherGroup")]
    [InlineData("projectGroup", "projectGroup")]
    public async void Test_DeleteBranch_With_Authentication(string projectGroupName, string userGroupName)
    {
        var loggerMock = new Mock<ILogger<ProjectController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var projectServiceMock = new Mock<IProjectService>();


        var project = TestHelpers.NewBalsamProject("project");
        var branch = TestHelpers.NewBalsamBranch("branch1", true);
        
        project.Branches.Add(branch);
        project.Oidc.GroupName = projectGroupName;

        projectServiceMock.Setup(m => m.GetProject(It.IsAny<string>(),
                                                It.IsAny<bool>()))
                            .ReturnsAsync((string projectId, bool includeBranches) => project);


        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();
        var repositoryApiMock = new Mock<IRepositoryApi>();

        var projectController = new ProjectController(capabilityOptionsSnapshotMock.Object,
                                                      loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      knowledgeLibraryServiceMock.Object,
                                                      repositoryApiMock.Object);

        var testUserName = "test_user";
        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", userGroupName)
            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        projectController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };


        var response = await projectController.DeleteBranch(project.Id, branch.Id);

        bool userInGroup = projectGroupName == userGroupName;

        Times verifyCallTimes = userInGroup ? Times.Once() : Times.Never();


        projectServiceMock.Verify(m => m.DeleteBranch(It.Is<string>(projectId => projectId == project.Id),
                                                            It.Is<string>(branchId => branchId == branch.Id)),
                                            verifyCallTimes);

        Assert.NotNull(response);

        if (userInGroup)
        {
            var okResult = response as OkResult;
            Assert.NotNull(okResult);
        }
        else
        {
            Assert.NotNull(response as UnauthorizedObjectResult);
        }

    }

    [Theory]
    [InlineData("projectGroup", "")]
    [InlineData("projectGroup", "otherGroup")]
    [InlineData("projectGroup", "projectGroup")]
    public async void Test_DeleteProject_With_Authentication(string projectGroupName, string userGroupName)
    {
        var loggerMock = new Mock<ILogger<ProjectController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var projectServiceMock = new Mock<IProjectService>();


        var project = TestHelpers.NewBalsamProject("project");
        var branch = TestHelpers.NewBalsamBranch("branch1", true);

        project.Branches.Add(branch);
        project.Oidc.GroupName = projectGroupName;

        projectServiceMock.Setup(m => m.GetProject(It.IsAny<string>(),
                                                It.IsAny<bool>()))
                            .ReturnsAsync((string projectId, bool includeBranches) => project);


        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();
        var repositoryApiMock = new Mock<IRepositoryApi>();

        var projectController = new ProjectController(capabilityOptionsSnapshotMock.Object,
                                                      loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      knowledgeLibraryServiceMock.Object,
                                                      repositoryApiMock.Object);

        var testUserName = "test_user";
        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", userGroupName)
            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        projectController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        var response = await projectController.DeleteProject(project.Id);

        bool userInGroup = projectGroupName == userGroupName;

        Times verifyCallTimes = userInGroup ? Times.Once() : Times.Never();


        projectServiceMock.Verify(m => m.DeleteProject(It.Is<string>(projectId => projectId == project.Id)),
                                            verifyCallTimes);

        Assert.NotNull(response);

        if (userInGroup)
        {
            var okResult = response as OkResult;
            Assert.NotNull(okResult);
        }
        else
        {
            Assert.NotNull(response as UnauthorizedObjectResult);
        }

    }

    [Theory]
    [InlineData("projectGroup", "")]
    [InlineData("projectGroup", "otherGroup")]
    [InlineData("projectGroup", "projectGroup")]
    public async void Test_CopyFromKnowledgeLibrary_With_Authentication(string projectGroupName, string userGroupName)
    {
        var loggerMock = new Mock<ILogger<ProjectController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var projectServiceMock = new Mock<IProjectService>();


        var project = TestHelpers.NewBalsamProject("project");
        var branch = TestHelpers.NewBalsamBranch("branch1", true);

        project.Branches.Add(branch);
        project.Oidc.GroupName = projectGroupName;

        projectServiceMock.Setup(m => m.GetProject(It.IsAny<string>(),
                                                It.IsAny<bool>()))
                            .ReturnsAsync((string projectId, bool includeBranches) => project);


        var file = TestHelpers.NewBalsamRepoFile("file1", BalsamRepoFile.TypeEnum.FileEnum);

        var knowledgeLibraryServiceMock = new Mock<IKnowledgeLibraryService>();
        var knowledgeLibrary = TestHelpers.NewBalsamKnowledgeLibrary("kb1", "The knowledge library");

        knowledgeLibraryServiceMock.Setup(m => m.GetKnowledgeLibrary(It.Is<string>(libraryId => libraryId == knowledgeLibrary.Id)))
                                        .ReturnsAsync(() => knowledgeLibrary);


        var repositoryApiMock = new Mock<IRepositoryApi>();

        var projectController = new ProjectController(capabilityOptionsSnapshotMock.Object,
                                                      loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      knowledgeLibraryServiceMock.Object,
                                                      repositoryApiMock.Object);

        var testUserName = "test_user";
        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", userGroupName)
            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        projectController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };


        var response = await projectController.CopyFromKnowleadgeLibrary(project.Id,
                                                                         branch.Id,
                                                                         knowledgeLibrary.Id,
                                                                         file.Id);

        bool userInGroup = projectGroupName == userGroupName;

        Times verifyCallTimes = userInGroup ? Times.Once() : Times.Never();


        projectServiceMock.Verify(m => m.CopyFromKnowledgeLibrary(It.Is<string>(projectId => projectId == project.Id),
                                                                    It.Is<string>(branchId => branchId == branch.Id),
                                                                    It.Is<string>(libraryId => libraryId == knowledgeLibrary.Id),
                                                                    It.Is<string>(fileId => fileId == file.Id)),
                                            verifyCallTimes);

        Assert.NotNull(response);

        if (userInGroup)
        {
            var okResult = response as OkResult;
            Assert.NotNull(okResult);
        }
        else
        {
            Assert.NotNull(response as UnauthorizedObjectResult);
        }

    }
}