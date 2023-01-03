﻿using SsbHedger.Model;
using SsbHedger.RegistryManager;

namespace SsbHedger.CommandHandler
{
    public sealed class InitializeCommandHandler
    {
        private IRegistryManager _registryManager = null!;
        private IConfiguration _configuration= null!;
        private IIbHost _ibHost = null!;

        public InitializeCommandHandler(
            IRegistryManager registryManager,
            IConfiguration configuration,
            IIbHost ibHost)
        {
            _registryManager = registryManager;
            _ibHost = ibHost;
            _configuration = configuration;
        }

        public void Handle()
        {
            var (host, port, clientId) = _registryManager.ReadConfiguration(
                (string)_configuration.GetValue("Host"),
                (int)_configuration.GetValue("Port"),
                (int)_configuration.GetValue("ClientId"));
            _ibHost.ConnectAndStartReaderThread(host, port, clientId);
        }
    }
}
