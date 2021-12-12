using IBSampleApp.messages;
using System;

namespace Rollover.Ib
{
    public interface IIbClientWrapper
    {
        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;
    }
}