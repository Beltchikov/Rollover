using Rollover.Configuration;
using Rollover.Helper;
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
            IIbClientQueue ibClientQueue = new IbClientQueue();
            IPortfolio portfolio = new Portfolio();

            ITrackedSymbolFactory trackedSymbolFactory = new TrackedSymbolFactory();
            IIbClientWrapper ibClient = new IbClientWrapper(ibClientQueue);

            IConnectedCondition connectedCondition = new ConnectedCondition();
            IInputQueue inputQueue = new InputQueue();
            IMessageProcessor messageProcessor = new MessageProcessor(portfolio);
            IMessageCollector messageCollector = new MessageCollector(
                ibClient,
                connectedCondition,
                ibClientQueue,
                configurationManager);
            IRepository repository = new Repository(
                ibClient,
                messageProcessor,
                messageCollector,
                trackedSymbolFactory);

            ITrackedSymbols trackedSymbols = new TrackedSymbols(fileHelper, serializer);
            IReducer reducer = new Reducer();
            IInputProcessor inputProcessor = new InputProcessor(
                reducer,
                portfolio,
                trackedSymbols,
                repository,
                trackedSymbolFactory);

            IInputLoop inputLoop = new InputLoop(inputProcessor, messageProcessor);

            ITwsConnector twsConnector = new TwsConnector(configurationManager, consoleWrapper, inputQueue, repository);
            IRolloverAgent rolloverAgent = new RolloverAgent(
                configurationManager,
                consoleWrapper,
                inputQueue,
                ibClientQueue,
                repository,
                inputLoop,
                twsConnector,
                portfolio);

            rolloverAgent.Run();
        }
    }
}
