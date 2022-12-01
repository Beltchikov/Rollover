using IbClient.messages;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing.Command;
using SsbHedger.WpfIbClient;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SsbHedger.ResponseProcessing
{
    public class ResponseHandler : IResponseHandler
    {
        private readonly object OPEN_ORDER_END = "OpenOrderEnd";
        IReaderThreadQueue _queue;
        IDispatcherAbstraction _dispatcherAbstraction;
        IWpfIbClient _client;
        Dictionary<Type, List<ResponseCommand>> _responseActionMap = new();

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

            if (message is ErrorInfo errorInfo)
            {
                _dispatcherAbstraction.Invoke(()
                    => _client.InvokeError(errorInfo.ReqId,
                    $"Code:{errorInfo.Code} Message:{errorInfo.Message} Exception:{errorInfo.Exception}"));
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

            //var commands = _responseActionMap[message.GetType()];
            //foreach(var command in commands)
            //{
            //    command.SetParameters(message);


            //    _dispatcherAbstraction.Invoke(() => command.Execute());
            //}

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

            _responseActionMap[typeof(ErrorInfo)] = new List<ResponseCommand>{ new CommandErrorInfo(_client) };
        }
    }
}
