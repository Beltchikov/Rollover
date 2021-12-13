using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using System;

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

            IIbClientWrapper ibClient = new IbClientWrapper();
            IRequestSender requestSender = new RequestSender(ibClient);
            IInputLoop inputLoop = new InputLoop();

            IRolloverAgent rolloverAgent = new RolloverAgent(
                configurationManager,
                consoleWrapper,
                inputQueue,
                requestSender,
                inputLoop);

            rolloverAgent.Run();
        }
    }
}
