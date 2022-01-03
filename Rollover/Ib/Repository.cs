using IBApi;
using IBSampleApp.messages;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Ib
{
    public class Repository : IRepository
    {
        private IIbClientWrapper _ibClient;
        private IMessageProcessor _messageProcessor;
        private IMessageCollector _messageCollector;
        private ITrackedSymbolFactory _trackedSymbolFactory;

        public Repository(
            IIbClientWrapper ibClient,
            IMessageProcessor messageProcessor,
            IMessageCollector messageCollector, ITrackedSymbolFactory trackedSymbolFactory)
        {
            _ibClient = ibClient;
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
            var underLyingContract = GetUnderlyingContract(contract, null);
            HashSet<double> strikes = null;
            if (underLyingContract.SecType == "FUT")
            {
                var secondUnderLyingContract = GetUnderlyingContract(underLyingContract, contract.LastTradeDateOrContractMonth);
                strikes = GetStrikes(secondUnderLyingContract, contract.LastTradeDateOrContractMonth);
                var tickSizePriceTuple = _messageCollector.reqMktData(underLyingContract, "", true, false, null);
                var currentPrice = tickSizePriceTuple.Item2.Price;
                var trackedSymbol = _trackedSymbolFactory.Create(contract, strikes, currentPrice);
                return trackedSymbol;
            }
            else if (underLyingContract.SecType == "STK")
            {
                // TODO
                // strikes = GetStrikes(underLyingContract, contract.LastTradeDateOrContractMonth);
                throw new NotImplementedException();
            }
            else if (underLyingContract.SecType == "IND")
            {
                // TODO

                //strikes = GetStrikes(underLyingContract, contract.LastTradeDateOrContractMonth);
                //var tickSizePriceTuple = _messageCollector.reqMktData(underLyingContract, "", true, false, null);
                //var currentPrice = tickSizePriceTuple.Item2.Price;
                //var trackedSymbol = _trackedSymbolFactory.Create(contract, strikes, currentPrice);
                //return trackedSymbol;

                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public List<ContractDetailsMessage> ContractDetails(Contract contract)
        {
            return _messageCollector.reqContractDetails(contract);
        }

        private HashSet<double> GetStrikes(Contract contract, string lastTradeDateOrContractMonth)
        {
            var contractDetailsMessageList = ContractDetails(contract);
            if (contractDetailsMessageList.Count() > 1)
            {
                throw new ApplicationException("Unexpected. Multiple ContractDetailsMessageList");
            }
            var contractDetailsMessage = contractDetailsMessageList.First();

            var secDefOptParamMessageList = _messageCollector.reqSecDefOptParams(
                contractDetailsMessage.ContractDetails.Contract.Symbol,
                contractDetailsMessage.ContractDetails.Contract.Exchange,
                contractDetailsMessage.ContractDetails.Contract.SecType,
                contractDetailsMessage.ContractDetails.Contract.ConId
                );

            var secDefOptParamMessageExpirationList = secDefOptParamMessageList
                .Where(s => s.Expirations.Contains(lastTradeDateOrContractMonth));

            if (secDefOptParamMessageExpirationList.Count() > 1)
            {
                throw new ApplicationException("Unexpected. Multiple secDefOptParamMessageExpirationList");
            }
            var secDefOptParamMessage = secDefOptParamMessageExpirationList.First();
            return secDefOptParamMessage.Strikes;
        }

        private Contract GetUnderlyingContract(Contract contract, string lastTradeDateOrContractMonth)
        {
            var contractDetailsMessageList = ContractDetails(contract);
            if (!contractDetailsMessageList.Any())
            {
                return null;
            }
            if (contractDetailsMessageList.Count() > 1 && string.IsNullOrWhiteSpace(lastTradeDateOrContractMonth))
            {
                throw new ApplicationException("Unexpected. Multiple ContractDetailsMessages");
            }
            var contractDetailsMessage = contractDetailsMessageList.Count() > 1
                ? contractDetailsMessageList.First(c => c.ContractDetails.RealExpirationDate == lastTradeDateOrContractMonth)
                : contractDetailsMessageList.First();

            if (string.IsNullOrWhiteSpace(contractDetailsMessage.ContractDetails.UnderSecType))
            {
                return null;
            }

            return new Contract
            {
                SecType = contractDetailsMessage.ContractDetails.UnderSecType,
                Symbol = contractDetailsMessage.ContractDetails.Contract.Symbol,
                Currency = contractDetailsMessage.ContractDetails.Contract.Currency,
                Exchange = contractDetailsMessage.ContractDetails.Contract.Exchange,
                LastTradeDateOrContractMonth = contractDetailsMessage.ContractDetails.Contract.LastTradeDateOrContractMonth

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
