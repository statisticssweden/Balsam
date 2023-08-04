using Balsam.Api.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace Balsam.Api
{
    public class S3Client
    {
        private HttpClient _httpClient;

        //private string _baseUrl = "http://s3-provider.balsam-system.svc.cluster.local/api/v1/buckets";
        private string _baseUrl = "http://localhost:8080/api/v1/buckets";
        public S3Client(HttpClient httpClient) {
            _httpClient = httpClient;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = TimeSpan.FromSeconds(30);


        }


        public async Task<S3Data> CreateRepository(string repositoryName)
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}?preferredName={repositoryName}", null);
            response.EnsureSuccessStatusCode();

            S3Data data = new S3Data();

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
                    

                    data.BucketName = createResponse.name;
                }
            }
            return data;
        }
    }
}
