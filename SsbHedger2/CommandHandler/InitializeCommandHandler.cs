using SsbHedger.Model;
using SsbHedger.RegistryManager;

namespace SsbHedger.CommandHandler
{
    public sealed class InitializeCommandHandler : IInitializeCommandHandler
    {
        private IRegistryManager _registryManager = null!;
        private IConfiguration _configuration = null!;
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

        public void Handle(MainWindowViewModel viewModel)
        {
            const string HOST = "Host";
            const string PORT = "Port";
            const string CLIENT_ID = "ClientId";

            var (host, port, clientId) = _registryManager.ReadConfiguration(
                (string)_configuration.GetValue(HOST),
                (int)_configuration.GetValue(PORT),
                (int)_configuration.GetValue(CLIENT_ID));

            _configuration.SetValue(HOST, host);
            _configuration.SetValue(PORT, port);
            _configuration.SetValue(CLIENT_ID, clientId);

            _ibHost.ViewModel= viewModel;   
            _ibHost.ConnectAndStartReaderThread(host, port, clientId);
        }
    }
}
