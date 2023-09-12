namespace MinIOS3Provider.Client
{
    public interface IMinioS3Client
    {
        Task CreateBucket(string bucketName);
        void CreatePolicy(string programName);
        void CreateUser(string programName);
        Task CreateDirectory(string bucket, string directory);
        KeyPair CreateAccessKey(string programName);
    }
}
