using Rollover.Configuration;
using Rollover.Input;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;

namespace Rollover.Ib
{
    public class Repository : IRepository
    {
        private IIbClientWrapper _ibClient;
        private IConnectedCondition _connectedCondition;
        private IInputQueue _inputQueue;
        private IConfigurationManager _configurationManager;
        
        private List<string> _positions = new List<string>();
        private int _reqIdContractDetails = 0;
        private int _timeout;

        public Repository(
            IIbClientWrapper ibClient,
            IConnectedCondition connectedCondition,
            IInputQueue inputQueue,
            IConfigurationManager configurationManager)
        {
            _ibClient = ibClient;
            _connectedCondition = connectedCondition;
            _inputQueue = inputQueue;
            _configurationManager = configurationManager;

            _timeout = _configurationManager.GetConfiguration().Timeout;
        }

        #region Connect, Disconnect

        public Tuple<bool, List<string>> Connect(string host, int port, int clientId)
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

        private Tuple<bool, List<string>>  CheckConnectionMessages(IInputQueue inputQueue, int timeout)
        {
            var messages = new List<string>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var input = inputQueue.Dequeue();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                messages.Add(input);    

                _connectedCondition.AddInput(input);
                if (_connectedCondition.IsConnected())
                {
                    return new Tuple<bool, List<string>>(true, messages);
                }
            }

            return new Tuple<bool, List<string>>(false, messages);
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

        private void ContractDetails(int reqId, IBApi.Contract contract)
        {
            _ibClient.ContractDetails(reqId, contract);
        }

        public ITrackedSymbol GetTrackedSymbol(IBApi.Contract contract)
        {
            var reqId = _reqIdContractDetails + 1;
            _ibClient.ContractDetails(reqId, contract);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (stopWatch.Elapsed.TotalMilliseconds < _timeout)
            {
                var input = _inputQueue.Dequeue();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                var trackedSymbol = JsonSerializer.Deserialize<TrackedSymbol>(input);
                if (trackedSymbol?.ReqIdContractDetails == reqId )
                {
                    return trackedSymbol;
                }
            }

            return null;
        }

        public void ReqSecDefOptParams(int reqId, string symbol, string exchange, string secType, int conId)
        {
            _ibClient.ReqSecDefOptParams(reqId, symbol, exchange, secType, conId);
        }
    }
}
