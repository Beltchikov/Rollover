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
            IInputQueue inputQueue = new InputQueue();
            
            IRolloverAgent rolloverAgent = new RolloverAgent(configurationManager, inputQueue);
            rolloverAgent.Run();
        }
    }
}
