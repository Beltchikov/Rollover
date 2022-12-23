using SsbHedger2.Model;
using SsbHedger2.RegistryManager;
using System;

namespace SsbHedger2.CommandHandler
{
    internal sealed class InitializeCommandHandler
    {
        private static InitializeCommandHandler _instance = null!;
        private static readonly object threadLock = new object();
        private IRegistryManager _registryManager = null!;

        private InitializeCommandHandler() { }

        internal static InitializeCommandHandler Create()
        {

            lock (threadLock)
            {
                if (_instance == null)
                {
                    _instance = new InitializeCommandHandler();
                }

                return _instance;
            }
        }

        internal static void Handle(MainWindowViewModel mainWindowViewModel)
        {
            throw new NotImplementedException();

            // Read configuration from Registry

            // Instatiate ibHost

            // IbHost is responsible for the updating of viewModel

        }
    }
}
