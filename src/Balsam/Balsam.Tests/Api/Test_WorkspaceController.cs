using Balsam.Api.Controllers;
using Balsam.Interfaces;
using Balsam.Model;
using Balsam.Tests.Helpers;
using BalsamApi.Server.Models;
using LibGit2Sharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

//using Castle.Core.Logging;
using Moq;
using System.Security.Claims;

namespace Balsam.Tests.Application;

public class Test_WorkspaceController
{
    [Theory]
    [InlineData("projectGroup", "")]
    [InlineData("projectGroup", "otherGroup")]
    [InlineData("projectGroup", "projectGroup")]
    public async void Test_CreateWorkspace_With_Authentication(string projectGroupName, string userGroupName)
    {
        var loggerMock = new Mock<ILogger<WorkspaceController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);
        var projectServiceMock = new Mock<IProjectService>();
        var project = TestHelpers.NewBalsamProject("project1");
        var branch = TestHelpers.NewBalsamBranch("branch1", true);
        var template = TestHelpers.NewWorkspaceTemplate("template1");
        var testUserName = "test_user";
        var testEmail = "x@y.z";

        project.Branches.Add(branch);
        project.Oidc.GroupName = projectGroupName;

        projectServiceMock.Setup(m => m.GetProject(It.IsAny<string>(),
                                                It.IsAny<bool>()))
                            .ReturnsAsync((string projectId, bool includeBranches) => project);

        var workspace = TestHelpers.NewBalsamWorkspace("expectedWorkspace");
        workspace.ProjectId = project.Id;
        workspace.BranchId = branch.Id;
        workspace.TemplateId = template.Id;
        workspace.Owner = testUserName;
        workspace.Url = "http://balsam.here.com";

        var createWorkspaceRequest = new CreateWorkspaceRequest
        {
            Name = workspace.Name,
            BranchId = workspace.BranchId,
            ProjectId = workspace.ProjectId,
            TemplateId = workspace.TemplateId,
        };


        var workspaceServiceMock = new Mock<IWorkspaceService>();
        workspaceServiceMock.Setup(m => m.CreateWorkspace(It.Is<string>(projectId => projectId == createWorkspaceRequest.ProjectId),
                                                            It.Is<string>(branchId => branchId == createWorkspaceRequest.BranchId),
                                                            It.Is<string>(name => name == createWorkspaceRequest.Name),
                                                            It.Is<string>(templateId => templateId == createWorkspaceRequest.TemplateId),
                                                            It.Is<string>(userName => userName == testUserName),
                                                            It.Is<string>(userMail => userMail == testEmail)))
                                .ReturnsAsync(() => workspace);


