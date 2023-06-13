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
        public async Task CallConnectAndStartReaderThreadAsync(
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
                .ReturnsForAnyArgs(new ConfigurationData( host,  port,  clientId, "", "", "", 1, 10, "1"));

            await sut.HandleAsync(viewModel);
            
            await ibHost.Received().ConnectAndStartReaderThread();
        }

        [Theory, AutoNSubstituteData]
        public async Task NotCallReqHistoricalDataIfNotConnected(
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
                .ReturnsForAnyArgs(new ConfigurationData(host, port, clientId, "", "", "", 1, 10, "1"));
            ibHost.ConnectAndStartReaderThread().Returns(false);

            await sut.HandleAsync(viewModel);

            ibHost.DidNotReceive().ReqHistoricalData();
        }

        [Theory, AutoNSubstituteData]
        public async Task CallReqHistoricalDataIfConnected(
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
                .ReturnsForAnyArgs(new ConfigurationData(host, port, clientId, "", "", "", 1, 10, "1"));
            ibHost.ConnectAndStartReaderThread().Returns(true);

            await sut.HandleAsync(viewModel);

            ibHost.Received().ReqHistoricalData();
        }

        [Theory, AutoNSubstituteData]
        public async Task CallApplyDefaultHistoricalDataIfNotConnected(
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
                .ReturnsForAnyArgs(new ConfigurationData(host, port, clientId, "", "", "", 1, 10, "1"));
            ibHost.ConnectAndStartReaderThread().Returns(false);

            await sut.HandleAsync(viewModel);

            ibHost.Received().ApplyDefaultHistoricalData();
        }
    }
}