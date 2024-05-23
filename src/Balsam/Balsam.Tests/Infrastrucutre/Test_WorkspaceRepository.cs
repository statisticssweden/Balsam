using Balsam.Model;
using Balsam.Repositories;
using Balsam.Tests.Helpers;
using Balsam.Utility;
using Microsoft.Extensions.Logging;

//using Castle.Core.Logging;
using Moq;
using Newtonsoft.Json;

namespace Balsam.Tests.Infrastrucutre;

public class Test_WorkspaceRepository
{
    [Fact]
    public async void Test_CreateWorkspace_And_GetWorkspace()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);


        var projectId = "project1";
        var branchId = "Branch1";
        var templateId = "template1";
        var owner = "theOnwer";
        var workspace = TestHelpers.NewBalsamWorkspace("Workspace", templateId, projectId, branchId, owner);


        try
        {
            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var createdWorkspace = await workspaceRepository.CreateWorkspace(workspace);

            Assert.NotNull(createdWorkspace);
            Assert.Equal(workspace.Owner, createdWorkspace.Owner);
            Assert.Equal(workspace.Name, createdWorkspace.Name);
            Assert.Equal(workspace.Url, createdWorkspace.Url);
            Assert.Equal(workspace.BranchId, createdWorkspace.BranchId);
            Assert.Equal(workspace.TemplateId, createdWorkspace.TemplateId);
            Assert.Equal(workspace.ProjectId, createdWorkspace.ProjectId);

            var persistedWorkspace = await workspaceRepository.GetWorkspace(projectId, owner, createdWorkspace.Id);


            Assert.NotNull(persistedWorkspace);
            Assert.Equivalent(createdWorkspace, persistedWorkspace);
        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_DeleteWorkspace()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        var workspace1 = TestHelpers.NewBalsamWorkspace("Workspace1", "template1", "project1", "branchId", "owner1");
        var workspace2 = TestHelpers.NewBalsamWorkspace("Workspace2", "template2", "project1", "branchId", "owner1");


        try
        {
            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var createdWorkspace1 = await workspaceRepository.CreateWorkspace(workspace1);
            var createdWorkspace2 = await workspaceRepository.CreateWorkspace(workspace2);

            Assert.NotNull(createdWorkspace1);
            Assert.NotNull(createdWorkspace2);

            await workspaceRepository.DeleteWorkspace(createdWorkspace1.ProjectId,
                                                      createdWorkspace1.BranchId,
                                                      createdWorkspace1.Id,
                                                      createdWorkspace1.Owner);

            var workspaces = await workspaceRepository.GetWorkspaces();

            Assert.Single(workspaces);

            Assert.False(workspaces.Exists(w => w.ProjectId == workspace1.ProjectId
                                            && w.BranchId == workspace1.BranchId
                                            && w.Id == workspace1.Id
                                            && w.Owner == workspace1.Owner));

            Assert.True(workspaces.Exists(w => w.ProjectId == workspace2.ProjectId
                                            && w.BranchId == workspace2.BranchId
                                            && w.Id == workspace2.Id
                                            && w.Owner == workspace2.Owner));


        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_GetWorkspaces()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        var workspace1 = TestHelpers.NewBalsamWorkspace("Workspace1", "template1", "project1", "branch1", "owner1");
        var workspace2 = TestHelpers.NewBalsamWorkspace("Workspace2", "template1", "project1", "branch1", "owner2");
        var workspace3 = TestHelpers.NewBalsamWorkspace("Workspace3", "template2", "project2", "branch1", "owner3");


        try
        {
            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var createdWorkspace1 = await workspaceRepository.CreateWorkspace(workspace1);
            var createdWorkspace2 = await workspaceRepository.CreateWorkspace(workspace2);
            var createdWorkspace3 = await workspaceRepository.CreateWorkspace(workspace3);

            
            var workspaces = await workspaceRepository.GetWorkspaces();

            Assert.Equal(3, workspaces.Count);

            Assert.True(workspaces.Exists(w => w.Name == workspace1.Name));
            Assert.True(workspaces.Exists(w => w.Name == workspace2.Name));
            Assert.True(workspaces.Exists(w => w.Name == workspace3.Name));


        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_GetWorkspacesByUser()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);
        var owner1 = "owner1";
        var owner2 = "owner2";
        var project1Id = "project1";
        var project2Id = "project2";
        var project3Id = "project3";

        var workspace1 = TestHelpers.NewBalsamWorkspace("Workspace1", "template1", project1Id, "branch1", owner1);
        var workspace2 = TestHelpers.NewBalsamWorkspace("Workspace2", "template1", project1Id, "branch1", owner1);
        var workspace3 = TestHelpers.NewBalsamWorkspace("Workspace3", "template1", project2Id, "branch1", owner1);
        var workspace4 = TestHelpers.NewBalsamWorkspace("Workspace4", "template2", project2Id, "branch1", owner2);
        var workspace5 = TestHelpers.NewBalsamWorkspace("Workspace5", "template2", project3Id, "branch1", owner2);


        try
        {
            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var createdWorkspace1 = await workspaceRepository.CreateWorkspace(workspace1);
            var createdWorkspace2 = await workspaceRepository.CreateWorkspace(workspace2);
            var createdWorkspace3 = await workspaceRepository.CreateWorkspace(workspace3);
            var createdWorkspace4 = await workspaceRepository.CreateWorkspace(workspace4);
            var createdWorkspace5 = await workspaceRepository.CreateWorkspace(workspace5);

            List<string> owner1Projects = new List<string>
            {
                project1Id,
                project2Id
            };

            var workspacesForOwner1 = await workspaceRepository.GetWorkspacesByUser(owner1, owner1Projects);

            Assert.Equal(3, workspacesForOwner1.Count());
            Assert.True(workspacesForOwner1.All(w => w.Owner == owner1));

            Assert.True(workspacesForOwner1.Exists(w => w.Name == workspace1.Name));
            Assert.True(workspacesForOwner1.Exists(w => w.Name == workspace2.Name));
            Assert.True(workspacesForOwner1.Exists(w => w.Name == workspace3.Name));
            Assert.False(workspacesForOwner1.Exists(w => w.Name == workspace4.Name));
            Assert.False(workspacesForOwner1.Exists(w => w.Name == workspace5.Name));

            List<string> owner2Projects = new List<string>
            {
                project2Id,
                project3Id
            };

            var workspacesForOwner2 = await workspaceRepository.GetWorkspacesByUser(owner2, owner2Projects);

            Assert.Equal(2, workspacesForOwner2.Count());
            Assert.True(workspacesForOwner2.All(w => w.Owner == owner2));

            Assert.False(workspacesForOwner2.Exists(w => w.Name == workspace1.Name));
            Assert.False(workspacesForOwner2.Exists(w => w.Name == workspace2.Name));
            Assert.False(workspacesForOwner2.Exists(w => w.Name == workspace3.Name));
            Assert.True(workspacesForOwner2.Exists(w => w.Name == workspace4.Name));
            Assert.True(workspacesForOwner2.Exists(w => w.Name == workspace5.Name));



        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_GetWorkspacesByProject()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);
        var owner1 = "owner1";
        var owner2 = "owner2";
        var project1Id = "project1";
        var project2Id = "project2";
        var project3Id = "project3";

        var workspace1 = TestHelpers.NewBalsamWorkspace("Workspace1", "template1", project1Id, "branch1", owner1);
        var workspace2 = TestHelpers.NewBalsamWorkspace("Workspace2", "template1", project1Id, "branch1", owner1);
        var workspace3 = TestHelpers.NewBalsamWorkspace("Workspace3", "template1", project2Id, "branch1", owner1);
        var workspace4 = TestHelpers.NewBalsamWorkspace("Workspace4", "template2", project2Id, "branch2", owner2);
        var workspace5 = TestHelpers.NewBalsamWorkspace("Workspace5", "template2", project3Id, "branch1", owner2);


        try
        {
            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var createdWorkspace1 = await workspaceRepository.CreateWorkspace(workspace1);
            var createdWorkspace2 = await workspaceRepository.CreateWorkspace(workspace2);
            var createdWorkspace3 = await workspaceRepository.CreateWorkspace(workspace3);
            var createdWorkspace4 = await workspaceRepository.CreateWorkspace(workspace4);
            var createdWorkspace5 = await workspaceRepository.CreateWorkspace(workspace5);

            var project2Workspaces = await workspaceRepository.GetWorkspacesByProject(project2Id);

            Assert.Equal(2, project2Workspaces.Count());
            Assert.True(project2Workspaces.All(w=> w.ProjectId == project2Id));

            Assert.False(project2Workspaces.Exists(w => w.Name == workspace1.Name));
            Assert.False(project2Workspaces.Exists(w => w.Name == workspace2.Name));
            Assert.True(project2Workspaces.Exists(w => w.Name == workspace3.Name));
            Assert.True(project2Workspaces.Exists(w => w.Name == workspace4.Name));
            Assert.False(project2Workspaces.Exists(w => w.Name == workspace5.Name));

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_GetWorkspacesByProjectAndBranch()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);
        var owner1 = "owner1";
        var owner2 = "owner2";
        var project1Id = "project1";
        var project2Id = "project2";
        var project3Id = "project3";
        var branch1Id = "branch1";
        var branch2Id = "branch2";

        var workspace1 = TestHelpers.NewBalsamWorkspace("Workspace1", "template1", project1Id, branch1Id, owner1);
        var workspace2 = TestHelpers.NewBalsamWorkspace("Workspace2", "template1", project1Id, branch1Id, owner1);
        var workspace3 = TestHelpers.NewBalsamWorkspace("Workspace3", "template1", project2Id, branch1Id, owner1);
        var workspace4 = TestHelpers.NewBalsamWorkspace("Workspace4", "template2", project2Id, branch2Id, owner2);
        var workspace5 = TestHelpers.NewBalsamWorkspace("Workspace5", "template2", project3Id, branch1Id, owner2);


        try
        {
            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var createdWorkspace1 = await workspaceRepository.CreateWorkspace(workspace1);
            var createdWorkspace2 = await workspaceRepository.CreateWorkspace(workspace2);
            var createdWorkspace3 = await workspaceRepository.CreateWorkspace(workspace3);
            var createdWorkspace4 = await workspaceRepository.CreateWorkspace(workspace4);
            var createdWorkspace5 = await workspaceRepository.CreateWorkspace(workspace5);

            var project2AndBranch2Workspaces = await workspaceRepository.GetWorkspacesByProjectAndBranch(project2Id, branch2Id);

            Assert.Single(project2AndBranch2Workspaces);
            Assert.True(project2AndBranch2Workspaces.All(w => w.ProjectId == project2Id && w.BranchId == branch2Id));

            Assert.False(project2AndBranch2Workspaces.Exists(w => w.Name == workspace1.Name));
            Assert.False(project2AndBranch2Workspaces.Exists(w => w.Name == workspace2.Name));
            Assert.False(project2AndBranch2Workspaces.Exists(w => w.Name == workspace3.Name));
            Assert.True(project2AndBranch2Workspaces.Exists(w => w.Name == workspace4.Name));
            Assert.False(project2AndBranch2Workspaces.Exists(w => w.Name == workspace5.Name));

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }


    [Fact]
    public async void Test_GetWorkspacesByProjectAndUser()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);
        var owner1 = "owner1";
        var owner2 = "owner2";
        var project1Id = "project1";
        var project2Id = "project2";
        var project3Id = "project3";
        var branch1Id = "branch1";
        var branch2Id = "branch2";

        var workspace1 = TestHelpers.NewBalsamWorkspace("Workspace1", "template1", project1Id, branch1Id, owner1);
        var workspace2 = TestHelpers.NewBalsamWorkspace("Workspace2", "template1", project1Id, branch1Id, owner1);
        var workspace3 = TestHelpers.NewBalsamWorkspace("Workspace3", "template1", project2Id, branch1Id, owner1);
        var workspace4 = TestHelpers.NewBalsamWorkspace("Workspace4", "template2", project2Id, branch2Id, owner2);
        var workspace5 = TestHelpers.NewBalsamWorkspace("Workspace5", "template2", project3Id, branch1Id, owner2);


        try
        {
            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var createdWorkspace1 = await workspaceRepository.CreateWorkspace(workspace1);
            var createdWorkspace2 = await workspaceRepository.CreateWorkspace(workspace2);
            var createdWorkspace3 = await workspaceRepository.CreateWorkspace(workspace3);
            var createdWorkspace4 = await workspaceRepository.CreateWorkspace(workspace4);
            var createdWorkspace5 = await workspaceRepository.CreateWorkspace(workspace5);

            var project2AndOwner2Workspaces = await workspaceRepository.GetWorkspacesByProjectAndUser(project2Id, owner2);

            Assert.Single(project2AndOwner2Workspaces);
            Assert.True(project2AndOwner2Workspaces.All(w => w.ProjectId == project2Id && w.Owner == owner2));

            Assert.False(project2AndOwner2Workspaces.Exists(w => w.Name == workspace1.Name));
            Assert.False(project2AndOwner2Workspaces.Exists(w => w.Name == workspace2.Name));
            Assert.False(project2AndOwner2Workspaces.Exists(w => w.Name == workspace3.Name));
            Assert.True(project2AndOwner2Workspaces.Exists(w => w.Name == workspace4.Name));
            Assert.False(project2AndOwner2Workspaces.Exists(w => w.Name == workspace5.Name));

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_GetWorkspacesByProjectBranchAndUser()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);
        var owner1 = "owner1";
        var owner2 = "owner2";
        var project1Id = "project1";
        var project2Id = "project2";
        var project3Id = "project3";
        var branch1Id = "branch1";
        var branch2Id = "branch2";

        var workspace1 = TestHelpers.NewBalsamWorkspace("Workspace1", "template1", project1Id, branch1Id, owner1);
        var workspace2 = TestHelpers.NewBalsamWorkspace("Workspace2", "template1", project1Id, branch2Id, owner1);
        var workspace3 = TestHelpers.NewBalsamWorkspace("Workspace3", "template1", project2Id, branch1Id, owner1);
        var workspace4 = TestHelpers.NewBalsamWorkspace("Workspace4", "template2", project2Id, branch2Id, owner2);
        var workspace5 = TestHelpers.NewBalsamWorkspace("Workspace5", "template2", project3Id, branch1Id, owner2);


        try
        {
            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var createdWorkspace1 = await workspaceRepository.CreateWorkspace(workspace1);
            var createdWorkspace2 = await workspaceRepository.CreateWorkspace(workspace2);
            var createdWorkspace3 = await workspaceRepository.CreateWorkspace(workspace3);
            var createdWorkspace4 = await workspaceRepository.CreateWorkspace(workspace4);
            var createdWorkspace5 = await workspaceRepository.CreateWorkspace(workspace5);

            var project1Branch1AndOwner1Workspaces = await workspaceRepository.GetWorkspacesByProjectBranchAndUser(project1Id, branch1Id, owner1);

            Assert.Single(project1Branch1AndOwner1Workspaces);
            Assert.True(project1Branch1AndOwner1Workspaces.All(w => w.ProjectId == project1Id && w.BranchId == branch1Id && w.Owner == owner1));

            Assert.True(project1Branch1AndOwner1Workspaces.Exists(w => w.Name == workspace1.Name));
            Assert.False(project1Branch1AndOwner1Workspaces.Exists(w => w.Name == workspace2.Name));
            Assert.False(project1Branch1AndOwner1Workspaces.Exists(w => w.Name == workspace3.Name));
            Assert.False(project1Branch1AndOwner1Workspaces.Exists(w => w.Name == workspace4.Name));
            Assert.False(project1Branch1AndOwner1Workspaces.Exists(w => w.Name == workspace5.Name));

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }


    [Fact]
    public async void Test_GetWorkspaceTemplate()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;
        HubPaths hubPaths = new HubPaths(hubPath);

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);


        var workspaceTemplate = TestHelpers.NewWorkspaceTemplate("template");
        var templateContent = JsonConvert.SerializeObject(workspaceTemplate);


        try
        {
            var workspaceTemplatePath = hubPaths.GetWorkspaceTemplatesPath(workspaceTemplate.Id);
            var workspaceTemplateFilePath = Path.Combine(workspaceTemplatePath, "properties.json");

            DirectoryUtil.AssureDirectoryExists(workspaceTemplatePath);
            File.WriteAllText(workspaceTemplateFilePath, templateContent);



            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var persistedWorkspaceTemplate = await workspaceRepository.GetWorkspaceTemplate(workspaceTemplate.Id);

            Assert.NotNull(persistedWorkspaceTemplate);

            Assert.Equivalent(workspaceTemplate, persistedWorkspaceTemplate);

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_ListWorkspaceTemplates()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<WorkspaceRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;
        HubPaths hubPaths = new HubPaths(hubPath);

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);


        var workspaceTemplate1 = TestHelpers.NewWorkspaceTemplate("template1");
        var workspaceTemplate2 = TestHelpers.NewWorkspaceTemplate("template2");
        


        try
        {
            WriteWorkspaceTemplateFile(hubPaths, workspaceTemplate1);
            WriteWorkspaceTemplateFile(hubPaths, workspaceTemplate2);

            var workspaceRepository = new WorkspaceRepository(hubRepositoryClientMock.Object, loggerMock.Object);
            var templates = await workspaceRepository.ListWorkspaceTemplates();

            Assert.NotNull(templates);

            Assert.Equal(2, templates.Count());

            Assert.Equivalent(workspaceTemplate1, templates.First(w => w.Id == workspaceTemplate1.Id));
            Assert.Equivalent(workspaceTemplate2, templates.First(w => w.Id == workspaceTemplate2.Id));

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    private static void WriteWorkspaceTemplateFile(HubPaths hubPaths, WorkspaceTemplate workspaceTemplate)
    {
        var workspaceTemplatePath = hubPaths.GetWorkspaceTemplatesPath(workspaceTemplate.Id);

        var workspaceTemplateFilePath = Path.Combine(workspaceTemplatePath, "properties.json");

        var templateContent = JsonConvert.SerializeObject(workspaceTemplate);

        DirectoryUtil.AssureDirectoryExists(workspaceTemplatePath);
        File.WriteAllText(workspaceTemplateFilePath, templateContent);
    }
}