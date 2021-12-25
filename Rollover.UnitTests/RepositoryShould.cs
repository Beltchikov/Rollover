﻿using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
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
            var ibClinet = Substitute.For<IbClientWrapper>();
            var inputQueue = Substitute.For<IInputQueue>();
            
            var sut = new Repository(ibClinet, null, null, inputQueue, null);
            // TODO
        }

        [Fact]
        public void ReturnTrackedSymbol()
        {

        }
    }
}
