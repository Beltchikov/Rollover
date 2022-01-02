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
            //var contractDetailsMessageList = ContractDetails(contract);
            //if (contractDetailsMessageList.Count() > 1)
            //{
            //    throw new ApplicationException("Unexpected. Multiple ContractDetailsMessages");
            //}
            //var contractDetailsMessage = contractDetailsMessageList.First();

            //var trackedSymbol = _trackedSymbolFactory.InitFromContractDetailsMessage(contractDetailsMessage);


            //var strikes = GetStrikes(contractDetailsMessage);
            ////var contractUnderlying = GetUnderlying(contract);
            ////var tickSizePriceTuple = _messageCollector.reqMktData(contractUnderlying, "", true, false, null);

            //return trackedSymbol;

            var underLyingContract = GetUnderlyingContract(contract, null);
            HashSet<double> strikes = null;
            if (underLyingContract != null)
            {
                var secondUnderLyingContract = GetUnderlyingContract(underLyingContract, contract.LastTradeDateOrContractMonth);
                strikes = GetStrikes(secondUnderLyingContract, contract.LastTradeDateOrContractMonth);
            }

            var trackedSymbol = _trackedSymbolFactory.InitFromContract(contract);
            return trackedSymbol;
        }

        public List<ContractDetailsMessage> ContractDetails(Contract contract)
        {
            return _messageCollector.reqContractDetails(contract);
        }

        public HashSet<double> GetStrikes(Contract contract, string lastTradeDateOrContractMonth)
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



            //return GetStrikes(contractDetailsMessage, secDefOptParamMessageList);


            // TODO
            //return null;
        }

        //public HashSet<double> GetStrikes(ContractDetailsMessage contractDetailsMessage)
        //{
        //    // UnderContractDetailsMessage
        //    var underContract = GetUnderlyingContract(contractDetailsMessage);
        //    var underContractDetailsMessageList = ContractDetails(underContract);
        //    if (!underContractDetailsMessageList.Any())
        //    {
        //        throw new ApplicationException("No UnderContractDetailsMessages");
        //    }
        //    if (underContractDetailsMessageList.Count() == 1)
        //    {
        //        // TODO case stocks

        //        //var message = underContractDetailsMessageList.First();
        //        //if (message.ContractDetails.Contract.SecType == "STK")
        //        //{
        //        //    // SecDefOptParamMessage
        //        //    var secDefOptParamMessageList = _messageCollector.reqSecDefOptParams(
        //        //        message.ContractDetails.Contract.Symbol,
        //        //        message.ContractDetails.Contract.Exchange,
        //        //        message.ContractDetails.Contract.SecType,
        //        //        message.ContractDetails.Contract.ConId
        //        //        );
        //        //    return GetStrikes(contractDetailsMessage, secDefOptParamMessageList);
        //        //}
        //    }
        //    var underContractDetailsMessage = underContractDetailsMessageList
        //        .First(c => c.ContractDetails.ContractMonth == contractDetailsMessage.ContractDetails.ContractMonth);

        //    // SecondUnderContractDetailsMessage
        //    var secondUnderContract = GetUnderlyingContract(underContractDetailsMessage);
        //    var secondUnderContractDetailsMessageList = ContractDetails(secondUnderContract);
        //    if (!secondUnderContractDetailsMessageList.Any())
        //    {
        //        throw new ApplicationException("No SecondUnderContractDetailsMessageList");
        //    }
        //    if (secondUnderContractDetailsMessageList.Count() > 1)
        //    {
        //        throw new ApplicationException("Unexpected. Multiple secondUnderContractDetailsMessage");
        //    }
        //    var secondUnderContractDetailsMessage = secondUnderContractDetailsMessageList.First();

        //    // SecDefOptParamMessage
        //    var secDefOptParamMessageList2 = _messageCollector.reqSecDefOptParams(
        //        secondUnderContractDetailsMessage.ContractDetails.Contract.Symbol,
        //        secondUnderContractDetailsMessage.ContractDetails.Contract.Exchange,
        //        secondUnderContractDetailsMessage.ContractDetails.Contract.SecType,
        //        secondUnderContractDetailsMessage.ContractDetails.Contract.ConId
        //        );
        //    return GetStrikes(contractDetailsMessage, secDefOptParamMessageList2);
        //}

        public HashSet<double> GetStrikes(ContractDetailsMessage contractDetailsMessage, List<SecurityDefinitionOptionParameterMessage> secDefOptParamMessageList)
        {
            var lastTradeDateOrContractMonth = contractDetailsMessage.ContractDetails.Contract.LastTradeDateOrContractMonth;
            var secDefOptParamMessageExpirationList = secDefOptParamMessageList
                .Where(s => s.Expirations.Contains(lastTradeDateOrContractMonth));

            if (secDefOptParamMessageExpirationList.Count() > 1)
            {
                throw new ApplicationException("Unexpected. Multiple secDefOptParamMessageExpirationList");
            }
            var secDefOptParamMessage = secDefOptParamMessageExpirationList.First();

            return secDefOptParamMessage.Strikes;
        }

        private static Contract GetUnderlyingContract(ContractDetailsMessage contractDetailsMessage)
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

        public Contract GetUnderlyingContract(Contract contract, string lastTradeDateOrContractMonth)
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

            return GetUnderlyingContract(contractDetailsMessage);
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
