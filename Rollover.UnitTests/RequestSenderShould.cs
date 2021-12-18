using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class RequestSenderShould
    {
        [Theory, AutoNSubstituteData]
        public void CallIbClientConnectInConnect(
            [Frozen] IIbClientWrapper ibClient,
            RequestSender sut)
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
            RequestSender sut)
        {
            sut.Connect("", 0, 0);
            ibClient.Received().ReaderFactory();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientIsConnectedInConnect(
            [Frozen] IIbClientWrapper ibClient,
            RequestSender sut)
        {
            sut.Connect("", 0, 0);
            ibClient.Received().IsConnected();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientWaitForSignalConnect(
            [Frozen] IIbClientWrapper ibClient,
            RequestSender sut)
        {
            ibClient.IsConnected().Returns(true);
            sut.Connect("", 0, 0);
            ibClient.Received().WaitForSignal();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientDisconnect(
            [Frozen] IIbClientWrapper ibClient,
            RequestSender sut)
        {
            sut.Disconnect();   
            ibClient.Received().Disconnect();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientRegisterResponseHandlers(
            [Frozen] IIbClientWrapper ibClient,
            [Frozen] IInputQueue inputQueue,
            [Frozen] SynchronizationContext synchronizationContext,
            RequestSender sut)
        {
            sut.RegisterResponseHandlers(inputQueue, synchronizationContext);
            ibClient.Received().RegisterResponseHandlers(inputQueue, synchronizationContext);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientListPositions(
            [Frozen] IIbClientWrapper ibClient,
            RequestSender sut)
        {
            sut.ListPositions();
            ibClient.Received().ListPositions();
        }
    }
}
