using MediatR;

namespace SsbHedger.MediatorCommands
{
    public class UpdateConfigurationMediatorCommand : IRequest
    {
        private string? data;

        public UpdateConfigurationMediatorCommand(string? data)
        {
            this.data = data;
        }
    }
}