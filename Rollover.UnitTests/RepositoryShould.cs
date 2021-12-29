using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Threading;
using Xunit;

namespace Rollover.UnitTests
{
    public class RepositoryShould
    {
        [Theory, AutoNSubstituteData]
        public void CallIbClientConnectInConnect(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            string host = "host1";
            int port = 398;
            int clientId = 29;

            sut.Connect(host, port, clientId);
            ibClient.Received().Connect(host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientReaderFactoryInConnect(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            sut.Connect("", 0, 0);
            ibClient.Received().ReaderFactory();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientIsConnectedInConnect(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            sut.Connect("", 0, 0);
            ibClient.Received().IsConnected();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientWaitForSignalConnect(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            ibClient.IsConnected().Returns(true);
            sut.Connect("", 0, 0);
            ibClient.Received().WaitForSignal();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientDisconnect(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            sut.Disconnect();
            ibClient.Received().Disconnect();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientListPositionsInAllPositions(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            sut.AllPositions();
            ibClient.Received().ListPositions();
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
            ibClinet.When(c => c.ContractDetails(Arg.Any<int>(), Arg.Any<Contract>()))
                .Do(c => { });

            var configurationManager = Substitute.For<IConfigurationManager>();
            
            var trackedSymbolFactory = new TrackedSymbolFactory();
            var portfolio = new Portfolio();
            var messageProcessor = new MessageProcessor(trackedSymbolFactory, portfolio);
            
            Configuration.Configuration configuration = new Configuration.Configuration
            { Timeout = timeout };
            configurationManager.GetConfiguration().Returns(configuration);

            var sut = new Repository(ibClinet, null, ibClientQueue, configurationManager, null, messageProcessor);
            var resultList = sut.AllPositions();

            Assert.Equal(3, resultList.Count);
        }

        [Fact]
        public void ReturnNullIfNoTrackedSymbolAfterTimeout()
        {
            var timeout = 1000;

            var ibClientQueue = Substitute.For<IIbClientQueue>();
            var ibClinet = Substitute.For<IIbClientWrapper>();
            ibClinet.When(c => c.ContractDetails(Arg.Any<int>(), Arg.Any<Contract>()))
                .Do(c => { });

            var configurationManager = Substitute.For<IConfigurationManager>();
            var contract = new Contract();

            Configuration.Configuration configuration = new Configuration.Configuration
            { Timeout = timeout };
            configurationManager.GetConfiguration().Returns(configuration);

            var trackedSymbolFactory = new TrackedSymbolFactory();
            var portfolio = new Portfolio();
            var messageProcessor = new MessageProcessor(trackedSymbolFactory, portfolio);

            var sut = new Repository(ibClinet, null, ibClientQueue, configurationManager, null, messageProcessor);
            var trackedSymbol = sut.GetTrackedSymbol(contract);
            Thread.Sleep(timeout);

            Assert.Null(trackedSymbol);
        }

        //[Fact]
        //public void ReturnTrackedSymbol()
        //{
        //    var timeout = 1000;

        //    var trackedSymbolString = @"{""Symbol"":""MNQ"",""ReqIdContractDetails"":1,""ConId"":515971877,""SecType"":""FOP"",""Currency"":""USD"",""Exchange"":""GLOBEX"",""Strike"":16300,""NextStrike"":0,""OverNextStrike"":0}";
        //    var ibClientQueue = Substitute.For<IIbClientQueue>();
        //    ibClientQueue.Dequeue().Returns(trackedSymbolString);

        //    var ibClinet = Substitute.For<IIbClientWrapper>();
        //    ibClinet.When(c => c.ContractDetails(Arg.Any<int>(), Arg.Any<Contract>()))
        //        .Do(c => { });

        //    var configurationManager = Substitute.For<IConfigurationManager>();
        //    var contract = new Contract();

        //    Configuration.Configuration configuration = new Configuration.Configuration
        //    { Timeout = timeout };
        //    configurationManager.GetConfiguration().Returns(configuration);

        //    var trackedSymbolFactory = new TrackedSymbolFactory();
        //    var portfolio = new Portfolio();
        //    var messageProcessor = new MessageProcessor(trackedSymbolFactory, portfolio);

        //    var sut = new Repository(ibClinet, null, ibClientQueue, configurationManager, null, messageProcessor);
        //    var trackedSymbol = sut.GetTrackedSymbol(contract);
        //    Thread.Sleep(timeout);

        //    Assert.NotNull(trackedSymbol);
        //}

        //[Theory, AutoNSubstituteData]
        //public void CallQueryParametersConverter(
        //    [Frozen] IInputQueue inputQueue,
        //    [Frozen] IQueryParametersConverter queryParametersConverter,
        //    Repository sut)
        //{
        //    var trackedSymbolString = @"{""Symbol"":""MNQ"",""ReqIdContractDetails"":1,""ConId"":515971877,""SecType"":""FOP"",""Currency"":""USD"",""Exchange"":""GLOBEX"",""Strike"":16300,""NextStrike"":0,""OverNextStrike"":0}";
        //    inputQueue.Dequeue().Returns(trackedSymbolString);
        //    var contract = new Contract();

        //    var trackedSymbol = sut.GetTrackedSymbol(contract);

        //    queryParametersConverter.Received().TrackedSymbolForReqSecDefOptParams(Arg.Any<ITrackedSymbol>());
        //}
    }
}
