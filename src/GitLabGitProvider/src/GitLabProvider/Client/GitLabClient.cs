using GitLabProvider.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace GitLabProvider.Client
{
    public partial class GitLabClient : IGitLabClient
    {
        private readonly string _baseUrl;
        private readonly string _accesstoken;
        private readonly string _namespaceId;
        private readonly string _templatePath;
        private readonly ILogger<GitLabClient> _logger;


        public GitLabClient(IOptions<ApiOptions> options, ILogger<GitLabClient> logger)
        {
            _baseUrl = options.Value.BaseUrl;
            _accesstoken = options.Value.PAT;
            _namespaceId = options.Value.GroupID;
            if (string.IsNullOrEmpty(options.Value.TemplatePath))
            {
                _templatePath = "/balsam/templates";
            } else
            {
                _templatePath = options.Value.TemplatePath;
            }
            _logger = logger;
        }

        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<RepositoryInfo?> CreateProjectRepo(string repoName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/v4/projects")
            {
                Content = new FormUrlEncodedContent(new KeyValuePair<string?, string?>[]
                {
                    new("name", repoName),
                    new("description", "project created from Balsam.UI"),
                    new("namespace_id", _namespaceId),
                    new("initialize_with_readme", "true")
                })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accesstoken);

            try
            {
                using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                RepositoryInfo? repoInfo = null;

                if (response.Content is object && response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (content == null) { 
                        return null;
                    }
                    dynamic createResponse = JsonConvert.DeserializeObject(content);
                    if (createResponse != null) {
                        repoInfo = new RepositoryInfo();

                        repoInfo.Id = createResponse.id;
                        repoInfo.Url = createResponse.http_url_to_repo;
                        repoInfo.Name = createResponse.name;

                        GitFilesToRepo(repoInfo.Id).Wait();
                    }
                }
                else
                {
                    _logger.LogWarning("HTTP Response was invalid and cannot be deserialised.");
                }

                return repoInfo;
            }
            
            
            catch (Exception ex)
            {
                _logger.LogError("Failed to create repository", ex);
            }
            return null;           
        }

        public async Task<bool> CreateBranch(string branchname, string repositoryId)
        {
            var projectId = repositoryId;

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/v4/projects/{projectId}/repository/branches")
                {
                    Content = new FormUrlEncodedContent(new KeyValuePair<string?, string?>[]
                    {
                        new("branch", branchname.ToLower()),
                        new("ref", "main")
                    })
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accesstoken);

                using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError("Could not create branch", ex);
            }

            return false;
        }

        public async Task<string?> GetUserID(string userName)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/v4/users?username={userName}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accesstoken);
                using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var responseContentStream = response.Content.ReadAsStringAsync().Result;
                var userID = JsonConvert.DeserializeObject<List<GitLabId>>(responseContentStream);
                return userID?.FirstOrDefault(item => item.Name == userName)?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not resolve user id for {userName}", ex);
            }
            return null;
        }

        public async Task<string?> CreatePAT(string userName)
        {
            var userId = await GetUserID(userName);
            if (userId == null) return null;

            try { 
                var patName = "token";
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/v4/users/{userId}/personal_access_tokens")
                {
                    Content = new FormUrlEncodedContent(new KeyValuePair<string?, string?>[]
                    {
                        new("name", patName),
                        new("expires_at", DateTime.Now.AddMonths(12).ToString("yyyy-MM-dd")),
                        new("scopes", "api")
                    })
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accesstoken);
                using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var responseContentStream = response.Content.ReadAsStringAsync().Result;
                var patToken = JsonConvert.DeserializeObject<GitLabPAT>(responseContentStream);

                return patToken?.Token;
            } 
            catch (Exception ex)
            {
                _logger.LogError($"Could not create token for user: {userName}({userId})", ex);
            }
            return null;
        }

        public async Task GitFilesToRepo(string repositoryId)
        {
            try
            {
                var path = _templatePath;

                if (!Directory.Exists(path))
                {
                    _logger.LogWarning($"Path {path} does not exist. Can not initiate git repo files");
                    return;
                }

                var directoryInfo = new DirectoryInfo(path);

                var actions = new List<Action>();

                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    var bytes = await File.ReadAllBytesAsync(fileInfo.FullName);
                    var file = Convert.ToBase64String(bytes);
                    actions.Add(new Action { action = "create", file_path = fileInfo.Name, encoding = "base64", content = file });
                }

                var gitContent = new GitContent
                {
                    branch = "main",
                    commit_message = "commit",
                    actions = actions
                };

                var jsonGitContent = JsonConvert.SerializeObject(gitContent);

                var projectId = repositoryId;
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/v4/projects/{projectId}/repository/commits")
                {
                    Content = new StringContent(jsonGitContent, Encoding.UTF8, "application/json")

                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accesstoken);

                using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex) {
                _logger.LogError("Could not initiate repo with files, due to error", ex);
            }
        }
    }
}
