namespace MinIOS3Provider.Client
{
    public class KeyPair
    {
        public KeyPair(string secretKey, string accessKey)
        {
            SecretKey = secretKey;
            AccessKey = accessKey;
        }

        public string AccessKey { get; }
        public string SecretKey { get; }
    }
}
