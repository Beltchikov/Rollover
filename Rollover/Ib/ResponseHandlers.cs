using IBSampleApp.messages;
using Rollover.Input;
using System;

namespace Rollover.Ib
{
    public static class ResponseHandlers
    {
        public static IInputQueue InputQueue { get; set; }

        public static void OnError(int arg1, int arg2, string arg3, Exception arg4)
        {
            throw new NotImplementedException();
        }

        public static void NextValidId(ConnectionStatusMessage obj)
        {
            throw new NotImplementedException();
        }

        public static void ManagedAccounts(ManagedAccountsMessage obj)
        {
            throw new NotImplementedException();
        }
    }
}
