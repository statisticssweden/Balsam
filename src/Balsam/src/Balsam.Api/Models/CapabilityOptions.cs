using Microsoft.Extensions.Options;

namespace Balsam.Api.Models
{
    public class CapabilityOptions
    {
        public bool Enabled { get; set; }
        public string ServiceLocation { get; set; }
        public string[] Actions { get; set; }
    }

    public class Capabilities
    {
        public const string Git = "Git";
        public const string S3 = "S3";
        public const string Authentication = "Authentication";
    }

    public class ConfigureCapabilityOptions : IConfigureNamedOptions<CapabilityOptions>
    {

        public ConfigureCapabilityOptions()
        {
                
        }

        public void Configure(string name, CapabilityOptions options)
        {
            //options.ClientId = this.decrypt.Decrypt(options.ClientId);
            //options.ClientSecret = this.decrypt.Decrypt(options.ClientSecret);
        }

        public void Configure(CapabilityOptions options)
        {
            Configure(Options.DefaultName, options);
        }
    }
}
