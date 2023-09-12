using S3Provider.Models;
using System.Data.Common;

namespace MinIOS3Provider.Configuration
{
    public class ApiOptions
    {
        public const string SectionName = "Api";

        public string Domain { get; set; }
        public string Protocol { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        

    }
}
