using IBSampleApp.messages;
using System;
using System.Threading;

namespace Rollover.Ib
{
    public interface IResponseHandlers
    {
        SynchronizationContext SynchronizationContext { get; set; }

        void ManagedAccounts(ManagedAccountsMessage managedAccountsMessage);
        void NextValidId(ConnectionStatusMessage connectionStatusMessage);
        void OnError(int id, int errorCode, string msg, Exception ex);
        void OnPosition(PositionMessage obj);
        void OnPositionEnd();
    }
}