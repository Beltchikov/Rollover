using AutoFixture.Xunit2;
using MediatR;
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
            throw new NotImplementedException();

            //string data = $"{host}; {port}; {clientId}";
            //UpdateConfigurationMediatorCommand command = new UpdateConfigurationMediatorCommand(data);


            ////Act
            //Unit x = await sut.Handle(command, new System.Threading.CancellationToken());

            ////Assert
            //registryManager.Received().WriteConfiguration(host, port, clientId);
        }
    }
}
