using SsbHedger.Model;

namespace SsbHedger
{
    public interface IIbHost
    {
        public MainWindowViewModel? ViewModel { get; set; }
        public void ConnectAndStartReaderThread(string host, int port, int clientId);
        public void Disconnect();

        public void ReqHistoricalData();
    }
}