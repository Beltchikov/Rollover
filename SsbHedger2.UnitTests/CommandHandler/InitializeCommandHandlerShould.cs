using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger2.CommandHandler;

namespace SsbHedger2.UnitTests.CommandHandler
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
    }
}