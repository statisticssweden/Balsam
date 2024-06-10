namespace Balsam.Model
{
    public class S3Token
    {
        public string AccessKey { get; set; }
        public string SecretKey{ get; set; }

        public S3Token(string accessKey, string secretKey)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
        }
    }
}
