using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;

namespace Rollover
{
    public class RolloverAgentFactory : IRolloverAgentFactory
    {
        private readonly IConfigurationManager _configurationManager;

        public RolloverAgentFactory(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public IRolloverAgent CreateInstance()
        {
            IConsoleWrapper consoleWrapper = new ConsoleWrapper();
            IIbClientQueue ibClientQueue = new IbClientQueue();
            IPortfolio portfolio = new Portfolio();

            ITrackedSymbolFactory trackedSymbolFactory = new TrackedSymbolFactory();
            IIbClientWrapper ibClient = new IbClientWrapper(ibClientQueue);

            IConnectedCondition connectedCondition = new ConnectedCondition();
            IInputQueue inputQueue = new InputQueue();
            IQueryParametersConverter queryParametersConverter = new QueryParametersConverter();
            IMessageProcessor messageProcessor = new MessageProcessor(portfolio);
            IMessageCollector messageCollector = new MessageCollector(
                ibClient,
                connectedCondition,
                ibClientQueue,
                _configurationManager);
            IRepository repository = new Repository(
                ibClient,
                ibClientQueue,
                _configurationManager,
                queryParametersConverter,
                messageProcessor,
                messageCollector,
                trackedSymbolFactory);

            ITrackedSymbols trackedSymbols = new TrackedSymbols();
            IReducer reducer = new Reducer();
            IInputProcessor inputProcessor = new InputProcessor(
                reducer,
                portfolio,
                trackedSymbols,
                repository,
                trackedSymbolFactory);

            IInputLoop inputLoop = new InputLoop(inputProcessor, messageProcessor);

            ITwsConnector twsConnector = new TwsConnector(_configurationManager, consoleWrapper, inputQueue, repository);
            IRolloverAgent rolloverAgent = new RolloverAgent(
                _configurationManager,
                consoleWrapper,
                inputQueue,
                ibClientQueue,
                repository,
                inputLoop,
                twsConnector,
                portfolio);
            
            return rolloverAgent;
        }
    }
}