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

            ITrackedSymbolFactory trackedSymbolFactory = new TrackedSymbolFactory();
            IIbClientWrapper ibClient = new IbClientWrapper(
                inputQueue, 
                portfolio, 
                trackedSymbolFactory);
                       
            IConnectedCondition connectedCondition = new ConnectedCondition();
            IQueryParametersConverter queryParametersConverter = new QueryParametersConverter();
            IRepository repository = new Repository(
                ibClient, 
                connectedCondition, 
                inputQueue,
                configurationManager,
                queryParametersConverter);
                        
            ITrackedSymbols trackedSymbols = new TrackedSymbols();
            IReducer reducer = new Reducer();
            IInputProcessor inputProcessor = new InputProcessor(
                reducer,
                portfolio,
                trackedSymbols,
                repository);
            
            IInputLoop inputLoop = new InputLoop(inputProcessor);

            IRolloverAgent rolloverAgent = new RolloverAgent(
                configurationManager,
                consoleWrapper,
                inputQueue,
                repository,
                inputLoop);

            rolloverAgent.Run();
        }
    }
}
