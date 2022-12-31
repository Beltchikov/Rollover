using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Configuration;

namespace SsbHedger.UnitTests.CommandHandler
{
    public class InitializeCommandHandlerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallRegistryManager(
            string host,
            int port,
            int clientId,
            [Frozen] IRegistryManager registryManager,
            [Frozen] IIbHost ibHost,
            InitializeCommandHandler sut)
        {
            ibHost.DefaultHost.Returns(host);
            ibHost.DefaultPort.Returns(port);
            ibHost.DefaultClientId.Returns(clientId);

            sut.Handle();
            registryManager.Received().ReadConfiguration(
                host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void CallConnectAndStartReaderThread(
            string host,
            int port,
            int clientId,
            [Frozen] IRegistryManager registryManager,
            [Frozen] IIbHost ibHost,
            InitializeCommandHandler sut)
        {
            registryManager.ReadConfiguration(Arg.Any<string>(), default, default)
                .ReturnsForAnyArgs(ValueTuple.Create( host,  port,  clientId));
            sut.Handle();
            ibHost.Received().ConnectAndStartReaderThread(host, port, clientId);
        }
    }
}