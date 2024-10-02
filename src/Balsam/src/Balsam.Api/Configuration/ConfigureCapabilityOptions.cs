using Microsoft.Extensions.Options;

namespace Balsam.Model
{
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
