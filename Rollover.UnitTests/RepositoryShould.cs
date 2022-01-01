using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Rollover.UnitTests
{
    public class RepositoryShould
    {
        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorEConnectInConnect(
            [Frozen] IMessageCollector messageCollector,
            Repository sut)
        {
            string host = "host1";
            int port = 398;
            int clientId = 29;

            sut.Connect(host, port, clientId);
            messageCollector.Received().eConnect(host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientDisconnect(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            sut.Disconnect();
            ibClient.Received().eDisconnect();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientListPositionsInAllPositions(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            sut.AllPositions();
            ibClient.Received().reqPositions();
        }

        [Fact]
        public void ReturnPositionsInAllPositions()
        {
            var timeout = 1000;

            var contract1 = new Contract();
            var positionMessage1 = new PositionMessage("account", contract1, 1, 1000);

            var contract2 = new Contract();
            var positionMessage2 = new PositionMessage("account", contract2, 2, 2000);

            var ibClientQueue = Substitute.For<IIbClientQueue>();
            ibClientQueue.Dequeue().Returns(positionMessage1, positionMessage2, Constants.ON_POSITION_END);

            var ibClinet = Substitute.For<IIbClientWrapper>();
            ibClinet.When(c => c.reqContractDetails(Arg.Any<int>(), Arg.Any<Contract>()))
                .Do(c => { });

            var configurationManager = Substitute.For<IConfigurationManager>();

            var trackedSymbolFactory = new TrackedSymbolFactory();
            var portfolio = new Portfolio();
            var messageProcessor = new MessageProcessor(portfolio);

            Configuration.Configuration configuration = new Configuration.Configuration
            { Timeout = timeout };
            configurationManager.GetConfiguration().Returns(configuration);

            var sut = new Repository(ibClinet, messageProcessor, null, null);
            var resultList = sut.AllPositions();

            Assert.Equal(3, resultList.Count);
        }

        [Fact]
        public void ReturnNullIfNoTrackedSymbolAfterTimeout()
        {
            var timeout = 1000;

            var ibClientQueue = Substitute.For<IIbClientQueue>();
            var ibClinet = Substitute.For<IIbClientWrapper>();
            ibClinet.When(c => c.reqContractDetails(Arg.Any<int>(), Arg.Any<Contract>()))
                .Do(c => { });

            var configurationManager = Substitute.For<IConfigurationManager>();
            var contract = new Contract();

            Configuration.Configuration configuration = new Configuration.Configuration
            { Timeout = timeout };
            configurationManager.GetConfiguration().Returns(configuration);

            var trackedSymbolFactory = new TrackedSymbolFactory();
            var portfolio = new Portfolio();
            var messageProcessor = new MessageProcessor(portfolio);

            var sut = new Repository(ibClinet, messageProcessor, null, null);
            var trackedSymbol = sut.GetTrackedSymbol(contract);
            Thread.Sleep(timeout);

            Assert.Null(trackedSymbol);
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorEConnect(
            [Frozen] IMessageCollector messageCollector,
            [Frozen] IConfigurationManager configurationManager)
        {
            IRepository sut = new Repository(null, null, messageCollector, null);
            sut.Connect("localhost", 4001, 1);
            messageCollector.Received().eConnect("localhost", 4001, 1);
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorReqPositions(
            [Frozen] IMessageCollector messageCollector,
            [Frozen] IConfigurationManager configurationManager)
        {
            IRepository sut = new Repository(null, null, messageCollector, null);
            sut.AllPositions();
            messageCollector.Received().reqPositions();
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorReqContractDetails(
            [Frozen] IMessageCollector messageCollector,
            [Frozen] IConfigurationManager configurationManager,
            [Frozen] Contract contract)
        {
            IRepository sut = new Repository(null, null, messageCollector, null);
            sut.GetTrackedSymbol(contract);
            messageCollector.Received().reqContractDetails(contract);
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorReqSecDefOptParams(
            [Frozen] IMessageCollector messageCollector,
            [Frozen] IConfigurationManager configurationManager,
            [Frozen] ITrackedSymbolFactory trackedSymbolFactory)
        {
            var contract = new Contract
            {
                Symbol = "MNQ",
                Currency = "USD",
                Exchange = "GLOBEX",
                LastTradeDateOrContractMonth = "20220318"
            };
            var contractDetails = new ContractDetails
            { Contract = contract, UnderSecType = "FUT" };
            contractDetails.Contract = contract;
            var contractDetailsMessage = new ContractDetailsMessage(1, contractDetails);
            var contractDetailsMessageList = new List<ContractDetailsMessage> { contractDetailsMessage };
            messageCollector.reqContractDetails(Arg.Any<Contract>()).Returns(contractDetailsMessageList);

            var expirations = new HashSet<string> { "20220318" };
            var strikes = new HashSet<double> { 980, 90, 100, 110, 120 };
            var secDefOptParamMessageList = new List<SecurityDefinitionOptionParameterMessage> {
            new SecurityDefinitionOptionParameterMessage(1,"",123, "", "", expirations, strikes)
            };
            messageCollector.reqSecDefOptParams(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
                .Returns(secDefOptParamMessageList);

            IRepository sut = new Repository(null, null, messageCollector, trackedSymbolFactory);
            sut.GetTrackedSymbol(contract);
            messageCollector.Received().reqSecDefOptParams(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<int>());
        }
    }
}
