using IbClient.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger
{
    public class ResponseHandler : IResponseHandler
    {
        public void OnError(int refId, int code, string message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void OnManagedAccounts(ManagedAccountsMessage managedAccountsMessage)
        {
            throw new NotImplementedException();
        }

        public void OnNextValidId(ConnectionStatusMessage connectionStatusMessage)
        {
            throw new NotImplementedException();
        }

        public void OnOpenOrder(OpenOrderMessage obj)
        {
            throw new NotImplementedException();
        }

        public void OnOpenOrderEnd()
        {
            throw new NotImplementedException();
        }

        public void OnOrderStatus(OrderStatusMessage obj)
        {
            throw new NotImplementedException();
        }
    }
}
