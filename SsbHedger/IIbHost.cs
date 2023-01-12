﻿using SsbHedger.Model;
using System.Threading.Tasks;

namespace SsbHedger
{
    public interface IIbHost
    {
        public MainWindowViewModel? ViewModel { get; set; }
        public Task<bool> ConnectAndStartReaderThread();
        public void Disconnect();
        public void ReqHistoricalData();
        void ApplyDefaultHistoricalData();
    }
}