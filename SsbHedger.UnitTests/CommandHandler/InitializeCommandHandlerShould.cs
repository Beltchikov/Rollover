using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;

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
            configuration.GetValue(SsbConfiguration.Configuration.HOST)
                .Returns(configurationData.Host);
            configuration.GetValue(SsbConfiguration.Configuration.PORT)
                .Returns(configurationData.Port);
            configuration.GetValue(SsbConfiguration.Configuration.CLIENT_ID)
                .Returns(configurationData.ClientId);
            configuration.GetValue(SsbConfiguration.Configuration.UNDERLYING_SYMBOL)
                .Returns(configurationData.UnderlyingSymbol);
            configuration.GetValue(SsbConfiguration.Configuration.SESSION_START)
                .Returns(configurationData.SessionStart);
            configuration.GetValue(SsbConfiguration.Configuration.SESSION_END)
                .Returns(configurationData.SessionEnd);

            sut.HandleAsync(viewModel);
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
            configuration.GetValue(SsbConfiguration.Configuration.HOST)
                .Returns(configurationData.Host);
            configuration.GetValue(SsbConfiguration.Configuration.PORT)
                .Returns(configurationData.Port);
            configuration.GetValue(SsbConfiguration.Configuration.CLIENT_ID)
                .Returns(configurationData.ClientId);
            configuration.GetValue(SsbConfiguration.Configuration.UNDERLYING_SYMBOL)
                .Returns(configurationData.UnderlyingSymbol);
            configuration.GetValue(SsbConfiguration.Configuration.SESSION_START)
                .Returns(configurationData.SessionStart);
            configuration.GetValue(SsbConfiguration.Configuration.SESSION_END)
                .Returns(configurationData.SessionEnd);

            registryManager.ReadConfiguration(configurationData)
                .Returns(configurationDataFromRegistry);

            sut.HandleAsync(viewModel);
            
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.HOST, configurationDataFromRegistry.Host);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.PORT, configurationDataFromRegistry.Port);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.CLIENT_ID, configurationDataFromRegistry.ClientId);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.UNDERLYING_SYMBOL, configurationDataFromRegistry.UnderlyingSymbol);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.SESSION_START, configurationDataFromRegistry.SessionStart);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.SESSION_END, configurationDataFromRegistry.SessionEnd);
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
            configuration.GetValue(Configuration.HOST).Returns(host);
            configuration.GetValue(Configuration.PORT).Returns(port);
            configuration.GetValue(Configuration.CLIENT_ID).Returns(clientId);
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("");
            configuration.GetValue(Configuration.SESSION_START).Returns("");
            configuration.GetValue(Configuration.SESSION_END).Returns("");

            registryManager.ReadConfiguration(Arg.Any<ConfigurationData>())
                .ReturnsForAnyArgs(new ConfigurationData( host,  port,  clientId, "", "", ""));
            
            sut.HandleAsync(viewModel);
            
            ibHost.Received().ConnectAndStartReaderThread();
        }
    }
}