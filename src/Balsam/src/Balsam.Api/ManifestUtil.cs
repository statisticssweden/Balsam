using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using BalsamApi.Server.Models;

namespace Balsam.Api
{
    public class ManifestUtil
    {
        internal class ManifestFile
        {
            public string ApiVersion { get; set; }
            public string Kind { get; set; }
            public Metadata Metadata { get; set; }

            public ManifestFile()
            {
                Metadata = new Metadata();
            }
        }


        internal class Metadata
        {
            public string Name { get; set; }
            public Dictionary<string, string> Annotations { get; set; } = new Dictionary<string, string>();

            public Metadata()
            {
               
            }
        }

        public static string GetWorkspaceUrl(string manifestFilePath)
        {

            if (!File.Exists(manifestFilePath))
            {
                return "";
            }

            var deserializer = new DeserializerBuilder()
               .IgnoreUnmatchedProperties()
               .WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();

            ManifestFile manifest;
            using (StreamReader manifestContent = File.OpenText(manifestFilePath))
            {
                manifest = deserializer.Deserialize<ManifestFile>(manifestContent);
            }

            if (manifest != null && manifest.Metadata.Annotations.ContainsKey("balsam-workspace-url"))
            {
                return manifest.Metadata.Annotations["balsam-workspace-url"];
            }
            return "";
        }
    }
}
