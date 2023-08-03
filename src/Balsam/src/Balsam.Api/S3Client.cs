using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;

namespace Balsam.Api
{
    public class S3Client
    {
        private HttpClient _httpClient;

        private string _baseUrl = "http://s3-provider.balsam-system.svc.cluster.local/api/v1/buckets";
        public S3Client(HttpClient httpClient) {
            _httpClient = httpClient;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = TimeSpan.FromSeconds(30);


        }


        public async Task CreateRepository(string repositoryName)
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}?preferredName={repositoryName}", null);
            response.EnsureSuccessStatusCode();

        }

    }
}
