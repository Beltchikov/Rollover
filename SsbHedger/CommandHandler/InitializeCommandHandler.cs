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
            _ibHost.ViewModel = viewModel;
            
            var connected = _ibHost.ConnectAndStartReaderThread();
            if(connected)
            {
                //_ibHost.ReqHistoricalData();
                _ibHost.ReqPositions();
                
            }
            else
            {
                //_ibHost.ApplyDefaultHistoricalData();
            }
        }
    }
}
