using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using Rollover.Input;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace Rollover.Ib
{
    public class IbClientWrapper : IIbClientWrapper
    {
        private EReaderMonitorSignal _signal;
        private IBClient _ibClient;
        private IIbClientQueue _ibClientQueue;
        private IPortfolio _portfolio;
        private ITrackedSymbolFactory _trackedSymbolFactory;


        private static List<string> _localSymbolsList = new List<string>();

        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;

        public IbClientWrapper(
            IIbClientQueue inputQueue,
            IPortfolio portfolio, 
            ITrackedSymbolFactory trackedSymbolFactory)
        {
            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);
            _ibClientQueue = inputQueue;
            _portfolio = portfolio;

            RegisterResponseHandlers();
            _trackedSymbolFactory = trackedSymbolFactory;
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
            _ibClientQueue.Enqueue($"id={id} errorCode={errorCode} msg={msg} Exception={ex}");
        }

        public void OnNextValidId(ConnectionStatusMessage connectionStatusMessage)
        {
            //string msg = connectionStatusMessage.IsConnected
            //    ? "Connected."
            //    : "Disconnected.";
            //_ibClientQueue.Enqueue(msg);

            _ibClientQueue.Enqueue(connectionStatusMessage);
        }

        public void OnManagedAccounts(ManagedAccountsMessage managedAccountsMessage)
        {
            //if (!managedAccountsMessage.ManagedAccounts.Any())
            //{
            //    throw new Exception("Unexpected");
            //}

            //string msg = Environment.NewLine + "Accounts found: " + managedAccountsMessage.ManagedAccounts.Aggregate((r, n) => r + ", " + n);
            //_ibClientQueue.Enqueue(msg);

            _ibClientQueue.Enqueue(managedAccountsMessage);
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
            if(obj.Position == 0)
            {
                return;
            }

            _portfolio.Add(obj);
            var localSymbol = obj.Contract.LocalSymbol;
            _localSymbolsList.Add(localSymbol);
        }

        public void OnPositionEnd()
        {
            _localSymbolsList.Sort();
            _localSymbolsList.ForEach(s => _ibClientQueue.Enqueue(s));
            _localSymbolsList.Clear();
            _ibClientQueue.Enqueue(Reducer.ENTER_SYMBOL_TO_TRACK);
        }

        #endregion

        #region ContractDetails

        public void ContractDetails(int reqId, Contract contract)
        {
            _ibClient.ClientSocket.reqContractDetails(reqId, contract);
        }

        public void OnContractDetails(ContractDetailsMessage obj)
        {
            var _trackedSymbol = _trackedSymbolFactory.FromContractDetailsMessage(obj);
            var serialized = JsonSerializer.Serialize(_trackedSymbol);
            _ibClientQueue.Enqueue(serialized);
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
            // obj = 1;
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
