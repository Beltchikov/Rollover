using IBApi;
using IBSampleApp.messages;
using System;
using System.Threading;

namespace Rollover.Ib
{
    public interface IIbClientWrapper
    {
        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;

        void Connect(string host, int port, int clientId);
        bool IsConnected();
        void Disconnect();
        void ListPositions();
        void ReqSecDefOptParams(int reqId, string symbol, string exchange, string secType, int conId);
        void ContractDetails(int reqId, Contract contract);
        EReader ReaderFactory();
        void WaitForSignal();
    }
}