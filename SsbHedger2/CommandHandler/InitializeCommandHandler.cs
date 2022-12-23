using SsbHedger2.IbHost;
using SsbHedger2.RegistryManager;
using System;

namespace SsbHedger2.CommandHandler
{
    internal sealed class InitializeCommandHandler
    {
        private static InitializeCommandHandler _instance = null!;
        private static readonly object threadLock = new object();
        private IRegistryManager _registryManager = null!;
        private IIbHost _ibHost = null!;

        private InitializeCommandHandler() { }

        internal static InitializeCommandHandler Create(IRegistryManager registryManager, IIbHost ibHost)
        {

            lock (threadLock)
            {
                if (_instance == null)
                {
                    _instance = new InitializeCommandHandler();
                    _instance._registryManager = registryManager;
                    _instance._ibHost = ibHost;
                }

                return _instance;
            }
        }

        internal void Handle()
        {
            throw new NotImplementedException();

            // Read configuration from Registry

            // Instatiate ibHost

            // IbHost is responsible for the updating of viewModel

        }
    }
}
