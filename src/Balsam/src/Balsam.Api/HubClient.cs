using Balsam.Api.Models;
using BalsamApi.Server.Models;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Balsam.Api
{
    public class HubClient
    {
        private readonly CapabilityOptions _git;
        private readonly CapabilityOptions _s3;
        private readonly CapabilityOptions _authentication;
        private readonly S3Client _s3Client;
        private readonly HubRepositoryClient _hubRepositoryClient;

        

         public HubClient(IOptionsSnapshot<CapabilityOptions> capabilityOptions, HubRepositoryClient hubRepoClient, S3Client s3Client)
        {
            _s3Client = s3Client;
            _hubRepositoryClient = hubRepoClient;

            _git = capabilityOptions.Get(Capabilities.Git);
            _s3 = capabilityOptions.Get(Capabilities.S3);
            _authentication = capabilityOptions.Get(Capabilities.Authentication);

            
        }

        public async Task CreateProgram(string preferredName)
        {
            //TODO check if there is a program with the same name.
            var program = new BalsamProgram();
            program.Id = Guid.NewGuid().ToString();
            program.Name = preferredName;

            string programPath = Path.Combine(_hubRepositoryClient.Path, "hub", program.Id);
            DirectoryUtil.AssureDirectoryExists(programPath);

            var tasks = new List<Task>();
            Task<S3Data> s3Task = null;

            //TODO Implement
            if (_authentication.Enabled)
            {
                //TODO call CreateRole in the OicdProvider
            }

            if (_git.Enabled)
            {
                //TODO call CreateRepository in GitProvider
            }

            if (_s3.Enabled)
            {
                s3Task = _s3Client.CreateRepository(preferredName);
                tasks.Add(s3Task);
            }
            await Task.WhenAll(tasks);

            if (s3Task != null)
            {
                program.S3 = s3Task.Result;
            }

            string propPath = Path.Combine(programPath, "properties.json");

            _hubRepositoryClient.PullChanges();
            // serialize JSON to a string and then write string to a file
            File.WriteAllText(propPath, JsonConvert.SerializeObject(program));
            _hubRepositoryClient.PersistChanges($"Created new program with id {program.Id}");
        }
    }
}
