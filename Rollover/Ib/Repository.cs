using Rollover.Configuration;
using Rollover.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Rollover.Ib
{
    public class Repository : IRepository
    {
        private IIbClientWrapper _ibClient;
        private IConnectedCondition _connectedCondition;
        private IConsoleWrapper _consoleWrapper;
        private IInputQueue _inputQueue;
        private IConfigurationManager _configurationManager;
        private List<string> _positions = new List<string>();

        public Repository(
            IIbClientWrapper ibClient,
            IConnectedCondition connectedCondition,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IConfigurationManager configurationManager)
        {
            _ibClient = ibClient;
            _connectedCondition = connectedCondition;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _configurationManager = configurationManager;
        }

        #region Connect, Disconnect

        public bool Connect(string host, int port, int clientId)
        {
            ConnectAndStartConsoleThread(host, port, clientId);
            return CheckConnectionMessages(_inputQueue, _configurationManager.GetConfiguration().Timeout);
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

                _consoleWrapper.WriteLine(input);

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

        #endregion

        #region AllPositions

        public List<string> AllPositions()
        {
            ListPositions();
            ReadPositions(_inputQueue, _configurationManager.GetConfiguration().Timeout);

            var positionsBuffer = new List<string>(_positions);
            _positions.Clear();
            return positionsBuffer;
        }

        private void ListPositions()
        {
            _ibClient.ListPositions();
        }

        private void ReadPositions(IInputQueue inputQueue, int timeout)
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

                if (input == Reducer.ENTER_SYMBOL_TO_TRACK)
                {
                    return;
                }

                _positions.Add(input);
            }
        }

        #endregion

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
