using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using Rollover.Input;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rollover.Ib
{
    public class IbClientWrapper : IIbClientWrapper
    {
        private EReaderMonitorSignal _signal;
        private IBClient _ibClient;
        private SynchronizationContext _synchronizationContext;
        private IInputQueue _inputQueue;
        private IPortfolio _portfolio;
        private static List<string> _localSymbolsList = new List<string>();

        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;

        public IbClientWrapper(SynchronizationContext synchronizationContext, IInputQueue inputQueue, IPortfolio portfolio)
        {
            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);
            _synchronizationContext = synchronizationContext;
            _inputQueue = inputQueue;
            _portfolio = portfolio;

            RegisterResponseHandlers();

        }

        #region Connect, Disconnect

        public void Connect(string host, int port, int clientId)
        {
            _ibClient.ClientSocket.eConnect(host, port, clientId);
        }

        public bool IsConnected()
        {
            return _ibClient.ClientSocket.IsConnected();
        }

        public void OnError(int id, int errorCode, string msg, Exception ex)
        {
            _inputQueue.Enqueue($"id={id} errorCode={errorCode} msg={msg} Exception={ex}");
        }

        public void OnNextValidId(ConnectionStatusMessage connectionStatusMessage)
        {
            string msg = connectionStatusMessage.IsConnected
                ? "Connected."
                : "Disconnected.";
            _inputQueue.Enqueue(msg);
        }

        public void OnManagedAccounts(ManagedAccountsMessage managedAccountsMessage)
        {
            if (!managedAccountsMessage.ManagedAccounts.Any())
            {
                throw new Exception("Unexpected");
            }

            string msg = Environment.NewLine + "Accounts found: " + managedAccountsMessage.ManagedAccounts.Aggregate((r, n) => r + ", " + n);
            _inputQueue.Enqueue(msg);
        }

        public void Disconnect()
        {
            if (!IsConnected())
            {
                return;
            }

            _ibClient.ClientSocket.eDisconnect();
        }

        #endregion

        #region ListPositions

        public void ListPositions()
        {
            _ibClient.ClientSocket.reqPositions();
        }

        public void OnPosition(PositionMessage obj)
        {
            _portfolio.Add(obj);
            var localSymbol = obj.Contract.LocalSymbol;
            _localSymbolsList.Add(localSymbol);
        }

        public void OnPositionEnd()
        {
            _localSymbolsList.Sort();
            _localSymbolsList.ForEach(s => _inputQueue.Enqueue(s));
            _localSymbolsList.Clear();
            _inputQueue.Enqueue(Reducer.ENTER_SYMBOL_TO_TRACK);
        }

        #endregion

        #region ContractDetails

        public void ContractDetails(int reqId, Contract contract)
        {
            _ibClient.ClientSocket.reqContractDetails(reqId, contract);
        }

        public void OnContractDetails(ContractDetailsMessage obj)
        {
            // obj.RequestId
            var msg = $"ConId={obj.ContractDetails.Contract.ConId} " +
                $"SecType={obj.ContractDetails.Contract.SecType} " +
                $"Symbol={obj.ContractDetails.Contract.Symbol} " +
                $"Currency={obj.ContractDetails.Contract.Currency} " +
                $"Exchange={obj.ContractDetails.Contract.Exchange} " +
                $"Strike={obj.ContractDetails.Contract.Strike } ";

            _inputQueue.Enqueue(msg);
        }

        #endregion

        #region OptionParameters

        public void ReqSecDefOptParams(int reqId, string symbol, string exchange, string secType, int conId)
        {
            _ibClient.ClientSocket.reqSecDefOptParams(reqId, symbol, exchange, secType, conId);
        }

        public void OnSecurityDefinitionOptionParameter(SecurityDefinitionOptionParameterMessage obj)
        {
            //throw new NotImplementedException();
        }

        public void OnSecurityDefinitionOptionParameterEnd(int obj)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        public EReader ReaderFactory()
        {
            return new EReader(_ibClient.ClientSocket, _signal);
        }

        public void WaitForSignal()
        {
            _signal.waitForSignal();
        }

        private void RegisterResponseHandlers()
        {
            _ibClient.Error += OnError;
            _ibClient.NextValidId += OnNextValidId;
            _ibClient.ManagedAccounts += OnManagedAccounts;
            _ibClient.Position += OnPosition;
            _ibClient.PositionEnd += OnPositionEnd;
            _ibClient.SecurityDefinitionOptionParameter += OnSecurityDefinitionOptionParameter;
            _ibClient.SecurityDefinitionOptionParameterEnd += OnSecurityDefinitionOptionParameterEnd;
            _ibClient.ContractDetails += OnContractDetails;
        }

        #endregion
    }
}
