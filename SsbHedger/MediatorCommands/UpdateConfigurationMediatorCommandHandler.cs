using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SsbHedger.MediatorCommands
{
    public class UpdateConfigurationMediatorCommandHandler : IRequestHandler<UpdateConfigurationMediatorCommand>
    {
        private IRegistryManager _registryManager;

        public UpdateConfigurationMediatorCommandHandler(IRegistryManager registryManager)
        {
            _registryManager = registryManager;
        }

        public Task<Unit> Handle(UpdateConfigurationMediatorCommand command, CancellationToken cancellationToken)
        {
            //_registryManager.WriteConfiguration()

            throw new NotImplementedException();
        }
    }
}
