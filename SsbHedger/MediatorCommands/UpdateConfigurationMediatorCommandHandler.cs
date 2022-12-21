using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SsbHedger.MediatorCommands
{
    public class UpdateConfigurationMediatorCommandHandler : INotificationHandler<UpdateConfigurationMediatorCommand>
    {
        private IRegistryManager _registryManager;

        public UpdateConfigurationMediatorCommandHandler(IRegistryManager registryManager)
        {
            _registryManager = registryManager;
        }

        public async Task Handle(UpdateConfigurationMediatorCommand notification, CancellationToken cancellationToken)
        {
            await Task.Run(() => _registryManager.WriteConfiguration(
                notification.Host,
                notification.Port,
                notification.ClientId));
            notification.CloseAction();
        }
    }
}
