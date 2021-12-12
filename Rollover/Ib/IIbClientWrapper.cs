using IBApi;
using IBSampleApp.messages;
using System;

namespace Rollover.Ib
{
    public interface IIbClientWrapper
    {
        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;

        void Connect(string host, int port, int clientId);
        EReader ReaderFactory();
        bool IsConnected();
        void WaitForSignal();
    }
}