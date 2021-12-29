﻿using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using System;

namespace Rollover.Ib
{
    public class IbClientWrapper : IIbClientWrapper
    {
        private EReaderMonitorSignal _signal;
        private IBClient _ibClient;
        private IIbClientQueue _ibClientQueue;

        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;

        public IbClientWrapper(IIbClientQueue inputQueue)
        {
            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);
            _ibClientQueue = inputQueue;

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
            _ibClientQueue.Enqueue($"id={id} errorCode={errorCode} msg={msg} Exception={ex}");
        }

        public void OnNextValidId(ConnectionStatusMessage connectionStatusMessage)
        {
            _ibClientQueue.Enqueue(connectionStatusMessage);
        }

        public void OnManagedAccounts(ManagedAccountsMessage managedAccountsMessage)
        {
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
            _ibClientQueue.Enqueue(obj);
        }

        public void OnPositionEnd()
        {
            _ibClientQueue.Enqueue(Constants.ON_POSITION_END);
        }

        #endregion

        #region ContractDetails

        public void ContractDetails(int reqId, Contract contract)
        {
            _ibClient.ClientSocket.reqContractDetails(reqId, contract);
        }

        public void OnContractDetails(ContractDetailsMessage obj)
        {
            _ibClientQueue.Enqueue(obj);
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
