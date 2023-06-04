using SsbHedger.Model;
using System.Threading.Tasks;

namespace SsbHedger.CommandHandler
{
    public sealed class InitializeCommandHandler : IInitializeCommandHandler
    {
       private IIbHost _ibHost = null!;

        public InitializeCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public async Task HandleAsync(MainWindowViewModel viewModel)
        {
            _ibHost.ViewModel = viewModel;
            
            var connected = await _ibHost.ConnectAndStartReaderThread();
            if(connected)
            {
                //_ibHost.ReqHistoricalData();
                _ibHost.ReqPositions();
                
            }
            else
            {
                _ibHost.ApplyDefaultHistoricalData();
            }
        }
    }
}
