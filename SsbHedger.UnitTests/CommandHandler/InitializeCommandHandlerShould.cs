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
            ConfigurationData configurationData,
            MainWindowViewModel viewModel,
            [Frozen] IConfiguration configuration,
            [Frozen] IRegistryManager registryManager,
            InitializeCommandHandler sut)
        {
            configuration.GetValue(Configuration.Configuration.HOST)
                .Returns(configurationData.Host);
            configuration.GetValue(Configuration.Configuration.PORT)
                .Returns(configurationData.Port);
            configuration.GetValue(Configuration.Configuration.CLIENT_ID)
                .Returns(configurationData.ClientId);
            configuration.GetValue(Configuration.Configuration.UNDERLYING_SYMBOL)
                .Returns(configurationData.UnderlyingSymbol);
            configuration.GetValue(Configuration.Configuration.SESSION_START)
                .Returns(configurationData.SessionStart);
            configuration.GetValue(Configuration.Configuration.SESSION_END)
                .Returns(configurationData.SessionEnd);

            sut.Handle(viewModel);
            registryManager.Received().ReadConfiguration(configurationData);
        }

        [Theory, AutoNSubstituteData]
        public void SyncConfigurationWithRegistry(
           ConfigurationData configurationData,
           ConfigurationData configurationDataFromRegistry,
           MainWindowViewModel viewModel,
           [Frozen] IRegistryManager registryManager,
           [Frozen] IConfiguration configuration,
           InitializeCommandHandler sut)
        {
            configuration.GetValue(Configuration.Configuration.HOST)
                .Returns(configurationData.Host);
            configuration.GetValue(Configuration.Configuration.PORT)
                .Returns(configurationData.Port);
            configuration.GetValue(Configuration.Configuration.CLIENT_ID)
                .Returns(configurationData.ClientId);
            configuration.GetValue(Configuration.Configuration.UNDERLYING_SYMBOL)
                .Returns(configurationData.UnderlyingSymbol);
            configuration.GetValue(Configuration.Configuration.SESSION_START)
                .Returns(configurationData.SessionStart);
            configuration.GetValue(Configuration.Configuration.SESSION_END)
                .Returns(configurationData.SessionEnd);

            registryManager.ReadConfiguration(configurationData)
                .Returns(configurationDataFromRegistry);

            sut.Handle(viewModel);
            
            configuration.Received().SetValue(
                Configuration.Configuration.HOST, configurationDataFromRegistry.Host);
            configuration.Received().SetValue(
                Configuration.Configuration.PORT, configurationDataFromRegistry.Port);
            configuration.Received().SetValue(
                Configuration.Configuration.CLIENT_ID, configurationDataFromRegistry.ClientId);
            configuration.Received().SetValue(
                Configuration.Configuration.UNDERLYING_SYMBOL, configurationDataFromRegistry.UnderlyingSymbol);
            configuration.Received().SetValue(
                Configuration.Configuration.SESSION_START, configurationDataFromRegistry.SessionStart);
            configuration.Received().SetValue(
                Configuration.Configuration.SESSION_END, configurationDataFromRegistry.SessionEnd);
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
            configuration.GetValue(Configuration.Configuration.HOST).Returns(host);
            configuration.GetValue(Configuration.Configuration.PORT).Returns(port);
            configuration.GetValue(Configuration.Configuration.CLIENT_ID).Returns(clientId);
            configuration.GetValue(Configuration.Configuration.UNDERLYING_SYMBOL).Returns("");
            configuration.GetValue(Configuration.Configuration.SESSION_START).Returns("");
            configuration.GetValue(Configuration.Configuration.SESSION_END).Returns("");

            registryManager.ReadConfiguration(Arg.Any<ConfigurationData>())
                .ReturnsForAnyArgs(new ConfigurationData( host,  port,  clientId, "", "", ""));
            
            sut.Handle(viewModel);
            
            ibHost.Received().ConnectAndStartReaderThread(host, port, clientId);
        }
    }
}