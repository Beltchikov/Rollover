using IbClient.messages;
using System;

namespace SsbHedger.ResponseProcessing
{
    public class ResponseHandler : IResponseHandler
    {
        private readonly object OPEN_ORDER_END = "OpenOrderEnd";
        IReaderThreadQueue _queue;

        public ResponseHandler(IReaderThreadQueue queue)
        {
            _queue = queue;
        }

        public IReaderThreadQueue ReaderQueue => _queue;

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

        public void OnOpenOrder(OpenOrderMessage openOrderMessage)
        {
            _queue.Enqueue(openOrderMessage);
        }

        public void OnOpenOrderEnd()
        {
            _queue.Enqueue(OPEN_ORDER_END);
        }

        public void OnOrderStatus(OrderStatusMessage orderStatusMessage)
        {
            _queue.Enqueue(orderStatusMessage);
        }
    }
}
