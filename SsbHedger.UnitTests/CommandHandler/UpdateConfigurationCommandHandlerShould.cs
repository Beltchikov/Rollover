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
                configurationData.SessionEnd,
                configurationData.Dte,
                configurationData.NumberOfStrikes,
                configurationData.StrikeStep
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
                configurationData.SessionEnd,
                configurationData.Dte,
                configurationData.NumberOfStrikes,
                configurationData.StrikeStep
            });

            configuration.Received().SetValue(
                Configuration.HOST, configurationData.Host);
            configuration.Received().SetValue(
                Configuration.PORT, configurationData.Port);
            configuration.Received().SetValue(
                Configuration.CLIENT_ID, configurationData.ClientId);
            configuration.Received().SetValue(
                Configuration.UNDERLYING_SYMBOL, configurationData.UnderlyingSymbol);
            configuration.Received().SetValue(
                Configuration.SESSION_START, configurationData.SessionStart);
            configuration.Received().SetValue(
                Configuration.SESSION_END, configurationData.SessionEnd);
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
                configurationData.SessionEnd,
                configurationData.Dte,
                configurationData.NumberOfStrikes,
                configurationData.StrikeStep
            });

            ibHost.Received().Disconnect();
            ibHost.Received().ConnectAndStartReaderThread();
        }
    }
}