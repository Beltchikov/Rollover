using IBSampleApp.messages;
using Rollover.Input;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Rollover.Ib
{
    public class ResponseHandlers : IResponseHandlers
    {
        private readonly IInputQueue _inputQueue;
        private readonly IPortfolio _portfolio;
        private static IResponseHandlers _responseHandlers;
        private static object _locker = new object();
        private static List<string> _localSymbolsList = new List<string>();

        private ResponseHandlers(IInputQueue inputQueue, IPortfolio portfolio)
        {
            _inputQueue = inputQueue;
            _portfolio = portfolio;
        }

        [ExcludeFromCodeCoverage]
        public static IResponseHandlers CreateInstance(IInputQueue inputQueue, IPortfolio portfolio)
        {
            lock (_locker)
            {
                return _responseHandlers ??= new ResponseHandlers(inputQueue, portfolio);
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
            _portfolio.Add(obj);
            var localSymbol = obj.Contract.LocalSymbol;
            _localSymbolsList.Add(localSymbol);
        }

        public void OnPositionEnd()
        {
            _localSymbolsList.ForEach(s => _inputQueue.Enqueue(s));
            _localSymbolsList.Clear();
            _inputQueue.Enqueue("Enter a symbol to track:");
        }
    }
}
