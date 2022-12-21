using MediatR;
using SsbHedger.Model;
using System;

namespace SsbHedger.MediatorCommands
{
    public class UpdateConfigurationMediatorCommand : INotification
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int ClientId { get; set; }
        public Action CloseAction { get; set; }
        
        public UpdateConfigurationMediatorCommand(
            string host,
            int port,
            int clientId,
            Action closeAction)
        {
            Host= host;
            Port= port;
            ClientId = clientId;
            CloseAction = closeAction;
        }
    }
}