using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;

namespace SsbHedger.UnitTests.CommandHandler
{
    public class UpdateConfigurationCommandHandlerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallWriteConfiguration(
            ConfigurationData configurationData,
            [Frozen] IRegistryManager registryManager,
            MainWindowViewModel viewModel,
            UpdateConfigurationCommandHandler sut)
        {
            sut.Handle(viewModel, new object[] 
            { 
                configurationData.Host, 
                configurationData.Port,
                configurationData.ClientId,
                configurationData.UnderlyingSymbol,
                configurationData.SessionStart,
                configurationData.SessionEnd
            });
            registryManager.Received().WriteConfiguration(configurationData);
        }

        [Theory, AutoNSubstituteData]
        public void UpdateConfiguration(
            ConfigurationData configurationData,
            MainWindowViewModel viewModel,
            [Frozen] IConfiguration configuration,
            UpdateConfigurationCommandHandler sut)
        {
            sut.Handle(viewModel, new object[]
            {
                configurationData.Host,
                configurationData.Port,
                configurationData.ClientId,
                configurationData.UnderlyingSymbol,
                configurationData.SessionStart,
                configurationData.SessionEnd
            });

            configuration.Received().SetValue(
                SsbConfiguration.Configuration.HOST, configurationData.Host);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.PORT, configurationData.Port);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.CLIENT_ID, configurationData.ClientId);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.UNDERLYING_SYMBOL, configurationData.UnderlyingSymbol);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.SESSION_START, configurationData.SessionStart);
            configuration.Received().SetValue(
                SsbConfiguration.Configuration.SESSION_END, configurationData.SessionEnd);
        }

        [Theory, AutoNSubstituteData]
        public void Reconnect(
           ConfigurationData configurationData,
           MainWindowViewModel viewModel,
           [Frozen] IIbHost ibHost,
           UpdateConfigurationCommandHandler sut)
        {
            sut.Handle(viewModel, new object[]
            {
                configurationData.Host,
                configurationData.Port,
                configurationData.ClientId,
                configurationData.UnderlyingSymbol,
                configurationData.SessionStart,
                configurationData.SessionEnd
            });

            ibHost.Received().Disconnect();
            ibHost.Received().ConnectAndStartReaderThread();
        }
    }
}