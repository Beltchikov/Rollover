using SsbHedger2.Model;
using System;

namespace SsbHedger2
{
    public interface IIbHost
    {
        public MainWindowViewModel? ViewModel { get; set; }
        string DefaultHost { get; }
        int DefaultPort { get; }
        int DefaultClientId { get; }

        public void ConnectAndStartReaderThread(string host, int port, int clientId);
    }
}