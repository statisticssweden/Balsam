using Balsam.Model;
using Balsam.Repositories;
using Balsam.Utility;
using LibGit2Sharp;
using System.IO.Compression;

namespace Balsam.Tests.Infrastrucutre;

public class Test_KnowledgeLibraryContentRepository
{
    [Fact]
    public async void Test_GetFileTree()
    {

        var uniqueTempPath = Directory.CreateTempSubdirectory().FullName;
        var sourceKnowledgeLibraryPath = Path.Combine(uniqueTempPath, "knowledge_library");

        var dir1 = "dir1";
        var dir1a = "dir1\\a";

        var sourceRoot = sourceKnowledgeLibraryPath;
        var sourceDir1 = Path.Combine(sourceKnowledgeLibraryPath, dir1);
        var sourceDir1a = Path.Combine(sourceKnowledgeLibraryPath, dir1a);


        DirectoryUtil.AssureDirectoryExists(sourceDir1);
        DirectoryUtil.AssureDirectoryExists(sourceDir1a);


        var file1Root = "file1Root.txt";
        var file2Root = "file2Root.txt";
        var file1Dir1 = "file1Dir1.txt";
        var file2Dir1 = "file2Dir1.txt";
        var file1Dir1a = "file1Dir1a.txt";
        var file2Dir1a = "file2Dir1a.txt";

        try
        {

            File.WriteAllText(Path.Combine(sourceRoot, file1Root), file1Root); //Use file name as content
            File.WriteAllText(Path.Combine(sourceRoot, file2Root), file2Root);
            File.WriteAllText(Path.Combine(sourceDir1, file1Dir1), file1Dir1);
            File.WriteAllText(Path.Combine(sourceDir1, file2Dir1), file2Dir1);
            File.WriteAllText(Path.Combine(sourceDir1a, file1Dir1a), file1Dir1a);
            File.WriteAllText(Path.Combine(sourceDir1a, file2Dir1a), file2Dir1a);

            Repository.Init(sourceKnowledgeLibraryPath);

            using (var repo = new Repository(sourceKnowledgeLibraryPath))
            {
                Commands.Stage(repo, "*");

                var signature = new Signature("x", "x@x.com", DateTimeOffset.Now);
                repo.Commit("first commit", signature, signature);
            }

            var knowledgeLibraryContentRepository = new KnowledgeLibraryContentRepository();
            var repoId = "kb_" + Guid.NewGuid();

            var fileTree = await knowledgeLibraryContentRepository.GetFileTree(repoId, sourceKnowledgeLibraryPath);

            Assert.NotNull(fileTree);

            Assert.Equal(8, fileTree.Count());

            AssertFile("", file1Root, fileTree);
            AssertFile("", file2Root, fileTree);
            AssertFile(dir1, file1Dir1, fileTree);
            AssertFile(dir1, file2Dir1, fileTree);
            AssertFile(dir1a, file1Dir1a, fileTree);
            AssertFile(dir1a, file2Dir1a, fileTree);

            AssertFolder(dir1, fileTree);
            AssertFolder(dir1a, fileTree);


        }
        finally
        {
            DeleteDirectory(Path.Combine(sourceKnowledgeLibraryPath, ".git"));
            Directory.Delete(sourceKnowledgeLibraryPath, true);
        }
    }


    [Fact]
    public async void Test_GetFilePath()
    {
        var uniqueTempPath = Directory.CreateTempSubdirectory().FullName;
        var sourceKnowledgeLibraryPath = Path.Combine(uniqueTempPath, "knowledge_library");
        var dir1 = "dir1";
        
        var sourceDir1 = Path.Combine(sourceKnowledgeLibraryPath, dir1);
        DirectoryUtil.AssureDirectoryExists(sourceDir1);
        
        try
        {
            var file1Dir1 = "file1Dir1.txt";

            File.WriteAllText(Path.Combine(sourceDir1, file1Dir1), file1Dir1);

            Repository.Init(sourceKnowledgeLibraryPath);

            using (var repo = new Repository(sourceKnowledgeLibraryPath))
            {
                Commands.Stage(repo, "*");

                var signature = new Signature("x", "x@x.com", DateTimeOffset.Now);
                repo.Commit("first commit", signature, signature);
            }

            var knowledgeLibraryContentRepository = new KnowledgeLibraryContentRepository();
            var repoId = "kb_" + Guid.NewGuid();

            var fileTree = await knowledgeLibraryContentRepository.GetFileTree(repoId, sourceKnowledgeLibraryPath);

            Assert.NotNull(fileTree);

            var repoFile = fileTree.First(f => f.Name == file1Dir1);

            var filePath = knowledgeLibraryContentRepository.GetFilePath(repoId, repoFile.Id);

            Assert.True(File.Exists(filePath));

            Assert.EndsWith(Path.Combine(dir1, file1Dir1), filePath);

        }
        finally
        {
            DeleteDirectory(Path.Combine(sourceKnowledgeLibraryPath, ".git"));
            Directory.Delete(sourceKnowledgeLibraryPath, true);
        }
    }

