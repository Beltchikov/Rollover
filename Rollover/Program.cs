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

            ITrackedSymbolFactory trackedSymbolFactory = new TrackedSymbolFactory();
            IIbClientWrapper ibClient = new IbClientWrapper(
                new SynchronizationContext(), 
                inputQueue, 
                portfolio, 
                trackedSymbolFactory);
                       
            IConnectedCondition connectedCondition = new ConnectedCondition();
            IRepository repository = new Repository(
                ibClient, 
                connectedCondition, 
                consoleWrapper,
                inputQueue,
                configurationManager);
                        
            ITrackedSymbols trackedSymbols = new TrackedSymbols(repository);
            IReducer reducer = new Reducer();
            IInputProcessor inputProcessor = new InputProcessor(
                reducer,
                portfolio,
                trackedSymbols,
                repository);
            
            IInputLoop inputLoop = new InputLoop(inputProcessor,connectedCondition);

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
