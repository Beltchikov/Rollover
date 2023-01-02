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
            
            configuration.Received().SetValue("Host", host);
            configuration.Received().SetValue("Port", port);
            configuration.Received().SetValue("ClientId", clientId);
        }
    }
}