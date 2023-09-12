using Balsam.Api.Models;
using BalsamApi.Server.Models;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly GitClient _gitClient;
        private readonly HubRepositoryClient _hubRepositoryClient;
        private readonly IMemoryCache _memoryCache;



        public HubClient(IOptionsSnapshot<CapabilityOptions> capabilityOptions, IMemoryCache memoryCach, HubRepositoryClient hubRepoClient, S3Client s3Client, GitClient gitClient)
        {
            _memoryCache = memoryCach;
            _s3Client = s3Client;
            _gitClient = gitClient;
            _hubRepositoryClient = hubRepoClient;

            _git = capabilityOptions.Get(Capabilities.Git);
            _s3 = capabilityOptions.Get(Capabilities.S3);
            _authentication = capabilityOptions.Get(Capabilities.Authentication);
        }

        private List<BalsamProgram> GetPrograms()
        {
            var programs = new List<BalsamProgram>();
            var hubPath = Path.Combine(_hubRepositoryClient.Path, "hub");

            foreach (var programPath in Directory.GetDirectories(hubPath))
            {
                var propsFile = Path.Combine(programPath, "properties.json");
                var program = JsonConvert.DeserializeObject<BalsamProgram>(File.ReadAllText(propsFile));
                programs.Add(program);
            }
            return programs;
        }

        private bool ProgramExisits(string programName)
        {
            var programs = GetPrograms();
            if (programs.FirstOrDefault(p => p.Name == programName) == null)
            {
                return false;
            }
            return true;
        }

        public async Task<BalsamProgram> CreateProgram(string preferredName)
        {
            //Check if there is a program with the same name.
            if (ProgramExisits(preferredName))
            {
                return null;
            }

            var program = new BalsamProgram();
            program.Id = Guid.NewGuid().ToString();
            program.Name = preferredName;

            string programPath = Path.Combine(_hubRepositoryClient.Path, "hub", program.Id);
            DirectoryUtil.AssureDirectoryExists(programPath);

            var tasks = new List<Task>();
            Task<S3Data> s3Task = null;
            Task<GitData> gitTask = null;

            //TODO Implement
            if (_authentication.Enabled)
            {
                //TODO call CreateRole in the OicdProvider
            }

            if (_git.Enabled)
            {
                gitTask = _gitClient.CreateRepository(preferredName);
                tasks.Add(gitTask);
            }

            if (_s3.Enabled)
            {
                s3Task = _s3Client.CreateBucket(preferredName);
                tasks.Add(s3Task);
            }

            //wait for all task to finish
            await Task.WhenAll(tasks);

            //fetch and sstore the results for each provider
            if (s3Task != null)
            {
                program.S3 = s3Task.Result;
            }

            if (gitTask != null)
            {
                program.Git = gitTask.Result;
            }

            string propPath = Path.Combine(programPath, "properties.json");

            _hubRepositoryClient.PullChanges();
            // serialize JSON to a string and then write string to a file
            await File.WriteAllTextAsync(propPath, JsonConvert.SerializeObject(program));
            _hubRepositoryClient.PersistChanges($"New program with id {program.Id}");
            return program;
        }
    }
}
