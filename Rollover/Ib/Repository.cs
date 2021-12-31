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
        private ITrackedSymbolFactory _trackedSymbolFactory;

        private List<string> _positions = new List<string>();
        private int _timeout;

        public Repository(
            IIbClientWrapper ibClient,
            IIbClientQueue ibClientQueue,
            IConfigurationManager configurationManager,
            IQueryParametersConverter queryParametersConverter,
            IMessageProcessor messageProcessor,
            IMessageCollector messageCollector, ITrackedSymbolFactory trackedSymbolFactory)
        {
            _ibClient = ibClient;
            _ibClientQueue = ibClientQueue;
            _configurationManager = configurationManager;

            _timeout = _configurationManager.GetConfiguration().Timeout;
            _queryParametersConverter = queryParametersConverter;
            _messageProcessor = messageProcessor;
            _messageCollector = messageCollector;
            _trackedSymbolFactory = trackedSymbolFactory;
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

        public ITrackedSymbol GetTrackedSymbol(Contract contract)
        {
            //var reqId = ++_reqIdContractDetails;
            //_ibClient.reqContractDetails(reqId, contract);
            //var contractDetailsMessage = ReadContractDetails(reqId);

            //// Under contract
            //var underContract = UnderContractFromContractDetailsMessage(contractDetailsMessage);
            //reqId = ++_reqIdContractDetails;
            //_ibClient.reqContractDetails(reqId, underContract);
            //contractDetailsMessage = ReadContractDetails(reqId);


            //var trackedSymbol = TrackedSymbolFromContractDetailsMessage(contractDetailsMessage, reqId);

            //// TODO
            //if (trackedSymbol != null)
            //{
            //    trackedSymbol.ReqIdSecDefOptParams = ++_reqIdSecDefOptParam;
            //    ReqSecDefOptParams(trackedSymbol);
            //}

            //// (strike, overNextStrike) ReadSecDefOptParams(strike)
            //// update trackedSymbol

            //return trackedSymbol;

            var contractDetailsMessageList = _messageCollector.reqContractDetails(contract);
            if (contractDetailsMessageList.Count() > 1)
            {
                throw new ApplicationException("Unexpected. Multiple ContractDetailsMessages");
            }
            var contractDetailsMessage = contractDetailsMessageList.First();

            var trackedSymbol = _trackedSymbolFactory.InitFromContractDetailsMessage(contractDetailsMessage);

            var underContract = UnderContractFromContractDetailsMessage(contractDetailsMessage);
            if (underContract != null)
            {
                var underContractDetailsMessageList = _messageCollector.reqContractDetails(underContract);
                var underContractDetailsMessage = underContractDetailsMessageList
                    .First(c => c.ContractDetails.ContractMonth == contractDetailsMessage.ContractDetails.ContractMonth);

                var secondUnderContract = UnderContractFromContractDetailsMessage(underContractDetailsMessage);
                if (secondUnderContract != null)
                {
                    var secondUnderContractDetailsMessageList = _messageCollector.reqContractDetails(secondUnderContract);
                    if (secondUnderContractDetailsMessageList.Count() > 1)
                    {
                        throw new ApplicationException("Unexpected. Multiple secondUnderContractDetailsMessage");
                    }

                    var secondUnderContractDetailsMessage = secondUnderContractDetailsMessageList.First();

                    var secDefOptParamMessageList = _messageCollector.reqSecDefOptParams(
                        secondUnderContractDetailsMessage.ContractDetails.Contract.Symbol,
                        secondUnderContractDetailsMessage.ContractDetails.Contract.Exchange,
                        secondUnderContractDetailsMessage.ContractDetails.Contract.SecType,
                        secondUnderContractDetailsMessage.ContractDetails.Contract.ConId
                        );
                    var lastTradeDateOrContractMonth = contractDetailsMessage.ContractDetails.Contract.LastTradeDateOrContractMonth;
                    var secDefOptParamMessageExpirationList = secDefOptParamMessageList
                        .Where(s => s.Expirations.Contains(lastTradeDateOrContractMonth));
                    if (secDefOptParamMessageExpirationList.Count() > 1)
                    {
                        throw new ApplicationException("Unexpected. Multiple secDefOptParamMessageExpirationList");
                    }
                    var secDefOptParamMessage = secDefOptParamMessageExpirationList.First();
                }
                else
                {

                }
            }

            return trackedSymbol;
        }

        private static Contract UnderContractFromContractDetailsMessage(ContractDetailsMessage contractDetailsMessage)
        {
            if (string.IsNullOrWhiteSpace(contractDetailsMessage.ContractDetails.UnderSecType))
            {
                return null;
            }

            return new Contract
            {
                SecType = contractDetailsMessage.ContractDetails.UnderSecType,
                Symbol = contractDetailsMessage.ContractDetails.Contract.Symbol,
                Currency = contractDetailsMessage.ContractDetails.Contract.Currency,
                Exchange = contractDetailsMessage.ContractDetails.Contract.Exchange
            };
        }

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
    }
}
