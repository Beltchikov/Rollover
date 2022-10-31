using IbClient.messages;
using System;

namespace SsbHedger.ResponseProcessing
{
    public class ResponseHandler : IResponseHandler
    {
        IReaderThreadQueue _queue;

        public ResponseHandler(IReaderThreadQueue queue)
        {
            _queue = queue;
        }

        public void OnError(int reqId, int code, string message, Exception exception)
        {
            _queue.Enqueue(new ErrorInfo(reqId, code, message, exception));
        }

        public void OnManagedAccounts(ManagedAccountsMessage managedAccountsMessage)
        {
            _queue.Enqueue(managedAccountsMessage);
        }

        public void OnNextValidId(ConnectionStatusMessage connectionStatusMessage)
        {
            _queue.Enqueue(connectionStatusMessage);
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
