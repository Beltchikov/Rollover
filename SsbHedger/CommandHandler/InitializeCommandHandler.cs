using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using System.Threading.Tasks;

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

        public async Task HandleAsync(MainWindowViewModel viewModel)
        {
            var configurationdata = _registryManager.ReadConfiguration(new ConfigurationData(
                (string)_configuration.GetValue(Configuration.HOST),
                (int)_configuration.GetValue(Configuration.PORT),
                (int)_configuration.GetValue(Configuration.CLIENT_ID),
                (string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL),
                (string)_configuration.GetValue(Configuration.SESSION_START),
                (string)_configuration.GetValue(Configuration.SESSION_END)));

            _configuration.SetValue(Configuration.HOST, configurationdata.Host);
            _configuration.SetValue(Configuration.PORT, configurationdata.Port);
            _configuration.SetValue(Configuration.CLIENT_ID, configurationdata.ClientId);
            _configuration.SetValue(Configuration.UNDERLYING_SYMBOL, configurationdata.UnderlyingSymbol);
            _configuration.SetValue(Configuration.SESSION_START, configurationdata.SessionStart);
            _configuration.SetValue(Configuration.SESSION_END, configurationdata.SessionEnd);

            _ibHost.ViewModel = viewModel;
            
            var connected = await _ibHost.ConnectAndStartReaderThread();
            if(connected)
            {
                _ibHost.ReqHistoricalData();
            }
            else
            {
                _ibHost.ApplyDefaultHistoricalData();
            }
        }
    }
}
