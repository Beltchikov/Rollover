using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Model;
using SsbHedger.RegistryManager;

namespace SsbHedger.UnitTests.CommandHandler
{
    public class UpdateConfigurationCommandHandlerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallWriteConfiguration(
            string host,
            int port,
            int clientId,
            [Frozen] IRegistryManager registryManager,
            MainWindowViewModel viewModel,
            UpdateConfigurationCommandHandler sut)
        {
            sut.Handle(viewModel, new object[] { host, port, clientId });
            registryManager.Received().WriteConfiguration(
                host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void UpdateConfiguration(
            string host,
            int port,
            int clientId,
            MainWindowViewModel viewModel,
            [Frozen] IConfiguration configuration,
            UpdateConfigurationCommandHandler sut)
        {
            sut.Handle(viewModel, new object[] { host, port, clientId });
            
            configuration.Received().SetValue("Host", host);
            configuration.Received().SetValue("Port", port);
            configuration.Received().SetValue("ClientId", clientId);
        }

        [Theory, AutoNSubstituteData]
        public void Reconnect(
           string host,
           int port,
           int clientId,
           MainWindowViewModel viewModel,
           [Frozen] IIbHost ibHost,
           UpdateConfigurationCommandHandler sut)
        {
            sut.Handle(viewModel, new object[] { host, port, clientId });

            ibHost.Received().Disconnect();
            ibHost.Received().ConnectAndStartReaderThread(host, port, clientId);
        }
    }
}