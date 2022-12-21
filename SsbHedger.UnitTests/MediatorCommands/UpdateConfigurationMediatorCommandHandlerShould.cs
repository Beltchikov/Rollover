using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.MediatorCommands;

namespace SsbHedger.UnitTests.MediatorCommands
{
    public class UpdateConfigurationMediatorCommandHandlerShould
    {
        [Theory, AutoNSubstituteData]
        public async Task RegistryManagerWriteConfigurationAsync(
            string host,
            int port,
            int clientId,
            [Frozen] IRegistryManager registryManager,
            UpdateConfigurationMediatorCommandHandler sut)
        {
            var command = new UpdateConfigurationMediatorCommand(host, port, clientId);
            await sut.Handle(command, new CancellationToken());
            registryManager.Received().WriteConfiguration(host, port, clientId);
        }
    }
}
