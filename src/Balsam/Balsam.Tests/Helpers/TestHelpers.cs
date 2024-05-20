using Balsam.Model;
using BalsamApi.Server.Models;
using GitProviderApiClient.Model;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Sdk;
using RepoFile = GitProviderApiClient.Model.RepoFile;

namespace Balsam.Tests.Helpers
{
    internal class TestHelpers
    {
        static int _balsamProjectUniqueId = 0;

        public static BalsamProject NewBalsamProject(string name)
        {
            int id = _balsamProjectUniqueId++;
            return new BalsamProject(name, "Test project")
            {
                Id = name + id,
                Git = new GitData
                {
                    Id = "Git" + id,
                    Name = "GitName" + id,
                    Path = "GitPath" + id,
                },
                S3 = new S3Data
                {
                    BucketName = "BucketName" + id,
                },
                Chat = new ChatData("chat" + id, "Chat"),
                Oidc = new OidcData("group" + id, "groupName")
            };
        }

        public static BalsamWorkspace NewBalsamWorkspace(string name)
        {
            var id = _balsamWorkspaceUniqueId++;
            return new BalsamWorkspace(id: name + id,
                name: name,
                templateId: "template" + id,
                projectId: "project" + id,
                branchId: "branch" + id,
                owner: "owner" + id);

        }

        static int _balsamBranchUniqueId = 0;
        public static BalsamBranch NewBalsamBranch(string name, bool isDefault)
        {
            int id = _balsamBranchUniqueId++;
            return new BalsamBranch
            {
                Id = name + id,
                IsDefault = isDefault,
                Description = "Description" + id,
                Name = name,
                GitBranch = "gitBranch" + id,
            };
        }

        static int _balsamWorkspaceUniqueId = 0;
        public static BalsamWorkspace NewBalsamWorkspace(string name, string templateId, string projectId, string branchId, string owner)
        {
            int id = _balsamWorkspaceUniqueId++;
            return new BalsamWorkspace(id: name + id,
                                        name: name,
                                        templateId: templateId,
                                        projectId: projectId,
                                        branchId: branchId,
                                        owner: owner);
        }

        static int _workspaceTemplateUniqueId = 0;
        public static WorkspaceTemplate NewWorkspaceTemplate(string name)
        {
            int id = _workspaceTemplateUniqueId++;
            return new WorkspaceTemplate
            {
                Id = "template" + id,
                Name = name + id,
                Description = "Description" + id,
                UrlConfig = $"urlconfig{id}.yaml"
            };
        }


        public static Mock<IOptionsSnapshot<CapabilityOptions>> CreateCapabilityOptionsSnapshotMock(bool gitEnabled, bool s3Enabled, bool authenticationEnabled, bool chatEnabled)
        {
            var gitCapabilityOptions = new CapabilityOptions
            {
                Enabled = gitEnabled,
                ServiceLocation = $"localhost/git" //Mock-value
            };

            var s3CapabilityOptions = new CapabilityOptions
            {
                Enabled = s3Enabled,
                ServiceLocation = $"localhost/s3" //Mock-value
            };

            var authenticationCapabilityOptions = new CapabilityOptions
            {
                Enabled = authenticationEnabled,
                ServiceLocation = $"localhost/authentication" //Mock-value
            };

            var chatCapabilityOptions = new CapabilityOptions
            {
                Enabled = chatEnabled,
                ServiceLocation = $"localhost/chat" //Mock-value
            };


            var optionsSnapshotMock = new Mock<IOptionsSnapshot<CapabilityOptions>>();

            optionsSnapshotMock.Setup(m => m.Get(Capabilities.Git)).Returns((string name) => gitCapabilityOptions);
            optionsSnapshotMock.Setup(m => m.Get(Capabilities.S3)).Returns((string name) => s3CapabilityOptions);
            optionsSnapshotMock.Setup(m => m.Get(Capabilities.Authentication)).Returns((string name) => authenticationCapabilityOptions);
            optionsSnapshotMock.Setup(m => m.Get(Capabilities.Chat)).Returns((string name) => chatCapabilityOptions);

            return optionsSnapshotMock;
        }

        static int repoFileUniqueId = 0;
        internal static RepoFile NewRepoFile(string name, RepoFile.TypeEnum type)
        {
            repoFileUniqueId++;

            return new RepoFile(id: repoFileUniqueId.ToString(),
                                    path: "/" + name,
                                    name: name,
                                    type: type,
                                    contentUrl: "http://" + name);

        }

        internal static void AssertRepoFile(GitProviderApiClient.Model.RepoFile repoFile, BalsamRepoFile actualFile)
        {
            Assert.Equal(repoFile.Id, actualFile.Id);
            Assert.Equal(repoFile.Name, actualFile.Name);
            Assert.Equal(repoFile.ContentUrl, actualFile.ContentUrl);
            Assert.Equal(repoFile.Path, actualFile.Path);

            switch (repoFile.Type)
            {
                case GitProviderApiClient.Model.RepoFile.TypeEnum.File:
                    Assert.Equal(BalsamRepoFile.TypeEnum.FileEnum, actualFile.Type);
                    break;
                case GitProviderApiClient.Model.RepoFile.TypeEnum.Folder:
                    Assert.Equal(BalsamRepoFile.TypeEnum.FolderEnum, actualFile.Type);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static IEnumerable<string> GetMustaschProperties(object obj)
        {
            var properties = GetAllPropertiesAndValues(obj);

            return properties.Select(p => p.Key + ": {{" + p.Key + "}}");
        }

        public static Dictionary<string, object> GetAllPropertiesAndValues(object obj, string prefix = "")
        {
            var propertiesAndValues = new Dictionary<string, object>();
            if (obj == null)
            {
                return propertiesAndValues;
            }

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                var path = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
                {
                    continue;
                }

                if (property.PropertyType.Assembly == typeof(int).Assembly)
                {
                    propertiesAndValues.Add(path, value);
                }
                else
                {
                    var nestedPropertiesAndValues = GetAllPropertiesAndValues(value, path);
                    foreach (var prop in nestedPropertiesAndValues)
                    {
                        propertiesAndValues.Add(prop.Key, prop.Value);
                    }
                }
            }

            return propertiesAndValues;
        }

        public static bool FindPropertyWithValue(string content, KeyValuePair<string, object> property)
        {
            string pattern = $"\\s*{property.Key}\\s*:\\s*{property.Value}";
            System.Text.RegularExpressions.Match match = Regex.Match(content, pattern);
            bool found = match.Success;
            return found;
        }

        internal static BalsamKnowledgeLibrary NewBalsamKnowledgeLibrary(string id, string name)
        {
            return new BalsamKnowledgeLibrary()
            {
                Id = id,
                Description = "Description",
                Name = name,
                RepositoryFriendlyUrl = $"http://{id}.somegit.html",
                RepositoryUrl = $"http://{id}.somegit.git",
            };
        }
    }
}
