using SsbHedger.Model;
using SsbHedger.RegistryManager;
using System;

namespace SsbHedger.CommandHandler
{
    public class UpdateConfigurationCommandHandler : IUpdateConfigurationCommandHandler
    {
        private IRegistryManager _registryManager = null!;
        private IConfiguration _configuration = null!;
        private IIbHost _ibHost = null!;

        public UpdateConfigurationCommandHandler(
            IRegistryManager registryManager,
            IConfiguration configuration,
            IIbHost ibHost)
        {
            _registryManager = registryManager;
            _configuration = configuration;
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel viewModel, object[] parameters)
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

            _ibHost.Disconnect();
            _ibHost.ConnectAndStartReaderThread(host, port, clientId);
        }
    }
}