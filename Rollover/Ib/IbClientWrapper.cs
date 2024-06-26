﻿using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using System;
using System.Collections.Generic;

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

        public void eConnect(string host, int port, int clientId)
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

        public void eDisconnect()
        {
            if (!IsConnected())
            {
                return;
            }

            _ibClient.ClientSocket.eDisconnect();
        }

        #endregion

        #region ListPositions

        public void reqPositions()
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

        public void reqContractDetails(int reqId, Contract contract)
        {
            _ibClient.ClientSocket.reqContractDetails(reqId, contract);
        }

        public void OnContractDetails(ContractDetailsMessage obj)
        {
            _ibClientQueue.Enqueue(obj);
        }

        private void OnContractDetailsEnd(int obj)
        {
            _ibClientQueue.Enqueue(Constants.ON_CONTRACT_DETAILS_END);
        }

        #endregion

        #region OptionParameters

        public void reqSecDefOptParams(int reqId, string symbol, string exchange, string secType, int conId)
        {
            _ibClient.ClientSocket.reqSecDefOptParams(reqId, symbol, exchange, secType, conId);
        }

        public void OnSecurityDefinitionOptionParameter(SecurityDefinitionOptionParameterMessage obj)
        {
            _ibClientQueue.Enqueue(obj);
        }

        public void OnSecurityDefinitionOptionParameterEnd(int obj)
        {
            _ibClientQueue.Enqueue(Constants.ON_SECURITY_DEFINITION_OPTION_PARAMETER_END);
        }

        #endregion

        #region MktData

        public void reqMktData(
            int reqId,
            Contract contract,
            string generickTickList,
            bool snapshot,
            bool regulatorySnapshot,
            List<TagValue> mktDataOptions)
        {
            _ibClient.ClientSocket.reqMktData(
                reqId,
                contract,
                generickTickList,
                snapshot,
                regulatorySnapshot,
                mktDataOptions);
        }

        public void OnTickSize(TickSizeMessage obj)
        {
            _ibClientQueue.Enqueue(obj);
        }
        public void OnTickPrice(TickPriceMessage obj)
        {
            _ibClientQueue.Enqueue(obj);
        }
        public void OnTickString(int arg1, int arg2, string arg3)
        {
            _ibClientQueue.Enqueue($"{Constants.ON_TICK_STRING} {arg1},{arg2},{arg3}");
        }
        public void OnTickGeneric(int arg1, int arg2, double arg3)
        {
            _ibClientQueue.Enqueue($"{Constants.ON_TICK_GENERIC} {arg1},{arg2},{arg3}");
        }
        private void OnTickSnapshotEnd(int obj)
        {
            _ibClientQueue.Enqueue(Constants.ON_TICK_SNAPSHOT_END);
        }

        #endregion

        #region PlaceOrder
        public void PlaceOrder(Contract contract, Order order)
        {
            _ibClient.ClientSocket.placeOrder(_ibClient.NextOrderId, contract, order);
        }

        public void reqIds()
        {
            _ibClient.ClientSocket.reqIds(-1);
        }
        private void OnOpenOrder(OpenOrderMessage obj)
        {
            _ibClientQueue.Enqueue(obj);
        }
        private void OnOpenOrderEnd()
        {
            _ibClientQueue.Enqueue("OnOpenOrderEnd");
        }
        private void OnOrderStatus(OrderStatusMessage obj)
        {
            _ibClientQueue.Enqueue(obj);
        }

        private void OnExecDetails(ExecutionMessage obj)
        {
            _ibClientQueue.Enqueue(obj);
        }

        private void OnExecDetailsEnd(int obj)
        {
            _ibClientQueue.Enqueue(obj);
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
            _ibClient.ContractDetailsEnd += OnContractDetailsEnd;
            _ibClient.TickSize += OnTickSize;
            _ibClient.TickPrice += OnTickPrice;
            _ibClient.TickString += OnTickString;
            _ibClient.TickGeneric += OnTickGeneric;
            _ibClient.TickSnapshotEnd += OnTickSnapshotEnd;
            _ibClient.OpenOrder += OnOpenOrder;
            _ibClient.OpenOrderEnd += OnOpenOrderEnd;
            _ibClient.OrderStatus += OnOrderStatus;
            _ibClient.ExecDetails += OnExecDetails;
            _ibClient.ExecDetailsEnd += OnExecDetailsEnd;
        }

        #endregion
    }
}
