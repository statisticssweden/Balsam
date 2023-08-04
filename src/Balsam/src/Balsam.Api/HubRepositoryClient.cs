using Balsam.Api.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace Balsam.Api
{
    public class HubRepositoryClient
    {

        private readonly string _repositoryPath;
        private readonly Repository _repository;
        private readonly string _userId;
        private readonly string _password;
        private readonly string _mail;
        private readonly ILogger _logger;

        public string Path { get { return _repositoryPath; } }

        public HubRepositoryClient(IOptions<HubRepositoryOptions> options, ILogger<HubRepositoryClient> logger) {

            _logger = logger;
            _userId = options.Value.User;
            _password = options.Value.Password;
            _mail = options.Value.Mail??"noreply@balsam.local";

            //Asure credentials
            if (string.IsNullOrEmpty(_userId) || string.IsNullOrEmpty(_password)) {
                _logger.LogWarning("Missing hub repository credentials");
            }

            //Prepare local folder
            string basePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "balsam");
            if (!DirectoryUtil.AssureDirectoryExists(basePath))
            {
                Directory.Delete(basePath, true);
            }

            //Clone repo
            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = _userId, Password = _password }
            };

            _repositoryPath = Repository.Clone(options.Value.RemoteUrl, basePath, cloneOptions);
            _repository = new Repository(_repositoryPath);
            _repositoryPath = basePath;
        }

        
        public void PersistChanges(string commitMessage)
        {
            var pushOptions = new PushOptions();

            pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = _userId, Password = _password };

            var namesig = new Signature(_userId, _mail, DateTimeOffset.Now);

            Commands.Stage(_repository, "*");

            _repository.Commit(commitMessage, namesig, namesig);

            var remote = _repository.Network.Remotes["origin"];

            var pushRefSpec = @"refs/heads/master";
            _repository.Network.Push(remote, pushRefSpec, pushOptions);
        }

        public void PullChanges()
        {
            var pullOptions = new PullOptions();

            pullOptions.FetchOptions = new FetchOptions();
            pullOptions.FetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = _userId, Password = _password };

            var signature = new Signature(_userId, _mail, DateTimeOffset.Now);
            Commands.Pull(_repository, signature, pullOptions);
        }

        

    }
}
