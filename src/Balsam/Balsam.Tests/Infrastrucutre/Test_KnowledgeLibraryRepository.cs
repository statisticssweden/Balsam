using Balsam.Model;
using Balsam.Repositories;
using Balsam.Tests.Helpers;
using Balsam.Utility;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Balsam.Tests.Infrastrucutre;

public class Test_KnowledgeLibraryRepository
{
    [Fact]
    public async void Test_ListLibraries()
    {

        var loggerMock = new Mock<ILogger<KnowledgeLibraryRepository>>();
        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        var knowledgeLibraryContentRepositoryMock = new Mock<IKnowledgeLibraryContentRepository>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;
        HubPaths hubPaths = new HubPaths(hubPath);

        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);


        var knowledgeLibrary1 = TestHelpers.NewBalsamKnowledgeLibrary("kb1", "The Knowledge Library");
        var knowledgeLibrary2 = TestHelpers.NewBalsamKnowledgeLibrary("kb2", "The Other Knowledge Library");

        try
        {
            WriteKnowledgeLibrarayFile(hubPaths, knowledgeLibrary1);
            WriteKnowledgeLibrarayFile(hubPaths, knowledgeLibrary2);

            var knowledgeLibraryRepository = new KnowledgeLibraryRepository(loggerMock.Object,
                                                                        hubRepositoryClientMock.Object,
                                                                        knowledgeLibraryContentRepositoryMock.Object);



            var knowledgeLibraries = await knowledgeLibraryRepository.ListKnowledgeLibraries();

            Assert.Equal(2, knowledgeLibraries.Count());
            Assert.True(knowledgeLibraries.Exists(kb => kb.Id  == knowledgeLibrary1.Id));
            Assert.True(knowledgeLibraries.Exists(kb => kb.Id == knowledgeLibrary2.Id));
        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }

    }

    [Fact]
    public async void Test_GetKnowledgeLibrary()
    {

        var loggerMock = new Mock<ILogger<KnowledgeLibraryRepository>>();
        var hubRepositoryClientMock = new Mock<IHubRepositoryClient>();
        var knowledgeLibraryContentRepositoryMock = new Mock<IKnowledgeLibraryContentRepository>();

        var hubPath = Directory.CreateTempSubdirectory().FullName;
        HubPaths hubPaths = new HubPaths(hubPath);

        hubRepositoryClientMock.Setup(x => x.Path).Returns(hubPath);


        var knowledgeLibrary1 = TestHelpers.NewBalsamKnowledgeLibrary("kb1", "The Knowledge Library");
        var knowledgeLibrary2 = TestHelpers.NewBalsamKnowledgeLibrary("kb2", "The Other Knowledge Library");

        try
        {
            WriteKnowledgeLibrarayFile(hubPaths, knowledgeLibrary1);
            WriteKnowledgeLibrarayFile(hubPaths, knowledgeLibrary2);

            var knowledgeLibraryRepository = new KnowledgeLibraryRepository(loggerMock.Object,
                                                                        hubRepositoryClientMock.Object,
                                                                        knowledgeLibraryContentRepositoryMock.Object);


            var persistedKnowledgeLibrary = await knowledgeLibraryRepository.GetKnowledgeLibrary(knowledgeLibrary2.Id);

            Assert.Equivalent(knowledgeLibrary2, persistedKnowledgeLibrary);
        }
        finally
        {
            //Clean up tempfiles
            Directory.Delete(hubPath, true);
        }

    }

    private static void WriteKnowledgeLibrarayFile(HubPaths hubPaths, BalsamKnowledgeLibrary knowledgeLibrary)
    {
        var path = hubPaths.GetKnowledgeLibrariesPath();
        var filePath = hubPaths.GetKnowledgeLibraryFilePath(knowledgeLibrary.Id);
        var content = JsonConvert.SerializeObject(knowledgeLibrary);

        DirectoryUtil.AssureDirectoryExists(path);
        File.WriteAllText(filePath, content);
    }
}