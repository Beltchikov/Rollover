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
            IIbClientQueue ibClientQueue = new Ib.IbClientQueue();
            IPortfolio portfolio = new Portfolio();

            ITrackedSymbolFactory trackedSymbolFactory = new TrackedSymbolFactory();
            IIbClientWrapper ibClient = new IbClientWrapper(ibClientQueue);
                       
            IConnectedCondition connectedCondition = new ConnectedCondition();
            IInputQueue inputQueue = new InputQueue();
            IQueryParametersConverter queryParametersConverter = new QueryParametersConverter();
            IMessageProcessor messageProcessor = new MessageProcessor(trackedSymbolFactory);
            IRepository repository = new Repository(
                ibClient, 
                connectedCondition,
                ibClientQueue,
                configurationManager,
                queryParametersConverter,
                messageProcessor,
                portfolio);
                        
            ITrackedSymbols trackedSymbols = new TrackedSymbols();
            IReducer reducer = new Reducer();
            IInputProcessor inputProcessor = new InputProcessor(
                reducer,
                portfolio,
                trackedSymbols,
                repository,
                trackedSymbolFactory);
            
            IInputLoop inputLoop = new InputLoop(inputProcessor, messageProcessor);

            IRolloverAgent rolloverAgent = new RolloverAgent(
                configurationManager,
                consoleWrapper,
                inputQueue,
                ibClientQueue,
                repository,
                inputLoop);

            rolloverAgent.Run();
        }
    }
}
