using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Threading;

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
           
            IIbClientWrapper ibClient = new IbClientWrapper(new SynchronizationContext(), inputQueue, portfolio);
            IConnectedCondition connectedCondition = new ConnectedCondition();
            IRequestSender requestSender = new RequestSender(ibClient, connectedCondition);
            ITrackedSymbols trackedSymbols = new TrackedSymbols(requestSender);
            IReducer reducer = new Reducer();
            IInputProcessor inputProcessor = new InputProcessor(
                reducer,
                portfolio,
                trackedSymbols);
            
            IInputLoop inputLoop = new InputLoop(inputProcessor,connectedCondition);

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
