using IBSampleApp.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Ib
{
    public static class ResponseHandlers
    {
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
