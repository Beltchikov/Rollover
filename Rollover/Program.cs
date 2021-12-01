namespace Rollover
{
    class Program
    {
        static void Main(string[] args)
        {
            IFileHelper fileHelper = new FileHelper();
            IConfigurationManager configurationManager = new ConfigurationManager(fileHelper);
            
            IRolloverAgent rolloverAgent = new RolloverAgent(configurationManager);
            
            rolloverAgent.Run();
        }
    }
}
