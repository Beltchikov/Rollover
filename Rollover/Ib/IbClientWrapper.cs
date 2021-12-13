using IBApi;
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

        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;

        public IbClientWrapper()
        {
            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);
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
            ResponseHandlers.InputQueue = _inputQueue;
            ResponseHandlers.SynchronizationContext = synchronizationContext;

            _ibClient.Error += ResponseHandlers.OnError;
            _ibClient.NextValidId += ResponseHandlers.NextValidId;
            _ibClient.ManagedAccounts += ResponseHandlers.ManagedAccounts;
        }

        public void Disconnect()
        {
            if(! IsConnected())
            {
                return;
            }

            _ibClient.ClientSocket.eDisconnect();
        }
    }
}
