using SsbHedger.Configuration;

namespace SsbHedger.RegistryManager
{
    public interface IRegistryManager
    {
        public ConfigurationData ReadConfiguration(ConfigurationData defaultConfigurationData);
        void WriteConfiguration(ConfigurationData defaultConfigurationData);
    }
}