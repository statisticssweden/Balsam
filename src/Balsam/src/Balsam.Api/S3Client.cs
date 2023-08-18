using Balsam.Api.Models;
using Microsoft.Extensions.Options;
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
        private string _baseUrl = "http://localhost:8080/api/v1";
        public S3Client(IOptionsSnapshot<CapabilityOptions> capabilityOptions, HttpClient httpClient) {
            var s3Options = capabilityOptions.Get(Capabilities.S3);
            _baseUrl = s3Options.ServiceLocation;
            _httpClient = httpClient;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = TimeSpan.FromSeconds(30);

        }


        public async Task<S3Data> CreateBucket(string bucketName)
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}/buckets?preferredName={bucketName}", null);
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
