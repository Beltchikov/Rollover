using Rollover.Configuration;
using Rollover.Ib;
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
            IRequestSender requestSender = new RequestSender();

            IRolloverAgent rolloverAgent = new RolloverAgent(
                configurationManager, 
                consoleWrapper,
                inputQueue,
                requestSender);
            rolloverAgent.Run();
        }
    }
}
