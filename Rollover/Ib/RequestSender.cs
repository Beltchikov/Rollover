using Rollover.Input;
using System;
using System.Threading;

namespace Rollover.Ib
{
    public class RequestSender : IRequestSender
    {
        private IIbClientWrapper _ibClient;

        public RequestSender(IIbClientWrapper ibClient)
        {
            _ibClient = ibClient;
        }

        public void Connect(string host, int port, int clientId)
        {
            _ibClient.Connect(host, port, clientId);

            var reader = _ibClient.ReaderFactory();
            reader.Start();

            new Thread(() =>
            {
                while (_ibClient.IsConnected())
                {
                    _ibClient.WaitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }
            .Start();
        }
        public void Disconnect()
        {
            _ibClient.Disconnect();
        }

        public void RegisterResponseHandlers(IInputQueue _inputQueue, SynchronizationContext synchronizationContext)
        {
            _ibClient.RegisterResponseHandlers(_inputQueue, synchronizationContext);
        }

        public void ListPositions()
        {
            _ibClient.ListPositions();
        }
        public void ReqSecDefOptParams(int reqId, string symbol, string exchange, string secType, int conId)
        {
            _ibClient.ReqSecDefOptParams(reqId, symbol, exchange, secType, conId);
        }

        public void ContractDetails(int reqId, IBApi.Contract contract)
        {
            _ibClient.ContractDetails(reqId, contract);
        }
    }
}
