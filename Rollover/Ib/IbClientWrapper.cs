﻿using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using Rollover.Input;
using System;
using System.Threading;

namespace Rollover.Ib
{
    public class IbClientWrapper : IIbClientWrapper
    {
        private EReaderMonitorSignal _signal;
        private IBClient _ibClient;
        private IResponseHandlers _responseHandlers;

        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;

        public IbClientWrapper(IResponseHandlers responseHandlers)
        {
            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);
            _responseHandlers = responseHandlers;
        }

        public void Connect(string host, int port, int clientId)
        {
            _ibClient.ClientSocket.eConnect(host, port, clientId);
        }

        public EReader ReaderFactory()
        {
            return new EReader(_ibClient.ClientSocket, _signal);
        }

        public bool IsConnected()
        {
            return _ibClient.ClientSocket.IsConnected();
        }

        public void WaitForSignal()
        {
            _signal.waitForSignal();
        }

        public void RegisterResponseHandlers(IInputQueue _inputQueue, SynchronizationContext synchronizationContext)
        {
            _responseHandlers.SynchronizationContext = synchronizationContext;

            _ibClient.Error += _responseHandlers.OnError;
            _ibClient.NextValidId += _responseHandlers.NextValidId;
            _ibClient.ManagedAccounts += _responseHandlers.ManagedAccounts;
            _ibClient.Position += _responseHandlers.OnPosition;
            _ibClient.PositionEnd += _responseHandlers.OnPositionEnd;
        }

        public void Disconnect()
        {
            if(! IsConnected())
            {
                return;
            }

            _ibClient.ClientSocket.eDisconnect();
        }

        public void ListPositions()
        {
            _ibClient.ClientSocket.reqPositions();
        }
    }
}
