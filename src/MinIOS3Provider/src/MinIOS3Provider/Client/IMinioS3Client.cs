namespace MinIOS3Provider.Client
{
    public interface IMinioS3Client
    {
        Task CreateBucket(string bucketName);
        Task DeleteBucket(string bucketName);
        void CreatePolicy(string programName, string bucketName);
        void CreateUser(string programName, string policyName);
        Task CreateDirectory(string bucket, string directory);
        Task DeleteDirectory(string bucket, string directory);
        KeyPair CreateAccessKey(string programName);
    }
}
