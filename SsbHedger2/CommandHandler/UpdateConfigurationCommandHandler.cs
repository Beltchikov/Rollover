using SsbHedger.Model;
using SsbHedger.RegistryManager;
using System;

namespace SsbHedger.CommandHandler
{
    public class UpdateConfigurationCommandHandler : IUpdateConfigurationCommandHandler
    {
        private IRegistryManager _registryManager = null!;
        IConfiguration _configuration = null!;

        public UpdateConfigurationCommandHandler(
            IRegistryManager registryManager,
            IConfiguration configuration)
        {
            _registryManager = registryManager;
            _configuration = configuration;
        }

        public void Handle(object[] parameters)
        {
            if (parameters == null)
            {
                throw new ApplicationException("Unexpected! data is null");
            }
            string? host = parameters[0]?.ToString();
            if (host == null)
            {
                throw new ApplicationException("Unexpected! host is null");
            }
            int port = Convert.ToInt32(parameters[1]);
            int clientId = Convert.ToInt32(parameters[2]);

            _registryManager.WriteConfiguration(host, port, clientId);
            _configuration.SetValue("Host", host);
            _configuration.SetValue("Port", port);
            _configuration.SetValue("ClientId", clientId);
        }
    }
}