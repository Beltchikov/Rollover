using AutoFixture.Xunit2;
using IBApi;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
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
        public void ReturnNullIfNoTrackedSymbolAfterTimeout()
        {
            var timeout = 1000;

            var inputQueue = Substitute.For<IInputQueue>();
            var ibClinet = Substitute.For<IIbClientWrapper>();
            ibClinet.When(c => c.ContractDetails(Arg.Any<int>(), Arg.Any<Contract>()))
                .Do(c => { });

            var configurationManager = Substitute.For<IConfigurationManager>();
            var contract = new Contract();

            Configuration.Configuration configuration = new Configuration.Configuration 
            { Timeout = timeout};
            configurationManager.GetConfiguration().Returns(configuration);

            var sut = new Repository(ibClinet, null, inputQueue, configurationManager);
            var trackedSymbol = sut.GetTrackedSymbol(contract);
            Thread.Sleep(timeout);
            
            Assert.Null(trackedSymbol);
        }

        [Fact]
        public void ReturnTrackedSymbol()
        {
            var timeout = 1000;

            var trackedSymbolString = @"{""Symbol"":""MNQ"",""ReqIdContractDetails"":1,""ConId"":515971877,""SecType"":""FOP"",""Currency"":""USD"",""Exchange"":""GLOBEX"",""Strike"":16300,""NextStrike"":0,""OverNextStrike"":0}";
            var inputQueue = Substitute.For<IInputQueue>();
            inputQueue.Dequeue().Returns(trackedSymbolString);

            var ibClinet = Substitute.For<IIbClientWrapper>();
            ibClinet.When(c => c.ContractDetails(Arg.Any<int>(), Arg.Any<Contract>()))
                .Do(c => { });

            var configurationManager = Substitute.For<IConfigurationManager>();
            var contract = new Contract();

            Configuration.Configuration configuration = new Configuration.Configuration
            { Timeout = timeout };
            configurationManager.GetConfiguration().Returns(configuration);

            var sut = new Repository(ibClinet, null, inputQueue, configurationManager);
            var trackedSymbol = sut.GetTrackedSymbol(contract);
            Thread.Sleep(timeout);

            Assert.NotNull(trackedSymbol);
        }
    }
}
