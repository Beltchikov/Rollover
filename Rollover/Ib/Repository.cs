using IBApi;
using IBSampleApp.messages;
using Rollover.Configuration;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace Rollover.Ib
{
    public class Repository : IRepository
    {
        private IIbClientWrapper _ibClient;
        private IConnectedCondition _connectedCondition;
        private IIbClientQueue _ibClientQueue;
        private IConfigurationManager _configurationManager;
        private IQueryParametersConverter _queryParametersConverter;
        private IMessageProcessor _messageProcessor;

        private List<string> _positions = new List<string>();
        private int _reqIdContractDetails = 0;
        private int _reqIdSecDefOptParam = 0;
        private int _timeout;

        public Repository(
            IIbClientWrapper ibClient,
            IConnectedCondition connectedCondition,
            IIbClientQueue ibClientQueue,
            IConfigurationManager configurationManager,
            IQueryParametersConverter queryParametersConverter,
            IMessageProcessor messageProcessor)
        {
            _ibClient = ibClient;
            _connectedCondition = connectedCondition;
            _ibClientQueue = ibClientQueue;
            _configurationManager = configurationManager;

            _timeout = _configurationManager.GetConfiguration().Timeout;
            _queryParametersConverter = queryParametersConverter;
            _messageProcessor = messageProcessor;
        }

        #region Connect, Disconnect

        public Tuple<bool, List<string>> Connect(string host, int port, int clientId)
        {
            ConnectAndStartConsoleThread(host, port, clientId);
            return CheckConnectionMessages(_ibClientQueue, _configurationManager.GetConfiguration().Timeout);
        }

        private void ConnectAndStartConsoleThread(string host, int port, int clientId)
        {
            _ibClient.eConnect(host, port, clientId);

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

        private Tuple<bool, List<string>> CheckConnectionMessages(IIbClientQueue ibClientQueue, int timeout)
        {
            var messages = new List<string>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var message = ibClientQueue.Dequeue();
                var input = _messageProcessor.ConvertMessage(message);
                if (!input.Any())
                {
                    continue;
                }

                messages.AddRange(input);

                input.ForEach(i => _connectedCondition.AddInput(i));
                if (_connectedCondition.IsConnected())
                {
                    return new Tuple<bool, List<string>>(true, messages);
                }
            }

            return new Tuple<bool, List<string>>(false, messages);
        }

        public void Disconnect()
        {
            _ibClient.eDisconnect();
        }

        #endregion

        #region AllPositions

        public List<string> AllPositions()
        {
            ListPositions();
            ReadPositions(_ibClientQueue, _configurationManager.GetConfiguration().Timeout);

            var positionsBuffer = new List<string>(_positions);
            _positions.Clear();
            return positionsBuffer;
        }

        private void ListPositions()
        {
            _ibClient.reqPositions();
        }

        private void ReadPositions(IIbClientQueue ibClientQueue, int timeout)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var message = ibClientQueue.Dequeue();
                var input = _messageProcessor.ConvertMessage(message);
                if (!input.Any())
                {
                    continue;
                }

                if (input.Any(m => m == Constants.ENTER_SYMBOL_TO_TRACK))
                {
                    _positions.AddRange(input);
                    return;
                }
            }
        }

        #endregion

        #region GetTrackedSymbol

        public ITrackedSymbol GetTrackedSymbol(Contract contract)
        {
            var reqId = ++_reqIdContractDetails;
            _ibClient.reqContractDetails(reqId, contract);
            var contractDetailsMessage = ReadContractDetails(reqId);
            
            // Under contract
            var underContract = UnderContractFromContractDetailsMessage(contractDetailsMessage);
            reqId = ++_reqIdContractDetails;
            _ibClient.reqContractDetails(reqId, underContract);
            contractDetailsMessage = ReadContractDetails(reqId);


            var trackedSymbol = TrackedSymbolFromContractDetailsMessage(contractDetailsMessage, reqId);

            // TODO
            if (trackedSymbol != null)
            {
                trackedSymbol.ReqIdSecDefOptParams = ++_reqIdSecDefOptParam;
                ReqSecDefOptParams(trackedSymbol);
            }

            // (strike, overNextStrike) ReadSecDefOptParams(strike)
            // update trackedSymbol

            return trackedSymbol;
        }

        private static Contract UnderContractFromContractDetailsMessage(ContractDetailsMessage contractDetailsMessage)
        {
            return new Contract
            {
                SecType = contractDetailsMessage.ContractDetails.UnderSecType,
                Symbol = contractDetailsMessage.ContractDetails.Contract.Symbol,
                Currency = contractDetailsMessage.ContractDetails.Contract.Currency,
                Exchange = contractDetailsMessage.ContractDetails.Contract.Exchange
            };
        }

        private ITrackedSymbol TrackedSymbolFromContractDetailsMessage(ContractDetailsMessage contractDetailsMessage, int reqId)
        {
            var input = _messageProcessor.ConvertMessage(contractDetailsMessage);
            if (input.Any())
            {
                var trackedSymbol = JsonSerializer.Deserialize<TrackedSymbol>(input.First());
                if (trackedSymbol?.ReqIdContractDetails == reqId)
                {
                    return trackedSymbol;
                }
            }

            return null;
        }

        private ContractDetailsMessage ReadContractDetails(int reqId)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (stopWatch.Elapsed.TotalMilliseconds < _timeout)
            {
                var message = _ibClientQueue.Dequeue() as ContractDetailsMessage;
                if (message == null)
                {
                    continue;
                }

                return message;
            }

            return null;
        }

        #endregion


        private void ReqSecDefOptParams(ITrackedSymbol trackedSymbol)
        {
            //_ibClient.ReqSecDefOptParams(
            //    trackedSymbol.ReqIdSecDefOptParams, 
            //    trackedSymbol.Symbol, 
            //    trackedSymbol.Exchange, 
            //    //trackedSymbol.SecType, 
            //    "IND",
            //    //trackedSymbol.ConId
            //    362687422);

            var trackedSymbolCopy = _queryParametersConverter.TrackedSymbolForReqSecDefOptParams(trackedSymbol);

            _ibClient.reqSecDefOptParams(
                trackedSymbolCopy.ReqIdSecDefOptParams,
                trackedSymbolCopy.Symbol,
                trackedSymbolCopy.Exchange,
                trackedSymbolCopy.SecType,
                trackedSymbolCopy.ConId);
        }
    }
}
