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
                inputQueue, 
                portfolio, 
                trackedSymbolFactory);
                       
            IConnectedCondition connectedCondition = new ConnectedCondition();
            IRepository repository = new Repository(
                ibClient, 
                connectedCondition, 
                inputQueue,
                configurationManager);
                        
            ITrackedSymbols trackedSymbols = new TrackedSymbols();
            IReducer reducer = new Reducer();
            IUnderlyingConverter underlyingConverter = new UnderlyingConverter();
            IInputProcessor inputProcessor = new InputProcessor(
                reducer,
                portfolio,
                trackedSymbols,
                repository,
                underlyingConverter);
            
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