    [Fact]
    public async void Test_GetZippedResource()
    {
        var uniqueTempPath = Directory.CreateTempSubdirectory().FullName;
        var sourceKnowledgeLibraryPath = Path.Combine(uniqueTempPath, "knowledge_library");

        var resourceName = "resource1";
        
        var resource1Path = $"Resources\\{resourceName}";

        var remoteResource1Path = Path.Combine(sourceKnowledgeLibraryPath, resource1Path);
        DirectoryUtil.AssureDirectoryExists(remoteResource1Path);

        try
        {
            var resourceFile = "resource.txt";
            var remoteResourceFilePath = Path.Combine(remoteResource1Path, resourceFile);

            var fileContent = "This is a file that is going to be compressed";
            File.WriteAllText(remoteResourceFilePath, fileContent);

            Repository.Init(sourceKnowledgeLibraryPath);

            using (var repo = new Repository(sourceKnowledgeLibraryPath))
            {
                Commands.Stage(repo, "*");

                var signature = new Signature("x", "x@x.com", DateTimeOffset.Now);
                repo.Commit("first commit", signature, signature);
            }

            var knowledgeLibraryContentRepository = new KnowledgeLibraryContentRepository();
            var repoId = "kb_" + Guid.NewGuid();

            var fileTree = await knowledgeLibraryContentRepository.GetFileTree(repoId, sourceKnowledgeLibraryPath);

            Assert.NotNull(fileTree);

            var repoFile = fileTree.First(f => f.Name == resourceName);


            string zipPath = await knowledgeLibraryContentRepository.GetZippedResource(repoId, sourceKnowledgeLibraryPath, repoFile.Id);


            var actualZipFileContentTempPath = Path.Combine(Directory.CreateTempSubdirectory().FullName, "actual");
            DirectoryUtil.AssureDirectoryExists(actualZipFileContentTempPath);

            ZipFile.ExtractToDirectory(zipPath, actualZipFileContentTempPath);

            var actualResource1Path = Path.Combine(actualZipFileContentTempPath, resourceName);
            Assert.True(Directory.Exists(actualResource1Path));

            var actualResoruceFilePath = Path.Combine(actualZipFileContentTempPath, resourceName, resourceFile);
            Assert.True(File.Exists(actualResoruceFilePath));

            var actualFileContent = File.ReadAllText(actualResoruceFilePath);

            Assert.Equal(fileContent, actualFileContent);

        }
        finally
        {
            
            DeleteDirectory(Path.Combine(sourceKnowledgeLibraryPath, ".git"));
            Directory.Delete(sourceKnowledgeLibraryPath, true);
        }
    }

    private static void AssertFile(string directory, string fileName, List<BalsamRepoFile> fileTree)
    {
        Assert.True(fileTree.Exists(f => f.Type == BalsamRepoFile.TypeEnum.FileEnum 
                                            && f.Name == fileName 
                                            && f.Path == Path.Combine(directory, fileName)));
    }

    private static void AssertFolder(string path, List<BalsamRepoFile> fileTree)
    {
        Assert.True(fileTree.Exists(f => f.Type == BalsamRepoFile.TypeEnum.FolderEnum && f.Path == path));
    }

    /// <summary>
    /// Deletes files and directories that are write protected like the .get-folder
    /// </summary>
    /// <param name="targetDir"></param>
    private static void DeleteDirectory(string targetDir)
    {
        string[] files = Directory.GetFiles(targetDir);
        string[] dirs = Directory.GetDirectories(targetDir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(targetDir, false);
    }
}
