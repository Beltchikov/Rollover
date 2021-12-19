using IBSampleApp.messages;
using Rollover.Input;
using System;
using System.Linq;
using System.Threading;

namespace Rollover.Ib
{
    public class ResponseHandlers : IResponseHandlers
    {
        private readonly IInputQueue _inputQueue;
        private static IResponseHandlers _responseHandlers;
        private static object _locker = new object();

        private ResponseHandlers(IInputQueue inputQueue)
        {
            _inputQueue = inputQueue;
        }

        public static IResponseHandlers CreateInstance(IInputQueue inputQueue)
        {
            lock (_locker)
            {
                if (_responseHandlers == null)
                {
                    _responseHandlers = new ResponseHandlers(inputQueue);
                }
                return _responseHandlers; 
            }
        }

        public SynchronizationContext SynchronizationContext { get; set; }

        public void OnError(int id, int errorCode, string msg, Exception ex)
        {
            _inputQueue.Enqueue($"id={id} errorCode={errorCode} msg={msg} Exception={ex}");
        }

        public void NextValidId(ConnectionStatusMessage connectionStatusMessage)
        {
            string msg = connectionStatusMessage.IsConnected
                ? "Connected."
                : "Disconnected.";
            _inputQueue.Enqueue(msg);
        }

        public void ManagedAccounts(ManagedAccountsMessage managedAccountsMessage)
        {
            if (!managedAccountsMessage.ManagedAccounts.Any())
            {
                throw new Exception("Unexpected");
            }

            string msg = Environment.NewLine + "Accounts found: " + managedAccountsMessage.ManagedAccounts.Aggregate((r, n) => r + ", " + n);
            _inputQueue.Enqueue(msg);
        }

        public void OnPosition(PositionMessage obj)
        {
            var localSymbol = obj.Contract.LocalSymbol;
            _inputQueue.Enqueue(localSymbol);
        }

        public void OnPositionEnd()
        {
            _inputQueue.Enqueue("Enter a command:");
        }
    }
}
