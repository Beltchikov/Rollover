using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Model;
using SsbHedger.RegistryManager;

namespace SsbHedger.UnitTests.CommandHandler
{
    public class InitializeCommandHandlerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallRegistryManager(
            string host,
            int port,
            int clientId,
            MainWindowViewModel viewModel,
            [Frozen] IRegistryManager registryManager,
            [Frozen] IConfiguration configuration,
            InitializeCommandHandler sut)
        {
            configuration.GetValue("Host").Returns(host);
            configuration.GetValue("Port").Returns(port);
            configuration.GetValue("ClientId").Returns(clientId);
            
            sut.Handle(viewModel);
            registryManager.Received().ReadConfiguration(
                host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void CallConnectAndStartReaderThread(
            string host,
            int port,
            int clientId,
            MainWindowViewModel viewModel,
            [Frozen] IRegistryManager registryManager,
            [Frozen] IConfiguration configuration,
            [Frozen] IIbHost ibHost,
            InitializeCommandHandler sut)
        {
            configuration.GetValue("Host").Returns(host);
            configuration.GetValue("Port").Returns(port);
            configuration.GetValue("ClientId").Returns(clientId);
            registryManager.ReadConfiguration(Arg.Any<string>(), default, default)
                .ReturnsForAnyArgs(ValueTuple.Create( host,  port,  clientId));
            
            sut.Handle(viewModel);
            
            ibHost.Received().ConnectAndStartReaderThread(host, port, clientId);
        }
    }
}