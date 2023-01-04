using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Configuration;
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
        public void SyncConfigurationWithRegistry(
           string host,
           string hostFromRegistry,
           int port,
           int portFromRegistry,
           int clientId,
           int clientIdFromRegistry,
           MainWindowViewModel viewModel,
           [Frozen] IRegistryManager registryManager,
           [Frozen] IConfiguration configuration,
           InitializeCommandHandler sut)
        {
            const string HOST = "Host";
            const string PORT = "Port";
            const string CLIENT_ID = "ClientId";
            
            configuration.GetValue(HOST).Returns(host);
            configuration.GetValue(PORT).Returns(port);
            configuration.GetValue(CLIENT_ID).Returns(clientId);

            registryManager.ReadConfiguration(host, port, clientId)
                .Returns(ValueTuple.Create(hostFromRegistry, portFromRegistry, clientIdFromRegistry));

            sut.Handle(viewModel);
            
            configuration.Received().SetValue(HOST, hostFromRegistry);
            configuration.Received().SetValue(PORT, portFromRegistry);
            configuration.Received().SetValue(CLIENT_ID, clientIdFromRegistry);
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