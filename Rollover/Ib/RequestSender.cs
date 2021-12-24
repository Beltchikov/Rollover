using Rollover.Input;
using System;
using System.Diagnostics;
using System.Threading;

namespace Rollover.Ib
{
    public class RequestSender : IRequestSender
    {
        private IIbClientWrapper _ibClient;
        private IConnectedCondition _connectedCondition;

        public RequestSender(IIbClientWrapper ibClient, IConnectedCondition connectedCondition)
        {
            _ibClient = ibClient;
            _connectedCondition = connectedCondition;
        }

        public bool Connect(string host, int port, int clientId, IInputQueue inputQueue, int timeout)
        {
            ConnectAndStartConsoleThread(host, port, clientId);
            return CheckConnectionMessages(inputQueue, timeout);
        }

        private void ConnectAndStartConsoleThread(string host, int port, int clientId)
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

        private bool CheckConnectionMessages(IInputQueue inputQueue, int timeout)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var input = inputQueue.Dequeue();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                _connectedCondition.AddInput(input);
                if (_connectedCondition.IsConnected())
                {
                    return true;
                }
            }

            return false;
        }
        public void Disconnect()
        {
            _ibClient.Disconnect();
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
