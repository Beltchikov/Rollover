using System;

namespace SsbHedger2.CommandHandler
{
    public sealed class InitializeCommandHandler
    {
        private IRegistryManager _registryManager = null!;
        private IIbHost _ibHost = null!;

        public InitializeCommandHandler(IRegistryManager registryManager, IIbHost ibHost)
        {
            _registryManager = registryManager;
            _ibHost = ibHost;
        }

        public void Handle()
        {
            //throw new NotImplementedException();

            // Read configuration from Registry
            var (host, port, clientId) = _registryManager.ReadConfiguration(
                _ibHost.DefaultHost,
                _ibHost.DefaultPort,
                _ibHost.DefaultClientId);

            // Call IbHost.ConnectAndStartReaderThread(string host, int port, int clientId)

            // IbHost is responsible for the updating of viewModel

        }
    }
}
