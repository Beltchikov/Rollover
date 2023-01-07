using SsbHedger.Configuration;
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
            var configurationdata = _registryManager.ReadConfiguration(new ConfigurationData(
                (string)_configuration.GetValue(Configuration.Configuration.HOST),
                (int)_configuration.GetValue(Configuration.Configuration.PORT),
                (int)_configuration.GetValue(Configuration.Configuration.CLIENT_ID),
                (string)_configuration.GetValue(Configuration.Configuration.UNDERLYING_SYMBOL),
                (string)_configuration.GetValue(Configuration.Configuration.SESSION_START),
                (string)_configuration.GetValue(Configuration.Configuration.SESSION_END)));

            _configuration.SetValue(Configuration.Configuration.HOST, configurationdata.Host);
            _configuration.SetValue(Configuration.Configuration.PORT, configurationdata.Port);
            _configuration.SetValue(Configuration.Configuration.CLIENT_ID, configurationdata.ClientId);
            _configuration.SetValue(Configuration.Configuration.UNDERLYING_SYMBOL, configurationdata.UnderlyingSymbol);
            _configuration.SetValue(Configuration.Configuration.SESSION_START, configurationdata.SessionStart);
            _configuration.SetValue(Configuration.Configuration.SESSION_END, configurationdata.SessionEnd);

            _ibHost.ViewModel = viewModel;
            _ibHost.ConnectAndStartReaderThread(
                configurationdata.Host,
                configurationdata.Port,
                configurationdata.ClientId);
        }
    }
}
