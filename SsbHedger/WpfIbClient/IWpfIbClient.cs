using IbClient.messages;
using System;

namespace SsbHedger.WpfIbClient
{
    public interface IWpfIbClient 
    {
        event Action<ConnectionStatusMessage> NextValidId;
        event Action<int, string> Error;

        void Execute();
        void InvokeError(int reqId, string message);
        void InvokeNextValidId(ConnectionStatusMessage message);
        void InvokeManagedAccounts(ManagedAccountsMessage message);
    }
}