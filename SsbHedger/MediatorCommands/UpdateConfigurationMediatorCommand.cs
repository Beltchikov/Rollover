using MediatR;

namespace SsbHedger.MediatorCommands
{
    public class UpdateConfigurationMediatorCommand : INotification
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int ClientId { get; set; }

        public UpdateConfigurationMediatorCommand(string host, int port, int clientId)
        {
            Host= host;
            Port= port;
            ClientId = clientId;
        }
    }
}