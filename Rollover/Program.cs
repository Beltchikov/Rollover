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
            
            IRolloverAgent rolloverAgent = new RolloverAgent(configurationManager);
            rolloverAgent.Run();
        }
    }
}
