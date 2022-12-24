using System;

namespace SsbHedger2.CommandHandler
{
    internal sealed class InitializeCommandHandler
    {
        private IRegistryManager _registryManager = null!;
        private IIbHost _ibHost = null!;

        public InitializeCommandHandler(IRegistryManager registryManager, IIbHost ibHost)
        {
            _registryManager = registryManager;
            _ibHost = ibHost;
        }

        internal void Handle()
        {
            throw new NotImplementedException();

            // Read configuration from Registry

            // Call IbHost.ConnectAndStartReaderThread(string host, int port, int clientId)

            // IbHost is responsible for the updating of viewModel

        }
    }
}
