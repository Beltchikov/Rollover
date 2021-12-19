using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
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
            IPortfolio portfolio = new Portfolio();
            IResponseHandlers responseHandlers = ResponseHandlers.CreateInstance(inputQueue, portfolio);

            IIbClientWrapper ibClient = new IbClientWrapper(responseHandlers);
            IRequestSender requestSender = new RequestSender(ibClient);
            IOutputHelper outputHelper = new OutputHelper();
            IConnectedCondition connectedCondition  = new ConnectedCondition();
            IInputLoop inputLoop = new InputLoop(outputHelper, connectedCondition);

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
