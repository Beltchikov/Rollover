namespace SsbHedger.SsbConfiguration
{
    public interface IRegistryManager
    {
        public ConfigurationData ReadConfiguration(ConfigurationData defaultConfigurationData);
        void WriteConfiguration(ConfigurationData defaultConfigurationData);
    }
}