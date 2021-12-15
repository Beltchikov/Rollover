using IBSampleApp.messages;
using Rollover.Input;
using System;
using System.Linq;
using System.Threading;

namespace Rollover.Ib
{
    public static class ResponseHandlers
    {
        public static IInputQueue InputQueue { get; set; }
        public static SynchronizationContext SynchronizationContext { get; internal set; }

        public static void OnError(int id, int errorCode, string msg, Exception ex)
        {
            InputQueue.Enqueue($"id={id} errorCode={errorCode} msg={msg} Exception={ex}");
        }

        public static void NextValidId(ConnectionStatusMessage connectionStatusMessage)
        {
            string msg = connectionStatusMessage.IsConnected
                ? "Connected."
                : "Disconnected.";
            InputQueue.Enqueue(msg);
        }

        public static void ManagedAccounts(ManagedAccountsMessage managedAccountsMessage)
        {
            if (!managedAccountsMessage.ManagedAccounts.Any())
            {
                throw new Exception("Unexpected");
            }

            string msg = Environment.NewLine + "Accounts found: " + managedAccountsMessage.ManagedAccounts.Aggregate((r, n) => r + ", " + n);
            InputQueue.Enqueue(msg);
        }
    }
}
