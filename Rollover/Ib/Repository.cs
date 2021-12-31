using IBApi;
using IBSampleApp.messages;
using Rollover.Configuration;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace Rollover.Ib
{
    public class Repository : IRepository
    {
        private IIbClientWrapper _ibClient;
        private IIbClientQueue _ibClientQueue;
        private IConfigurationManager _configurationManager;
        private IQueryParametersConverter _queryParametersConverter;
        private IMessageProcessor _messageProcessor;
        private IMessageCollector _messageCollector;

        private List<string> _positions = new List<string>();
        private int _reqIdContractDetails = 0;
        private int _reqIdSecDefOptParam = 0;
        private int _timeout;

        public Repository(
            IIbClientWrapper ibClient,
            IIbClientQueue ibClientQueue,
            IConfigurationManager configurationManager,
            IQueryParametersConverter queryParametersConverter,
            IMessageProcessor messageProcessor, 
            IMessageCollector messageCollector)
        {
            _ibClient = ibClient;
            _ibClientQueue = ibClientQueue;
            _configurationManager = configurationManager;

            _timeout = _configurationManager.GetConfiguration().Timeout;
            _queryParametersConverter = queryParametersConverter;
            _messageProcessor = messageProcessor;
            _messageCollector = messageCollector;
        }

        public Tuple<bool, List<string>> Connect(string host, int port, int clientId)
        {
            ConnectionMessages connectionMessages = _messageCollector.eConnect(host, port, clientId);
            return ConnectionMessagesToConnectionTuple(connectionMessages);
        }

        public void Disconnect()
        {
            _ibClient.eDisconnect();
        }

        public List<PositionMessage> AllPositions()
        {
            return _messageCollector.reqPositions();    
        }

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

        private Tuple<bool, List<string>> ConnectionMessagesToConnectionTuple(ConnectionMessages connectionMessages)
        {
            var connectionTuple = new Tuple<bool, List<string>>(connectionMessages.Connected, new List<string>());

            connectionMessages.OnErrorMessages.ForEach(m => connectionTuple.Item2.Add(m));

            var managedAccountsMessageAsString = _messageProcessor.ConvertMessage(connectionMessages.ManagedAccountsMessage);
            connectionTuple.Item2.AddRange(managedAccountsMessageAsString);
            var connectionStatusMessageAsString = _messageProcessor.ConvertMessage(connectionMessages.ConnectionStatusMessage);
            connectionTuple.Item2.AddRange(connectionStatusMessageAsString);

            return connectionTuple;
        }

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
