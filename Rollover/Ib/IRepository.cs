using IBApi;
using Rollover.Tracking;
using System;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public interface IRepository
    {
        Tuple<bool, List<string>> Connect(string host, int port, int clientId);
        void Disconnect();
        List<string> AllPositions();
        void ReqSecDefOptParams(int reqId, string symbol, string exchange, string secType, int conId);
        void ContractDetails(int reqId, Contract contract);
        ITrackedSymbol GetTrackedSymbol(Contract contract);
    }
}