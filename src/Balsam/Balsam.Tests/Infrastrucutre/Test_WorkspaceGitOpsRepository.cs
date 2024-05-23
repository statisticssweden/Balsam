using Balsam.Model;
using Balsam.Repositories;
using Balsam.Tests.Helpers;
using Moq;
using Balsam.Utility;

namespace Balsam.Tests.Infrastrucutre;

public class Test_WorkspacetGitOpsRepository
{
    [Fact]
    public async void Test_CreateWorkspaceManifests()
    {
        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        var project = TestHelpers.NewBalsamProject("Project");
        var branch = TestHelpers.NewBalsamBranch("Branch", true);
        var templateId = "template1";
        var owner = "theOnwer";
        var workspace = TestHelpers.NewBalsamWorkspace("Workspace", templateId, project.Id, branch.Id, owner);
        var userInfo = new UserInfo(owner, "x@y.z", "123456789");

        var workspaceContext = new WorkspaceContext(project, branch, workspace, userInfo);


        var properties = TestHelpers.GetMustaschProperties(workspaceContext).ToList();

        HubPaths hubPaths = new HubPaths(hubPath);

        var templateFileName = "workspace_template_test.yaml";

        var workspaceTemplatePath = hubPaths.GetWorkspaceTemplatesPath(templateId);
        var projectsTemplateFilePath = Path.Combine(workspaceTemplatePath, templateFileName);

        DirectoryUtil.AssureDirectoryExists(workspaceTemplatePath);

        //Write poperties to a test file
        File.WriteAllLines(projectsTemplateFilePath, properties);


        try
        {

            var projectGitOpsRepository = new WorkspaceGitOpsRepository(hubRepositoryClientMock.Object);

            await projectGitOpsRepository.CreateWorkspaceManifests(project, branch, workspace, userInfo, templateId);

            var workspacePath = hubPaths.GetWorkspacePath(project.Id, branch.Id, owner, workspace.Id);
            var workspaceFilePath = Path.Combine(workspacePath, templateFileName);

            Assert.True(File.Exists(workspaceFilePath));

            var fileContent = File.ReadAllText(workspaceFilePath);

            var propertiesAndValues = TestHelpers.GetAllPropertiesAndValues(workspaceContext);

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
    public async void Test_DeleteWorkspaceManifests()
    {
        var hubPath = Directory.CreateTempSubdirectory().FullName;

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);

        var project = TestHelpers.NewBalsamProject("Project");
        var branch = TestHelpers.NewBalsamBranch("Branch", true);
        var templateId = "template1";
        var owner = "theOnwer";
        var workspace = TestHelpers.NewBalsamWorkspace("Workspace", templateId, project.Id, branch.Id, owner);
        var userInfo = new UserInfo(owner, "x@y.z", "123456789");

        var workspaceContext = new WorkspaceContext(project, branch, workspace, userInfo);


        var properties = TestHelpers.GetMustaschProperties(workspaceContext).ToList();

        HubPaths hubPaths = new HubPaths(hubPath);

        var templateFileName = "workspace_template_test.yaml";

        var workspaceTemplatePath = hubPaths.GetWorkspaceTemplatesPath(templateId);
        var projectsTemplateFilePath = Path.Combine(workspaceTemplatePath, templateFileName);

        DirectoryUtil.AssureDirectoryExists(workspaceTemplatePath);

        //Write poperties to a test file
        File.WriteAllLines(projectsTemplateFilePath, properties);


        try
        {

            var projectGitOpsRepository = new WorkspaceGitOpsRepository(hubRepositoryClientMock.Object);

            await projectGitOpsRepository.CreateWorkspaceManifests(project, branch, workspace, userInfo, templateId);

            var workspacePath = hubPaths.GetWorkspacePath(project.Id, branch.Id, owner, workspace.Id);
            var workspaceFilePath = Path.Combine(workspacePath, templateFileName);

            Assert.True(File.Exists(workspaceFilePath));

            await projectGitOpsRepository.DeleteWorkspaceManifests(project.Id, branch.Id, workspace.Id, owner);

            Assert.False(File.Exists(workspaceFilePath));
            Assert.False(Directory.Exists(workspacePath));


        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }
    }


    [Fact]
    public async void Test_GetWorkspaceUrl()
    {
        var projectId = "project1";
        var branchId = "branch1";
        var userName = "username";
        var workspaceId = "workspace1";
        var urlConfigFile = "workspace_url_ingress.yaml";

        var hubPath = Directory.CreateTempSubdirectory().FullName;
        HubPaths hubPaths = new HubPaths(hubPath);

        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);
        string manifestPath = hubPaths.GetWorkspacePath(projectId, branchId, userName, workspaceId);

        string manifestFilePath = Path.Combine(manifestPath, urlConfigFile);
        string urltestpath = "ws.test.balsam";

        string content = @"apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  annotations:
    " + WorkspaceGitOpsRepository.WorkspaceUrlAnnotationKey + @": " + urltestpath + @"
    name: test-ingress
    namespace: testnamespace
spec:
  ingressClassName: contour
  rules:
  - host: test.host
    http:
      paths:
      - backend:
          service:
            name: test-svc
            port:
              number: 80
        path: /user/workspace
        pathType: Prefix";


        DirectoryUtil.AssureDirectoryExists(manifestPath);
        File.WriteAllText(manifestFilePath, content);

        var projectGitOpsRepository = new WorkspaceGitOpsRepository(hubRepositoryClientMock.Object);

        var url = await projectGitOpsRepository.GetWorkspaceUrl(projectId, branchId, userName, workspaceId, urlConfigFile);


        Assert.Equal(urltestpath, url);
    }
}