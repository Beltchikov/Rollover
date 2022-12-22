using IbClient.messages;
using System;

namespace SsbHedger2.WpfIbClient
{
    public interface IWpfIbClient 
    {
        event Action<ConnectionStatusMessage> NextValidId;
        event Action<int, string> Error;
        event Action<ManagedAccountsMessage> ManagedAccounts;

        void Execute(string host, int port, int clientId);
        void InvokeError(int reqId, string message);
        void InvokeNextValidId(ConnectionStatusMessage message);
        void InvokeManagedAccounts(ManagedAccountsMessage message);
    }
}