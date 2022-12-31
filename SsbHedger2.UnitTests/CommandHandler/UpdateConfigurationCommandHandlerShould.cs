using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Configuration;

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
            UpdateConfigurationCommandHandler sut)
        {
            sut.Handle(new object[] { host, port, clientId });
            registryManager.Received().WriteConfiguration(
                host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void UpdateConfigurationInViewModel(
            string host,
            int port,
            int clientId,
            [Frozen] IConfiguration configuration,
            UpdateConfigurationCommandHandler sut)
        {
            sut.Handle(new object[] { host, port, clientId });
            configuration.Received().Host = host;
            configuration.Received().Port = port;
            configuration.Received().ClientId = clientId;
        }
    }
}