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
            switch (contract.SecType)
            {
                case "FOP":
                    return GetTrackedSymbolFop(contract);
                case "OPT":
                    return GetTrackedSymbolOpt(contract);
                default:
                    throw new NotImplementedException();
            }
        }

        public ITrackedSymbol GetTrackedSymbolFop(Contract contract)
        {
            // Underlying
            var contractDetails = ContractDetails(contract);
            var underlyingContracts = GetUnderlyingContracts(contractDetails);
            if (underlyingContracts.Count() > 1)
            {
                throw new ApplicationException($"Multiple underlyings for the contract {contract}");
            }
            var underLyingContract = underlyingContracts.First();

            // Second underlying
            var secondContractDetails = ContractDetails(underLyingContract)
                .First(d => d.ContractDetails.Contract.LastTradeDateOrContractMonth == contract.LastTradeDateOrContractMonth);
            var secondUnderlyingContracts = GetUnderlyingContracts(contractDetails);
            if (secondUnderlyingContracts.Count() > 1)
            {
                throw new ApplicationException($"Multiple underlyings for the contract {underLyingContract}");
            }
            var secondUnderLyingContract = secondUnderlyingContracts.First();

            // Strikes & price
            var strikes = GetStrikes(secondUnderLyingContract, contract.LastTradeDateOrContractMonth);
            var currentPrice = GetCurrentPrice(underLyingContract);
            if (!currentPrice.Item1)
            {
                return null;
            }

            return _trackedSymbolFactory.Create(contract, strikes, currentPrice.Item2);
        }

        public ITrackedSymbol GetTrackedSymbolOpt(Contract contract)
        {
            throw new NotImplementedException();

            // else if (underLyingContract.SecType == "STK")
            //{
            //    strikes = GetStrikes(underLyingContract, contract.LastTradeDateOrContractMonth);
            //    var currentPrice = GetCurrentPrice(underLyingContract);
            //    if (!currentPrice.Item1)
            //    {
            //        return null;
            //    }

            //    return _trackedSymbolFactory.Create(contract, strikes, currentPrice.Item2);
            //}
            //else if (underLyingContract.SecType == "IND")
            //{
            //    // TODO
            //    throw new NotImplementedException();
            //}
            //else
            //{
            //    throw new NotImplementedException();
            //}
        }

        private Tuple<bool, double> GetCurrentPrice(Contract underLyingContract)
        {
            var tickPriceMessage = _messageCollector.reqMktData(underLyingContract, "", true, false, null);
            if (tickPriceMessage == null)
            {
                return new Tuple<bool, double>(false, -1);
            }
            return new Tuple<bool, double>(true, tickPriceMessage.Price);
        }

        public List<ContractDetailsMessage> ContractDetails(Contract contract)
        {
            return _messageCollector.reqContractDetails(contract);
        }

        public List<SecurityDefinitionOptionParameterMessage> OptionParameters(
            string symbol,
            string exchange,
            string secType,
            int conId)
        {
            var secDefOptParamMessageList = OptionParametersTryOnce(symbol, exchange, secType, conId);
            if (secDefOptParamMessageList.Any())
            {
                return secDefOptParamMessageList;
            }

            exchange = "";
            return OptionParametersTryOnce(symbol, exchange, secType, conId);
        }

        private List<SecurityDefinitionOptionParameterMessage> OptionParametersTryOnce(
            string symbol,
            string exchange,
            string secType,
            int conId)
        {
            return _messageCollector.reqSecDefOptParams(symbol, exchange, secType, conId);
        }

        private HashSet<double> GetStrikes(Contract contract, string lastTradeDateOrContractMonth)
        {
            var contractDetailsMessageList = ContractDetails(contract);
            if (contractDetailsMessageList.Count() > 1)
            {
                throw new ApplicationException("Unexpected. Multiple ContractDetailsMessageList");
            }
            var contractDetailsMessage = contractDetailsMessageList.First();

            var secDefOptParamMessageList = OptionParameters(
                contractDetailsMessage.ContractDetails.Contract.Symbol,
                contractDetailsMessage.ContractDetails.Contract.Exchange,
                contractDetailsMessage.ContractDetails.Contract.SecType,
                contractDetailsMessage.ContractDetails.Contract.ConId
                )
                .Where(s => s.Expirations.Contains(lastTradeDateOrContractMonth));

            if (!secDefOptParamMessageList.Any())
            {
                return new HashSet<double>();
            }
            if (secDefOptParamMessageList.Count() > 1)
            {
                secDefOptParamMessageList = secDefOptParamMessageList.Where(e => e.Exchange == Constants.SMART).ToList();
                if (secDefOptParamMessageList.Count() > 1)
                {
                    throw new ApplicationException("Unexpected. Multiple secDefOptParamMessageExpirationList");
                }
            }
            var secDefOptParamMessage = secDefOptParamMessageList.First();
            return secDefOptParamMessage.Strikes;
        }

        private List<Contract> GetUnderlyingContracts(List<ContractDetailsMessage> contractDetails)
        {
            return contractDetails.Select(d => new Contract
            {
                SecType = d.ContractDetails.UnderSecType,
                Symbol = d.ContractDetails.Contract.Symbol,
                Currency = d.ContractDetails.Contract.Currency,
                Exchange = d.ContractDetails.Contract.Exchange,
                LastTradeDateOrContractMonth = d.ContractDetails.Contract.LastTradeDateOrContractMonth
            }).ToList();
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
