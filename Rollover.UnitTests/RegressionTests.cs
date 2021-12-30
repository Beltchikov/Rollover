using IBApi;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class RegressionTests
    {
        [Fact]
        public void GetTrackedSymbol()
        {
            //var timeout = 1000;
            //Configuration.Configuration configuration = new Configuration.Configuration
            //{ Timeout = timeout };
            //var configurationManager = Substitute.For<IConfigurationManager>();
            //configurationManager.GetConfiguration().Returns(configuration);

            //IConsoleWrapper consoleWrapper = new ConsoleWrapper();
            //IInputQueue inputQueue = new InputQueue();
            //IIbClientQueue ibClientQueue = new IbClientQueue();

            //IIbClientWrapper ibClient = new IbClientWrapper(ibClientQueue);
            //IRepository repository = new Repository(
            //    ibClient,
            //    connectedCondition,
            //    ibClientQueue,
            //    configurationManager,
            //    queryParametersConverter,
            //    messageProcessor);

            //IRolloverAgent rolloverAgent = new RolloverAgent(
            //    configurationManager,
            //    consoleWrapper,
            //    inputQueue,
            //    ibClientQueue,
            //    repository,
            //    inputLoop);

            
            /////////////////////////////////////////////////////////////
            
            //var timeout = 1000;
            //Configuration.Configuration configuration = new Configuration.Configuration
            //{ Timeout = timeout };
            //var configurationManager = Substitute.For<IConfigurationManager>();
            //configurationManager.GetConfiguration().Returns(configuration);

            //var contract = new Contract();
            //var ibClientQueue = new IbClientQueue();
            //var ibClinet = new IbClientWrapper(ibClientQueue);
            //var trackedSymbolFactory = new TrackedSymbolFactory();
            //var portfolio = new Portfolio();
            //var messageProcessor = new MessageProcessor(trackedSymbolFactory, portfolio);

            //contract.SecType = "FOP";
            //contract.Symbol = "MNQ";
            //contract.Currency = "USD";
            //contract.Exchange = null;

            //contract.LocalSymbol = "MNQH2 C1650";
            //contract.Strike = 16500;
            //contract.LastTradeDateOrContractMonth = "20220318";
            //contract.ConId = 515971773;


            //var sut = new Repository(ibClinet, null, ibClientQueue, configurationManager, null, messageProcessor);
            //ITrackedSymbol trackedSymbol = sut.GetTrackedSymbol(contract);
        }
    }
}
