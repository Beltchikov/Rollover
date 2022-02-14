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

            IIbClientWrapper ibClient = new IbClientWrapper(ibClientQueue);

            IConnectedCondition connectedCondition = new ConnectedCondition();
            IInputQueue inputQueue = new InputQueue();
            IMessageProcessor messageProcessor = new MessageProcessor(portfolio);
            IMessageCollector messageCollector = new MessageCollector(
                ibClient,
                connectedCondition,
                ibClientQueue,
                configurationManager);
            IRepositoryHelper repositoryHelper = new RepositoryHelper();
            IRepository repository = new Repository(
                ibClient,
                messageProcessor,
                messageCollector,
                repositoryHelper);

            ITrackedSymbols trackedSymbols = new TrackedSymbols(fileHelper, serializer);
            IOrderManager orderManager = new OrderManager(repository);
            IInputProcessor inputProcessor = new InputProcessor(
                portfolio,
                trackedSymbols,
                repository);

            IInputLoop inputLoop = new InputLoop(inputProcessor, messageProcessor);

            ITwsConnector twsConnector = new TwsConnector(inputQueue, repository);
            IRolloverAgent rolloverAgent = new RolloverAgent(
                configurationManager,
                consoleWrapper,
                inputQueue,
                ibClientQueue,
                repository,
                inputLoop,
                twsConnector,
                portfolio,
                trackedSymbols,
                orderManager);

            rolloverAgent.Run();
        }
    }
}