        var workspaceController = new WorkspaceController(loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      workspaceServiceMock.Object);


        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", userGroupName),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", testEmail)

            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        workspaceController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };



        var response = await workspaceController.CreateWorkspace(createWorkspaceRequest);

        bool userInGroup = projectGroupName == userGroupName;

        Times verifyCallTimes = userInGroup ? Times.Once() : Times.Never();

        workspaceServiceMock.Verify(m => m.CreateWorkspace(It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>()),
                                            verifyCallTimes);

        Assert.NotNull(response);

        if (userInGroup)
        {
            var okObjectResult = response as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var workspaceResponse = okObjectResult.Value as WorkspaceCreatedResponse;
            Assert.NotNull(workspaceResponse);

            AssertResult(workspace, workspaceResponse);

        }
        else
        {
            Assert.NotNull(response as UnauthorizedObjectResult);
        }

    }

    private static void AssertResult(BalsamWorkspace expectedWorkspace, WorkspaceCreatedResponse actualWorkspace)
    {
        Assert.Equal(expectedWorkspace.Name, actualWorkspace.Name);
        Assert.Equal(expectedWorkspace.BranchId, actualWorkspace.BranchId);
        Assert.Equal(expectedWorkspace.ProjectId, actualWorkspace.ProjectId);
        Assert.Equal(expectedWorkspace.Url, actualWorkspace.Url);
    }

    [Theory]
    [InlineData("projectGroup", "")]
    [InlineData("projectGroup", "otherGroup")]
    [InlineData("projectGroup", "projectGroup")]
    public async void Test_DeleteWorkspace_With_Authentication(string projectGroupName, string userGroupName)
    {
        var loggerMock = new Mock<ILogger<WorkspaceController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);
        var projectServiceMock = new Mock<IProjectService>();
        var project = TestHelpers.NewBalsamProject("project1");
        var branch = TestHelpers.NewBalsamBranch("branch1", true);
        var template = TestHelpers.NewWorkspaceTemplate("template1");
        var testUserName = "test_user";
        var testEmail = "x@y.z";

        project.Branches.Add(branch);
        project.Oidc.GroupName = projectGroupName;

        projectServiceMock.Setup(m => m.GetProject(It.IsAny<string>(),
                                                It.IsAny<bool>()))
                            .ReturnsAsync((string projectId, bool includeBranches) => project);

        var workspace = TestHelpers.NewBalsamWorkspace("expectedWorkspace");
        workspace.ProjectId = project.Id;
        workspace.BranchId = branch.Id;
        workspace.TemplateId = template.Id;
        workspace.Owner = testUserName;
        workspace.Url = "http://balsam.here.com";

        var workspaceServiceMock = new Mock<IWorkspaceService>();
        workspaceServiceMock.Setup(m => m.GetWorkspace(It.Is<string>(projectId => projectId == workspace.ProjectId),
                                                            It.Is<string>(userName => userName == workspace.Owner),
                                                            It.Is<string>(workspaceId => workspaceId == workspace.Id)))
                                .ReturnsAsync(() => workspace);

        var workspaceController = new WorkspaceController(loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      workspaceServiceMock.Object);

        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", userGroupName),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", testEmail)

            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        workspaceController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        var response = await workspaceController.DeleteWorkspace(workspace.Id, workspace.ProjectId, workspace.BranchId);

        bool userInGroup = projectGroupName == userGroupName;

        Times verifyCallTimes = userInGroup ? Times.Once() : Times.Never();

        workspaceServiceMock.Verify(m => m.DeleteWorkspace(It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>()),
                                            verifyCallTimes);

        Assert.NotNull(response);

        if (userInGroup)
        {
            var okObjectResult = response as OkObjectResult;
            Assert.NotNull(okObjectResult);
        }
        else
        {
            Assert.NotNull(response as UnauthorizedObjectResult);
        }
    }

    [Fact]
    public async void Test_ListTempates()
    {
        var loggerMock = new Mock<ILogger<WorkspaceController>>();
        var projectServiceMock = new Mock<IProjectService>();
        var workspaceServiceMock = new Mock<IWorkspaceService>();

        var template1 = TestHelpers.NewWorkspaceTemplate("template1");
        var template2 = TestHelpers.NewWorkspaceTemplate("template2");

        var listTemplates = new List<WorkspaceTemplate> {
            template1,
            template2
        };

        workspaceServiceMock.Setup(m => m.ListWorkspaceTemplates())
                                .ReturnsAsync(() => listTemplates);

        var workspaceController = new WorkspaceController(loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      workspaceServiceMock.Object);

        var response = await workspaceController.ListTemplates();

        workspaceServiceMock.Verify(m => m.ListWorkspaceTemplates());

        Assert.NotNull(response);

        var okObjectResult = response as OkObjectResult;
        Assert.NotNull(okObjectResult);

        var actualTemplates = okObjectResult.Value as IEnumerable<Template>;
        Assert.NotNull(actualTemplates);

        TestHelpers.AssertTemplate(template1, actualTemplates.First(t => t.Id == template1.Id));
        TestHelpers.AssertTemplate(template2, actualTemplates.First(t => t.Id == template2.Id));

    }

    [Fact]
    public async void Test_ListWorkspaces()
    {
        var loggerMock = new Mock<ILogger<WorkspaceController>>();
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);
        var projectServiceMock = new Mock<IProjectService>();
        var project1 = TestHelpers.NewBalsamProject("project1");
        var project2 = TestHelpers.NewBalsamProject("project2");
        var branch1 = TestHelpers.NewBalsamBranch("branch1", true);
        var branch2 = TestHelpers.NewBalsamBranch("branch2", true);
        var template = TestHelpers.NewWorkspaceTemplate("template1");

        var projects = new List<BalsamProject> { project1, project2 };

        var testUserName = "test_user";
        var testEmail = "x@y.z";

        var otherUserName = "other_user";
        var otherEmail = "a@b.c";

        project1.Branches.Add(branch1);
        project1.Oidc.GroupName = "project1Group";

        project2.Branches.Add(branch2);
        project2.Oidc.GroupName = "project2Group";

        projectServiceMock.Setup(m => m.GetProject(It.IsAny<string>(),
                                                It.IsAny<bool>()))
                            .ReturnsAsync((string projectId, bool includeBranches) =>
                            {
                                return projects.First(p => p.Id == projectId);
                            });


        projectServiceMock.Setup(m => m.GetProjects(It.IsAny<bool>()))
                            .ReturnsAsync(() => projects);


        var ownedWorkspace1 = TestHelpers.NewBalsamWorkspace("workspace1", template.Id, project1.Id, branch1.Id, testUserName);
        var notOwnedWorkspace1 = TestHelpers.NewBalsamWorkspace("workspace2", template.Id, project2.Id, branch2.Id, otherUserName);
        var ownedWorkspace2 = TestHelpers.NewBalsamWorkspace("workspace1", template.Id, project1.Id, branch1.Id, testUserName);
        var notOwnedWorkspace2 = TestHelpers.NewBalsamWorkspace("workspace2", template.Id, project2.Id, branch2.Id, otherUserName);

        var workspaces = new List<BalsamWorkspace>()
            {
                ownedWorkspace1,
                ownedWorkspace2,
                notOwnedWorkspace1,
                notOwnedWorkspace2
            };

        var workspaceServiceMock = new Mock<IWorkspaceService>();
        workspaceServiceMock.Setup(m => m.GetWorkspacesByProject(It.IsAny<string>()))
                                .ReturnsAsync((string projectId) => workspaces.Where(w => w.ProjectId == projectId).ToList());
        workspaceServiceMock.Setup(m => m.GetWorkspacesByProjectAndBranch(It.IsAny<string>(),
                                                                            It.IsAny<string>()))
                                    .ReturnsAsync((string projectId, string branchId)
                                                => workspaces.Where(w => w.ProjectId == projectId && w.BranchId == branchId).ToList());


        workspaceServiceMock.Setup(m => m.GetWorkspacesByProjectAndBranch(It.IsAny<string>(),
                                                                    It.IsAny<string>()))
                                    .ReturnsAsync((string projectId, string branchId)
                                                => workspaces.Where(w => w.ProjectId == projectId && w.BranchId == branchId).ToList());

        workspaceServiceMock.Setup(m => m.GetWorkspacesByProjectAndUser(It.IsAny<string>(),
                                                                    It.IsAny<string>()))
                                    .ReturnsAsync((string projectId, string userName)
                                                => workspaces.Where(w => w.ProjectId == projectId && w.Owner == userName).ToList());


        workspaceServiceMock.Setup(m => m.GetWorkspacesByProjectBranchAndUser(It.IsAny<string>(),
                                                                   It.IsAny<string>(),
                                                                   It.IsAny<string>()))
                                    .ReturnsAsync((string projectId, string branchId, string username)
                                                    => workspaces.Where(w => w.ProjectId == projectId && w.BranchId == branchId && w.Owner == username).ToList());

        workspaceServiceMock.Setup(m => m.GetWorkspaces())
                                    .ReturnsAsync(() => workspaces);


        workspaceServiceMock.Setup(m => m.GetWorkspacesByUser(It.IsAny<string>(),
                                                            It.IsAny<List<string>>()))
                                    .ReturnsAsync((string userName, List<string> projectIds) => workspaces.Where(w => projectIds.Contains(w.ProjectId) && w.Owner == userName).ToList());


        var workspaceController = new WorkspaceController(loggerMock.Object,
                                                      projectServiceMock.Object,
                                                      workspaceServiceMock.Object);

        var claims = new[]
            {

                new Claim("preferred_username", testUserName),
                new Claim("groups", project1.Oidc.GroupName),
                new Claim("groups", project2.Oidc.GroupName),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", testEmail)

            };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        workspaceController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        var result = await workspaceController.ListWorkspaces(null, null, null);
        AssertResponse(result, workspaces);

        result = await workspaceController.ListWorkspaces(null, null, false);
        AssertResponse(result, workspaces.Where(w => w.Owner == testUserName).ToList());

        result = await workspaceController.ListWorkspaces(project1.Id, null, null);
        AssertResponse(result, workspaces.Where(w => w.ProjectId == project1.Id).ToList());

        result = await workspaceController.ListWorkspaces(project1.Id, null, false);
        AssertResponse(result, workspaces.Where(w => w.ProjectId == project1.Id && w.Owner == testUserName).ToList());

        result = await workspaceController.ListWorkspaces(project1.Id, branch1.Id, null);
        AssertResponse(result, workspaces.Where(w => w.ProjectId == project1.Id && w.BranchId == branch1.Id).ToList());

        result = await workspaceController.ListWorkspaces(project1.Id, branch1.Id, false);
        AssertResponse(result, workspaces.Where(w => w.ProjectId == project1.Id && w.BranchId == branch1.Id && w.Owner == testUserName).ToList());



    }

    private static void AssertResponse(IActionResult result, List<BalsamWorkspace> expectedWorkspaces)
    {
        Assert.NotNull(result);

        var okObjectResult = result as OkObjectResult;
        Assert.NotNull(okObjectResult);

        var workspacesResult = okObjectResult.Value as IEnumerable<Workspace>;
        Assert.NotNull(workspacesResult);
        
        Assert.Equal(workspacesResult.Count(), expectedWorkspaces.Count());

        foreach( var expectedWorkspace in expectedWorkspaces )
        {
            var workspaceResult = workspacesResult.First(w => w.Id == expectedWorkspace.Id);
            Assert.NotNull(workspaceResult);

            TestHelpers.AssertWorkspace(expectedWorkspace, workspaceResult);
        }
    }


    //[Theory]
    //[InlineData("")]
    //[InlineData("project1Group")]
    //public async void Test_ListWorkspace_With_Authentication(string userGroupName)
    //{
    //    var loggerMock = new Mock<ILogger<WorkspaceController>>();
    //    var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);
    //    var projectServiceMock = new Mock<IProjectService>();
    //    var project1 = TestHelpers.NewBalsamProject("project1");
    //    var project2 = TestHelpers.NewBalsamProject("project2");
    //    var branch1 = TestHelpers.NewBalsamBranch("branch1", true);
    //    var branch2 = TestHelpers.NewBalsamBranch("branch2", true);
    //    var template = TestHelpers.NewWorkspaceTemplate("template1");

    //    var projects = new List<BalsamProject> { project1, project2 };

    //    var testUserName = "test_user";
    //    var testEmail = "x@y.z";

    //    var otherUserName = "other_user";
    //    var otherEmail = "a@b.c";

    //    project1.Branches.Add(branch);
    //    project1.Oidc.GroupName = "project1Group";

    //    project1.Branches.Add(branch);
    //    project1.Oidc.GroupName = "project2Group";

    //    projectServiceMock.Setup(m => m.GetProject(It.IsAny<string>(),
    //                                            It.IsAny<bool>()))
    //                        .ReturnsAsync((string projectId, bool includeBranches) =>
    //                        {
    //                            return projects.First(p => p.Id == projectId);
    //                        });


    //    projectServiceMock.Setup(m => m.GetProjects(It.IsAny<bool>()))
    //                        .ReturnsAsync(() => projects);


    //    var ownedWorkspace1 = TestHelpers.NewBalsamWorkspace("workspace1", template.Id, project1.Id, branch1.Id, testUserName);
    //    var notOwnedWorkspace1 = TestHelpers.NewBalsamWorkspace("workspace2", template.Id, project2.Id, branch2.Id, otherUserName);
    //    var ownedWorkspace2 = TestHelpers.NewBalsamWorkspace("workspace1", template.Id, project1.Id, branch1.Id, testUserName);
    //    var notOwnedWorkspace2 = TestHelpers.NewBalsamWorkspace("workspace2", template.Id, project2.Id, branch2.Id, otherUserName);

    //    var workspaces = new List<BalsamWorkspace>()
    //    {
    //        ownedWorkspace1,
    //        ownedWorkspace2,
    //        notOwnedWorkspace1,
    //        notOwnedWorkspace2
    //    };

    //    var workspaceServiceMock = new Mock<IWorkspaceService>();
    //    workspaceServiceMock.Setup(m => m.GetWorkspacesByProject(It.IsAny<string>()))
    //                            .ReturnsAsync((string projectId) => workspaces.Where(w => w.ProjectId == projectId).ToList());
    //    workspaceServiceMock.Setup(m => m.GetWorkspacesByProjectAndBranch(It.IsAny<string>(), 
    //                                                                        It.IsAny<string>()))
    //                           .ReturnsAsync((string projectId, string branchId) 
    //                                            => workspaces.Where(w => w.ProjectId == projectId && w.BranchId == branchId).ToList());


    //    workspaceServiceMock.Setup(m => m.GetWorkspacesByProjectAndBranch(It.IsAny<string>(),
    //                                                                It.IsAny<string>()))
    //                           .ReturnsAsync((string projectId, string username) 
    //                                            => workspaces.Where(w => w.ProjectId == projectId && w.Owner == username).ToList());

    //    workspaceServiceMock.Setup(m => m.GetWorkspacesByProjectBranchAndUser(It.IsAny<string>(),
    //                                                               It.IsAny<string>(),
    //                                                               It.IsAny<string>()))
    //                              .ReturnsAsync((string projectId, string branchId, string username) 
    //                                                => workspaces.Where(w => w.ProjectId == projectId && w.BranchId == branchId && w.Owner == username).ToList());

    //    workspaceServiceMock.Setup(m => m.GetWorkspaces())
    //                              .ReturnsAsync(() => workspaces);


    //    workspaceServiceMock.Setup(m => m.GetWorkspacesByUser(It.IsAny<string>(),
    //                                                        It.IsAny<List<string>>()))
    //                       .ReturnsAsync((string userName, List<string> projectId)
    //                                         => workspaces.Where(w => w.ProjectId && w.BranchId == branchId && w.Owner == username).ToList());


    //    var workspaceController = new WorkspaceController(loggerMock.Object,
    //                                                  projectServiceMock.Object,
    //                                                  workspaceServiceMock.Object);


    //    var claims = new[]
    //        {

    //            new Claim("preferred_username", testUserName),
    //            new Claim("groups", userGroupName),
    //            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", testEmail)

    //        };

    //    var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

    //    workspaceController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };



    //    var response = await workspaceController.CreateWorkspace(createWorkspaceRequest);

    //    bool userInGroup = projectGroupName == userGroupName;

    //    Times verifyCallTimes = userInGroup ? Times.Once() : Times.Never();

    //    workspaceServiceMock.Verify(m => m.CreateWorkspace(It.IsAny<string>(),
    //                                                        It.IsAny<string>(),
    //                                                        It.IsAny<string>(),
    //                                                        It.IsAny<string>(),
    //                                                        It.IsAny<string>(),
    //                                                        It.IsAny<string>()),
    //                                        verifyCallTimes);

    //    Assert.NotNull(response);

    //    if (userInGroup)
    //    {
    //        var okObjectResult = response as OkObjectResult;
    //        Assert.NotNull(okObjectResult);

    //        var workspaceResponse = okObjectResult.Value as WorkspaceCreatedResponse;
    //        Assert.NotNull(workspaceResponse);

    //        Assert.Equal(workspaceResponse.Name, workspace.Name);
    //        Assert.Equal(workspaceResponse.BranchId, workspace.BranchId);
    //        Assert.Equal(workspaceResponse.ProjectId, workspace.ProjectId);
    //        Assert.Equal(workspaceResponse.Url, workspace.Url);


    //    }
    //    else
    //    {
    //        Assert.NotNull(response as UnauthorizedObjectResult);
    //    }

    //}
}