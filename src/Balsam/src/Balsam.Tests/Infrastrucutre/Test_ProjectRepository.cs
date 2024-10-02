using Balsam.Repositories;
using Balsam.Tests.Helpers;
using Microsoft.Extensions.Logging;

//using Castle.Core.Logging;
using Moq;

namespace Balsam.Tests.Infrastrucutre;

public class Test_ProjectRepository
{
    [Fact]
    public async void Test_CreateProject_And_GetProject()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<ProjectRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        try
        {

            var projectRepository = new ProjectRepository(capabilityOptionsSnapshotMock.Object, loggerMock.Object, hubRepositoryClientMock.Object);

            var defaultBranchName = "DefaultBranchName";

            var project = TestHelpers.NewBalsamProject("ProjectName");

            var createdProject = await projectRepository.CreateProject(project, defaultBranchName);

            Assert.NotNull(createdProject);
            Assert.True(createdProject.Id is not null);
            Assert.True(createdProject.Id.Trim().Length > 0);
            Assert.True(createdProject.Branches.Count > 0);
            Assert.True(createdProject.Branches[0].Name == defaultBranchName);

            var persistedProject = await projectRepository.GetProject(createdProject.Id);

            Assert.NotNull(persistedProject);

            Assert.Equivalent(createdProject, persistedProject);

            Assert.True(persistedProject.Branches.Count > 0);
            Assert.Equivalent(createdProject.Branches[0], persistedProject.Branches[0]);

            //Compare to original object
            Assert.Equal(project.Name, persistedProject.Name);
            Assert.Equal(project.Description, persistedProject.Description);
            Assert.Equivalent(project.Git, persistedProject.Git);
            Assert.Equivalent(project.Oidc, persistedProject.Oidc);
            Assert.Equivalent(project.Chat, persistedProject.Chat);
            Assert.Equivalent(project.S3, persistedProject.S3);
        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_DeleteProject()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<ProjectRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        try
        {

            var projectRepository = new ProjectRepository(capabilityOptionsSnapshotMock.Object, loggerMock.Object, hubRepositoryClientMock.Object);

            var defaultBranchName = "DefaultBranchName";

            var project = TestHelpers.NewBalsamProject("ProjectName");

            var createdProject = await projectRepository.CreateProject(project, defaultBranchName);

            Assert.NotNull(createdProject);
            Assert.True(createdProject.Id is not null);

            await projectRepository.DeleteProject(project.Id);

            var persistedProject = await projectRepository.GetProject(project.Id);

            Assert.Null(persistedProject);

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_UpdateProject()
    {
        //Setup
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<ProjectRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        //Test

        try
        {

            var projectRepository = new ProjectRepository(capabilityOptionsSnapshotMock.Object, loggerMock.Object, hubRepositoryClientMock.Object);

            var defaultBranchName = "DefaultBranchName";

            var originalName = "ProjectName";

            var project = TestHelpers.NewBalsamProject(originalName);
            

            var createdProject = await projectRepository.CreateProject(project, defaultBranchName);

            Assert.NotNull(createdProject);
            Assert.True(createdProject.Id is not null);
            Assert.True(createdProject.Id.Trim().Length > 0);

            Assert.Equal(project.Name, createdProject.Name);

            createdProject.Name = "NewName";

            await projectRepository.UpdateProject(createdProject);

            var persistedProject = await projectRepository.GetProject(createdProject.Id);

            Assert.NotNull(persistedProject);
            Assert.Equal(createdProject.Name, persistedProject.Name);
            Assert.NotEqual(originalName, persistedProject.Name);

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async void Test_GetProjects_With_Branches(bool includeBranches)
    {
        //Setup
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<ProjectRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        //Test

        try
        {

            var projectRepository = new ProjectRepository(capabilityOptionsSnapshotMock.Object, loggerMock.Object, hubRepositoryClientMock.Object);

            var defaultBranchName = "DefaultBranchName";

            var project1 = TestHelpers.NewBalsamProject("Project1");
            var project2 = TestHelpers.NewBalsamProject("Project2");

            await projectRepository.CreateProject(project1, defaultBranchName);
            await projectRepository.CreateProject(project2, defaultBranchName);


            
            var branch1 = await projectRepository.CreateBranch(project1.Id, "branch1", "Branch1");
            var branch2 = await projectRepository.CreateBranch(project1.Id, "branch2", "Branch2");
            var branch3 = await projectRepository.CreateBranch(project2.Id, "branch3", "Branch3");

            Assert.NotNull(branch1);
            Assert.NotNull(branch2);
            Assert.NotNull(branch3);

            var projects = await projectRepository.GetProjects(includeBranches);

            Assert.Equal(2, projects.Count);

            var persistedProject1 = projects.First(p => p.Name == project1.Name);
            Assert.Equal(project1.Description, persistedProject1.Description);

            var persistedProject2 = projects.First(p => p.Name == project2.Name);
            Assert.Equal(project2.Description, persistedProject2.Description);

            if (includeBranches)
            {
                Assert.Equal(3, persistedProject1.Branches.Count()); //3 including default branch
                Assert.True(persistedProject1.Branches.Exists(b => b.Id == branch1.Id));
                Assert.True(persistedProject1.Branches.Exists(b => b.Id == branch2.Id));


                Assert.Equal(2, persistedProject2.Branches.Count); //2 including default branch
                Assert.True(persistedProject2.Branches.Exists(b => b.Id == branch3.Id));
            }
            else
            {
                Assert.Empty(persistedProject1.Branches);
                Assert.Empty(persistedProject2.Branches);
            }

            


        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }


    [Fact]
    public async void Test_CreateBranch_And_GetBranch()
    {
        //Setup
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<ProjectRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        //Test

        try
        {

            var projectRepository = new ProjectRepository(capabilityOptionsSnapshotMock.Object, loggerMock.Object, hubRepositoryClientMock.Object);

            var defaultBranchName = "DefaultBranchName";

            var project1 = TestHelpers.NewBalsamProject("Project1");

            var branchName = "NewBranch";
            var branchDescription = "BranchDescription";

            var project = await projectRepository.CreateProject(project1, defaultBranchName);

            var branch = await projectRepository.CreateBranch(project.Id, branchName, branchDescription);

            Assert.NotNull(branch);
            Assert.NotNull(branch.Id);
            Assert.True(branch.Id.Trim().Length > 0);

            var persistedBranch = await projectRepository.GetBranch(project.Id, branch.Id);

            Assert.Equal(branchName, persistedBranch.Name);
            Assert.Equal(branchDescription, persistedBranch.Description);

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }


    [Fact]
    public async void Test_DeleteBranch()
    {
        //Setup
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<ProjectRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        //Test

        try
        {

            var projectRepository = new ProjectRepository(capabilityOptionsSnapshotMock.Object, loggerMock.Object, hubRepositoryClientMock.Object);

            var defaultBranchName = "DefaultBranchName";

            var newProject = TestHelpers.NewBalsamProject("Project1");


            var project = await projectRepository.CreateProject(newProject, defaultBranchName);

            Assert.NotNull(project);
            Assert.NotNull(project.Branches);
            Assert.Single(project.Branches);
            var createdBranch = project.Branches[0];
            Assert.NotNull(createdBranch.Id);
            Assert.True(createdBranch.Id.Length > 0);

            await projectRepository.DeleteBranch(project.Id, createdBranch.Id);

            var persistedBranch = await projectRepository.GetBranch(project.Id, createdBranch.Id);

            Assert.Null(persistedBranch);
        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }

    [Fact]
    public async void Test_ProjectExist()
    {
        var capabilityOptionsSnapshotMock = TestHelpers.CreateCapabilityOptionsSnapshotMock(true, true, true, true);

        var loggerMock = new Mock<ILogger<ProjectRepository>>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        //Test

        try
        {

            var projectRepository = new ProjectRepository(capabilityOptionsSnapshotMock.Object, loggerMock.Object, hubRepositoryClientMock.Object);

            var defaultBranchName = "DefaultBranchName";

            var newProject = TestHelpers.NewBalsamProject("Project1"); 


            var project = await projectRepository.CreateProject(newProject, defaultBranchName);


            var exists = await projectRepository.ProjectExists(project.Name);

            Assert.True(exists);

            var newProjectName = "new";

            Assert.NotEqual(newProjectName, project.Name);

            var newExists = await projectRepository.ProjectExists(newProjectName);

            Assert.False(newExists);

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }
}