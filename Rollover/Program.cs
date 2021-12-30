using Rollover.Configuration;

namespace Rollover
{
    class Program
    {
        static void Main(string[] args)
        {
            IFileHelper fileHelper = new FileHelper();
            ISerializer serializer = new Serializer();
            IConfigurationManager configurationManager = new ConfigurationManager(
                fileHelper, serializer);
            IRolloverAgentFactory rolloverAgentFactory = new RolloverAgentFactory(configurationManager);

            IRolloverAgent rolloverAgent = rolloverAgentFactory.CreateInstance();

            rolloverAgent.Run();
        }
    }
}
