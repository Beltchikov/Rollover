using SsbHedger2.Model;
using System;

namespace SsbHedger2
{
    internal interface IIbHost
    {
        public MainWindowViewModel? ViewModel { get; set; }

        public void ConnectAndStartReaderThread(string host, int port, int clientId);
    }
}