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
            Action closeAction,
            [Frozen] IRegistryManager registryManager,
            UpdateConfigurationMediatorCommandHandler sut)
        {
            var command = new UpdateConfigurationMediatorCommand(
                host,
                port,
                clientId,
                closeAction);
            await sut.Handle(command, new CancellationToken());
            registryManager.Received().WriteConfiguration(host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public async Task CloseWindow(
            string host,
            int port,
            int clientId,
            Action closeAction,
            UpdateConfigurationMediatorCommandHandler sut)
        {
            UpdateConfigurationMediatorCommand command 
                = Substitute.For<UpdateConfigurationMediatorCommand>(host, port, clientId, closeAction);
            await sut.Handle(command, new CancellationToken());
            command.Received().CloseAction();
        }
    }
}
