using Microsoft.Extensions.Options;
using System.Diagnostics;
using Minio;
using Minio.Exceptions;
using MinIOS3Provider.Configuration;

namespace MinIOS3Provider.Client
{
    public class MinioS3Client : IMinioS3Client
    {
        private readonly ILogger _logger;
        private readonly string _apiUrl;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _directoryStructure;

        public MinioS3Client(IOptions<ApiOptions> options, ILogger<MinioS3Client> logger)
        {
            _logger = logger;
            var api = options.Value;
            _apiUrl = api.Domain; 
            _accessKey = api.AccessKey; 
            _secretKey = api.SecretKey; 
            //_directoryStructure = options.Value.S3DirectoryStructure; 
            var s3Mc = $"{api.Protocol}://{api.Domain}";

            var cmd = $"alias set balsam {s3Mc} {_accessKey} {_secretKey}";

            var t = MC(cmd);

            if (t.Status == TaskStatus.Faulted)
            {
                _logger.LogError("Can not initialize mc");
                return;
            }

            var i = t.Result;

            if (i != 0)
            {
                _logger.LogWarning("Could not create alias balsam for mc, mc did not return 0");
            }
        }

        /// <summary>
        /// Creates a new bucket with the name of bucketName asynchronous . Before creatinf the bucket it checks that a bucket with the same name does not exist
        /// </summary>
        /// <param name="bucketName">The name of the bucket</param>
        /// <returns>A task</returns>
        public async Task CreateBucket(string bucketName)
        {
            try
            {
                var client = new MinioClient()
                    .WithEndpoint(_apiUrl)
                    .WithCredentials(accessKey: _accessKey, _secretKey)
                    .Build();

                var existArgs = new BucketExistsArgs().WithBucket(bucketName);
                var found = await client.BucketExistsAsync(existArgs);
                if (found)
                {
                    _logger.LogWarning("Bucket already exists! {bucketName}", bucketName);

                }
                else
                {
                    var makeArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await client.MakeBucketAsync(makeArgs);
                    _logger.LogInformation("S3 Bucket created: {bucketName}", bucketName);
                }
            }
            catch (MinioException e)
            {
                _logger.LogError(e, "Bucket creation failed for : {bucketName}", bucketName);
            }
        }

        /// <summary>
        /// Creates a virtual directory in a bucket and places a readme.txt as dummy file.
        /// </summary>
        /// <param name="bucket">name of the bucket</param>
        /// <param name="directory">name of the virtual directory</param>
        /// <returns></returns>
        public async Task CreateDirectory(string bucket, string directory)
        {
            var fileName = "readme.txt";
            try
            {
                var client = new MinioClient()
                    .WithEndpoint(_apiUrl)
                    .WithCredentials(accessKey: _accessKey, _secretKey)
                    .Build();

                var existArgs = new BucketExistsArgs().WithBucket(bucket);
                var found = await client.BucketExistsAsync(existArgs);
                if (!found)
                {
                    _logger.LogError("{Bucket} does not exists!", bucket);
                    return;
                }

                var directoryName = directory; //.Replace(@"{PROJECT_NAME}", project);
                try
                {
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucket)
                        .WithObject($"{directoryName}/{fileName}")
                        .WithFileName(fileName)
                        .WithContentType("application/octet-stream");
                    await client.PutObjectAsync(putObjectArgs);
                    _logger.LogInformation("Folder template created for {Bucket} {Directory}", bucket, directoryName);
                }
                catch (MinioException e)
                {
                    Console.WriteLine("Error occurred: " + e);
                    _logger.LogError(e, "Failed to create bucket template folder: '{DirectoryName}' for bucket: '{Bucket}'", directoryName, bucket);
                }
                

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create bucket template for {bucket}", bucket);
            }
        }

        private Task<int> MC(string cmd)
        {
            var source = new TaskCompletionSource<int>();
            var escapedArgs = cmd.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "mc",
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            process.Exited += (sender, args) =>
            {
                var standardError = process.StandardError.ReadToEnd();
                _logger.LogWarning("{standardError}", standardError);

                var readToEnd = process.StandardOutput.ReadToEnd();
                _logger.LogInformation("{readToEnd}", readToEnd);

                if (process.ExitCode == 0)
                {
                    source.SetResult(0);
                }
                else
                {
                    source.SetException(new Exception($"mc failed with exit code `{process.ExitCode}`"));
                }

                process.Dispose();
            };

            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Command failed: '{cmd}' ", cmd);
                source.SetException(e);
            }

            return source.Task;
        }

        /// <summary>
        /// Creates a policy for full access to a bucket.
        /// </summary>
        /// <param name="bucketName">name of the bucket</param>
        public void CreatePolicy(string bucketName)
        {
            var jsonPolicy = GetJsonPolicy(bucketName);
            var policyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.WriteAllText(policyPath, jsonPolicy);

            var cmd = $"admin policy add balsam {bucketName}-users {policyPath}";
            var i = MC(cmd).Result;

            if (i != 0)
            {
                _logger.LogWarning("Could not create policy, mc did not return 0");
            }
        }

        /// <summary>
        /// Creates a service account and adds that user to the bucket policy.
        /// Note that a bucket policy with the name bucketname-users must exisit
        /// </summary>
        /// <param name="bucketName"></param>
        public void CreateUser(string bucketName)
        {
            var secret = TokenGenerate();
            var userName = $"svc-{bucketName}";
            var cmd = $"admin user add balsam {userName} {secret}";
            var i = MC(cmd).Result;

            AddPolicyUser(bucketName, userName);
        }

        /// <summary>
        /// Adds a user to a existing policy by the naming rule of the policy as <bucketName>-user
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        private void AddPolicyUser(string bucketName, string userName)
        {
            var cmd = $"admin policy set balsam {bucketName}-users user={userName}";
            var i = MC(cmd).Result;
        }

        /// <summary>
        /// Creates access keys for a service account named svc-<bucket>.
        /// The service account must exist beforehand.
        /// </summary>
        /// <param name="bucket">name of the bucket</param>
        /// <returns></returns>
        public KeyPair CreateAccessKey(string bucket)
        {
            var jsonPolicy = GetJsonPolicy(bucket);
            var policyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.WriteAllText(policyPath, jsonPolicy);

            var secretKey = TokenGenerate();
            var accessKey = TokenGenerate();
            var cmd = $"admin user svcacct add --access-key {accessKey} --secret-key {secretKey} balsam svc-{bucket}";
            var i = MC(cmd).Result;

            if (i != 0)
            {
                _logger.LogWarning("Could not create policy, mc did not return 0");
            }
            return new KeyPair(secretKey, accessKey);
        }

        private static string GetJsonPolicy(string programName)
        {
            var jsonPolicy = @$"{{
                    ""Version"": ""2012-10-17"",
                    ""Statement"": [
                        {{
                            ""Effect"": ""Allow"",
                            ""Action"": [
                                ""s3:ListBucket""
                            ],
                            ""Resource"": [
                                ""arn:aws:s3:::{programName}""
                            ]
                        }},
                        {{
                            ""Effect"": ""Allow"",
                            ""Action"": [
                                ""s3:*""
                            ],
                            ""Resource"": [
                                ""arn:aws:s3:::{programName}/*""
                            ]
                        }}
                    ]
                }}";
            return jsonPolicy;
        }

        private static string TokenGenerate()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalToken = new String(stringChars);
            return finalToken;
        }
    }
}
