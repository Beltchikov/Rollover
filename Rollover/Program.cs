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
            IOutputHelper outputHelper = new OutputHelper();
            IInputLoop inputLoop = new InputLoop(outputHelper);

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
