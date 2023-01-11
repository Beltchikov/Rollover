using SsbHedger.Model;

namespace SsbHedger
{
    public interface IIbHost
    {
        public MainWindowViewModel? ViewModel { get; set; }
        public void ConnectAndStartReaderThread();
        public void Disconnect();

        public void ReqHistoricalData();
    }
}