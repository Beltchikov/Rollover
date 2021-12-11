using Rollover.Configuration;
using Rollover.Input;

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
            IConsoleWrapper consoleWrapper = new ConsoleWrapper();
            IInputQueue inputQueue = new InputQueue();

            IRolloverAgent rolloverAgent = new RolloverAgent(
                configurationManager, 
                consoleWrapper,
                inputQueue);
            rolloverAgent.Run();
        }
    }
}
