using Balsam.Api.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Balsam.Api
{
    public class GitClient
    {
        private HttpClient _httpClient;

        //private string _baseUrl = "http://git-provider.balsam-system.svc.cluster.local/api/v1";
        private string _baseUrl = "http://localhost:8081/api/v1";
        public GitClient(IOptionsSnapshot<CapabilityOptions> capabilityOptions, HttpClient httpClient)
        {
            var gitOptions = capabilityOptions.Get(Capabilities.Git);
            _baseUrl = gitOptions.ServiceLocation;
            _httpClient = httpClient;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }


        public async Task<GitData> CreateRepository(string repositoryName)
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}/repos?preferredName={repositoryName}", null);
            response.EnsureSuccessStatusCode();

            GitData data = new GitData();

            if (response.Content is object && response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType == "application/json")
            {
                string content = await response.Content.ReadAsStringAsync();
                if (content == null)
                {
                    return null;
                }
                dynamic createResponse = JsonConvert.DeserializeObject(content);
                if (createResponse != null)
                {
                    data.Name = createResponse.name;
                    data.Path = createResponse.path;
                }
            }
            return data;
        }
    }
}