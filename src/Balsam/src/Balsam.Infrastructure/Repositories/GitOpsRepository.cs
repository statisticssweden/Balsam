using Balsam.Model;
using Balsam.Utility;
using HandlebarsDotNet;

namespace Balsam.Repositories
{
    public class GitOpsRepository
    {
        protected readonly IHubRepositoryClient HubRepositoryClient;
        protected HubPaths HubPaths { get; set; }


        public GitOpsRepository(IHubRepositoryClient hubRepositoryClient)
        {
            HubRepositoryClient = hubRepositoryClient;
            HubPaths = new HubPaths(HubRepositoryClient.Path);

            Handlebars.RegisterHelper("curlies", (writer, context, parameters) =>
            {
                if (parameters.Length == 1 && parameters.At<bool>(0) == true)
                {
                    writer.Write("{{");
                }
                else
                {
                    writer.Write("}}");
                }

            });
        }

        /// <summary>
        /// Copies manifest-templates and replaces templated variables like {{variable}} with context value
        /// </summary>
        /// <param name="context">The Context for the template</param>
        /// <param name="destinationPath">The destination path for the manifest</param>
        /// <param name="templateRelativePath">The path to the template relative to the path of all templates</param>
        /// <returns></returns>
        protected async Task CreateManifests(BalsamContext context, string destinationPath, string templatePath)
        {

            //var templatesPath = HubPaths.GetTemplatesPath();
            //var templateFullPath = Path.Combine(templatesPath, templateRelativePath);
            
            DirectoryUtil.AssureDirectoryExists(destinationPath);

            foreach (var file in Directory.GetFiles(templatePath, "*.yaml"))
            {
                var source = await File.ReadAllTextAsync(file);

                var template = Handlebars.Compile(source);

                var result = template(context);

                var destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(file));

                await File.WriteAllTextAsync(destinationFilePath, result);
            }
        }

        protected async Task DeleteManifests(string path)
        {
            if (Directory.Exists(path))
            {
                await Task.Run(() => Directory.Delete(path, true));
            }
        }
    }
}
