using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;

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
            IInputProcessor inputProcessor = new InputProcessor();
            IConnectedCondition connectedCondition  = new ConnectedCondition();
            ITrackedSymbols trackedSymbols  = new TrackedSymbols(requestSender);
            IReducer reducer = new Reducer();
            IInputLoop inputLoop = new InputLoop(
                inputProcessor, 
                connectedCondition, 
                portfolio, 
                trackedSymbols, 
                reducer);

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
