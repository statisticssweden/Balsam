using Balsam.Model;
using Balsam.Repositories;
using Balsam.Tests.Helpers;
using Moq;
using Balsam.Utility;

namespace Balsam.Tests.Infrastrucutre;

public class Test_ProjectGitOpsRepository
{
    [Fact]
    public async void Test_CreateProjectManifests()
    {
        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        var project = TestHelpers.NewBalsamProject("Project");


        ProjectContext projectContext = new ProjectContext();
        projectContext.Project = project;

        var properties = TestHelpers.GetMustaschProperties(projectContext).ToList();

        HubPaths hubPaths = new HubPaths(hubPath);

        var templateFileName = "project_template_test.yaml";

        var projectsTemplatePath = Path.Combine(hubPaths.GetTemplatesPath(), hubPaths.GetProjectsTemplatesPath());
        var projectsTemplateFilePath = Path.Combine(projectsTemplatePath, templateFileName);

        DirectoryUtil.AssureDirectoryExists(projectsTemplatePath);

        //Write poperties to a test file
        File.WriteAllLines(projectsTemplateFilePath, properties);


        try
        {
            var projectGitOpsRepository = new ProjectGitOpsRepository(hubRepositoryClientMock.Object);

            await projectGitOpsRepository.CreateProjectManifests(project);

            var projectPath = hubPaths.GetProjectPath(project.Id);
            var projectFilePath = Path.Combine(projectPath, templateFileName);

            Assert.True(File.Exists(projectFilePath));

            var fileContent = File.ReadAllText(projectFilePath);

            var propertiesAndValues = TestHelpers.GetAllPropertiesAndValues(projectContext);

            Assert.Equal(properties.Count, propertiesAndValues.Count);

            if (propertiesAndValues.Count > 0)
            {
                Assert.True(fileContent.Trim().Length > 0);
            }

            //Make shure that the mustasch properties has been replaced with the right values
            foreach (var property in propertiesAndValues)
            {
                bool found = TestHelpers.FindPropertyWithValue(fileContent, property);

                Assert.True(found);
            }


        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }


    [Fact]
    public async void Test_DeleteProjectManifests()
    {
        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        var project = TestHelpers.NewBalsamProject("Project");

        ProjectContext projectContext = new ProjectContext();
        projectContext.Project = project;

        var properties = TestHelpers.GetMustaschProperties(projectContext).ToList();

        HubPaths hubPaths = new HubPaths(hubPath);

        var templateFileName = "project_template_test.yaml";
        var projectsTemplatePath = Path.Combine(hubPaths.GetTemplatesPath(), hubPaths.GetProjectsTemplatesPath());
        var projectsTemplateFilePath = Path.Combine(projectsTemplatePath, templateFileName);

        

        try
        {
            DirectoryUtil.AssureDirectoryExists(projectsTemplatePath);

            //Write poperties to a test file
            File.WriteAllLines(projectsTemplateFilePath, properties);

            var projectGitOpsRepository = new ProjectGitOpsRepository(hubRepositoryClientMock.Object);

            await projectGitOpsRepository.CreateProjectManifests(project);

            var projectPath = hubPaths.GetProjectPath(project.Id);
            var projectFilePath = Path.Combine(projectPath, templateFileName);

            Assert.True(File.Exists(projectFilePath));

            await projectGitOpsRepository.DeleteProjectManifests(project.Id);

            Assert.False(File.Exists(projectFilePath));
            Assert.False(Directory.Exists(projectPath));

        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }


    [Fact]
    public async void Test_DeleteBranchManifests()
    {

        var hubPath = Directory.CreateTempSubdirectory().FullName;
        HubPaths hubPaths = new HubPaths(hubPath);

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        var projectId = "project1";
        var branchId = "branch1";

        var branchPath = hubPaths.GetBranchPath(projectId, branchId);

        try
        {
            DirectoryUtil.AssureDirectoryExists(branchPath);

            var branchManifestFilePath = Path.Combine(branchPath, "branch_manifest_file.yaml");

            File.WriteAllText(branchManifestFilePath, "test");

            Assert.True(File.Exists(branchManifestFilePath));

            var projectGitOpsRepository = new ProjectGitOpsRepository(hubRepositoryClientMock.Object);

            await projectGitOpsRepository.DeleteBranchManifests(projectId, branchId);

            Assert.False(Directory.Exists(branchPath));
            Assert.False(File.Exists(branchManifestFilePath));
        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }


}