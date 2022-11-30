using IbClient.messages;
using SsbHedger.Abstractions;
using SsbHedger.WpfIbClient;
using System;

namespace SsbHedger.ResponseProcessing
{
    public class ResponseHandler : IResponseHandler
    {
        private readonly object OPEN_ORDER_END = "OpenOrderEnd";
        IReaderThreadQueue _queue;
        IDispatcherAbstraction _dispatcherAbstraction;
        IWpfIbClient _client;
        
        public ResponseHandler(
            IReaderThreadQueue queue,
            IDispatcherAbstraction dispatcherAbstraction)
        {
            _queue = queue;
            _dispatcherAbstraction = dispatcherAbstraction;
        }

        internal IReaderThreadQueue ReaderQueue => _queue;
   
        public object? Dequeue()
        {
            return _queue.Dequeue();
        }

        public void HandleNextMessage()
        {
            var message = Dequeue();
            if(message == null)
            {
                return;
            }

            if(message is ErrorInfo errorInfo)
            {
                _dispatcherAbstraction.Invoke(() 
                    => _client.InvokeError(errorInfo.ReqId, 
                    $"{errorInfo.Message} Exception: {errorInfo.exception}"));
            }
            else if (message is ConnectionStatusMessage connectionStatusMessage)
            {
                _dispatcherAbstraction.Invoke(()
                    => _client.InvokeNextValidId(connectionStatusMessage));
            }
            else if (message is ManagedAccountsMessage managedAccountsMessage)
            {
                _dispatcherAbstraction.Invoke(()
                    => _client.InvokeManagedAccounts(managedAccountsMessage));
            }
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

        public void SetClient(IWpfIbClient wpfIbClient)
        {
            _client = wpfIbClient;
        }
    }
}
