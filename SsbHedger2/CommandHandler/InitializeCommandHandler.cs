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
            var (host, port, clientId) = _registryManager.ReadConfiguration(
                _ibHost.DefaultHost,
                _ibHost.DefaultPort,
                _ibHost.DefaultClientId);
            _ibHost.ConnectAndStartReaderThread(host, port, clientId);
        }
    }
}
