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

        public Repository(
            IIbClientWrapper ibClient,
            IMessageProcessor messageProcessor,
            IMessageCollector messageCollector)
        {
            _ibClient = ibClient;
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

        public TrackedSymbol GetTrackedSymbol(Contract contract)
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

        public TrackedSymbol GetTrackedSymbolFop(Contract contract)
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
            var currentPrice = LastPrice(underLyingContract.ConId, underLyingContract.Exchange);
            if (!currentPrice.Item1)
            {
                return null;
            }

            // TODO
            //int buyConId = GetBuyConIdForBearSpread(contract, strikes, currentPrice.Item2);

            return new TrackedSymbol(contract.LocalSymbol, contract.ConId, contract.Exchange);
        }

        //private int GetBuyConIdForBearSpread(Contract callContract, HashSet<double> strikes, double price)
        //{
        //    var putContract = new Contract
        //    {
        //        Symbol = callContract.Symbol,
        //        Currency = callContract.Currency,
        //        SecType = callContract.SecType,
        //        Exchange = callContract.Exchange,
        //        LastTradeDateOrContractMonth = callContract.LastTradeDateOrContractMonth,
        //        Strike = _trackedSymbolFactory.PreviousStrike(strikes, price),
        //        Right = "P"
        //    };

        //    var contractDetails = ContractDetails(putContract);
        //    if (contractDetails.Count() > 1)
        //    {
        //        throw new ApplicationException($"Multiple contract details for the contract {putContract}");
        //    }
        //    return contractDetails.First().ContractDetails.Contract.ConId;
        //}

        private int GetBuyConIdForBearSpread(ITrackedSymbol trackedSymbol)
        {
            var priceUnderlying = LastPrice(trackedSymbol.ConId, trackedSymbol.Exchange);
            var putContract = new Contract
            {
                Symbol = trackedSymbol.Symbol(this),
                Currency = trackedSymbol.Currency(this),
                SecType = trackedSymbol.SecType(this),
                Exchange = trackedSymbol.Exchange,
                LastTradeDateOrContractMonth = trackedSymbol.LastTradeDateOrContractMonth(this),
                Strike = trackedSymbol.PreviousStrike(this, priceUnderlying.Item2),
                Right = "P"
            };

            var contractDetails = ContractDetails(putContract);
            if (contractDetails.Count() > 1)
            {
                throw new MultipleContractDetailsMessage($"Multiple contract details for the contract {putContract}");
            }
            return contractDetails.First().ContractDetails.Contract.ConId;
        }

        public TrackedSymbol GetTrackedSymbolOpt(Contract contract)
        {
            // Underlying
            var contractDetails = ContractDetails(contract);
            var underlyingContracts = GetUnderlyingContracts(contractDetails);
            if (underlyingContracts.Count() > 1)
            {
                throw new ApplicationException($"Multiple underlyings for the contract {contract}");
            }
            var underLyingContract = underlyingContracts.First();

            // Strikes & price
            var strikes = GetStrikes(underLyingContract, contract.LastTradeDateOrContractMonth);
            var currentPrice = LastPrice(underLyingContract.ConId, underLyingContract.Exchange);
            if (!currentPrice.Item1)
            {
                return null;
            }

            // TODO
            //int buyConId = GetBuyConIdForBearSpread(contract, strikes, currentPrice.Item2);

            return new TrackedSymbol(contract.LocalSymbol, contract.ConId, contract.Exchange);
        }

        public Tuple<bool, double> LastPrice(int conId, string exchange)
        {
            var contract = new Contract { ConId = conId, Exchange = exchange };
            var mktDataTuple = _messageCollector.reqMktData(contract, "", true, false, null);

            //    // Last Price	4	Last price at which the contract traded (does not include some trades in RTVolume).
            //    // https://interactivebrokers.github.io/tws-api/tick_types.html
            var tickPriceMessage = mktDataTuple.Item2.FirstOrDefault(m => m.Field == 4);
            if (tickPriceMessage == null)
            {
                return new Tuple<bool, double>(false, 0);
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

        public HashSet<double> GetStrikes(Contract contract, string lastTradeDateOrContractMonth)
        {
            var contractDetailsMessageList = ContractDetails(contract);
            if (contractDetailsMessageList.Count() > 1)
            {
                throw new MultipleContractDetailsMessage("Unexpected. Multiple ContractDetailsMessageList");
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

            // Evtl exchange SMART should be used to get a single secDefOptParamMessage
            return secDefOptParamMessageList.First().Strikes;
        }

        private List<Contract> GetUnderlyingContracts(List<ContractDetailsMessage> contractDetails)
        {
            return contractDetails.Select(d => new Contract
            {
                ConId = d.ContractDetails.UnderConId,
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

        public void PlaceBearSpread(ITrackedSymbol trackedSymbol)
        {
            Contract contract = new Contract
            {
                ConId = trackedSymbol.ConId,
                Exchange = trackedSymbol.Exchange
            };

            var contractDetailsMessageList = ContractDetails(contract);
            if (contractDetailsMessageList.Count() > 1)
            {
                throw new MultipleContractDetailsMessage("Unexpected. Multiple ContractDetailsMessageList");
            }
            var contractDetailsMessage = contractDetailsMessageList.First();

            Contract bagContract = new Contract
            {
                Symbol = contractDetailsMessage.ContractDetails.Contract.Symbol,
                SecType = contractDetailsMessage.ContractDetails.Contract.SecType,
                Exchange = trackedSymbol.Exchange,
                Currency = contractDetailsMessage.ContractDetails.Contract.Currency
            };
            var sellLeg = new ComboLeg()
            {
                Action = "SELL",
                ConId = trackedSymbol.ConId,
                Ratio = 1,
                Exchange = trackedSymbol.Exchange
            };
            var buyLegConId = GetBuyConIdForBearSpread(trackedSymbol);
            var buyLeg = new ComboLeg()
            {
                Action = "BUY",
                ConId = buyLegConId,
                Ratio = 1,
                Exchange = trackedSymbol.Exchange
            };

            // TODO in  MessageCollector
            //    ibClient.ClientSocket.placeOrder(_nextOrderId, contract, order);
            //    ibClient.ClientSocket.reqIds(-1);

            //****************************************

            //throw new NotImplementedException();

            //    var exchange = string.IsNullOrWhiteSpace(txtExchageComboOrder.Text)
            //? null
            //: txtExchageComboOrder.Text;

            //    Contract contract = new Contract
            //    {
            //        Symbol = txtSymbolComboOrder.Text,
            //        SecType = txtSecTypeComboOrder.Text,
            //        Exchange = txtExchageComboOrder.Text,
            //        Currency = txtCurrencyComboBox.Text
            //    };

            //    // Add legs
            //    var sellLeg = new ComboLeg()
            //    {
            //        Action = "SELL",
            //        ConId = Convert.ToInt32(txtSellLegConId.Text),
            //        Ratio = 1,
            //        Exchange = exchange
            //    };
            //    var buyLeg = new ComboLeg()
            //    {
            //        Action = "BUY",
            //        ConId = Convert.ToInt32(txtBuyLegConId.Text),
            //        Ratio = 1,
            //        Exchange = exchange
            //    };
            //    contract.ComboLegs = new List<ComboLeg>();
            //    contract.ComboLegs.AddRange(new List<ComboLeg> { sellLeg, buyLeg });

            //    Order order = new Order
            //    {
            //        Action = txtActionComboBox.Text,
            //        OrderType = "LMT",
            //        TotalQuantity = Convert.ToInt32(txtQuantityComboOrder.Text),
            //        LmtPrice = Double.Parse(txtLimitPriceComboOrder.Text)
            //    };

            //    ibClient.ClientSocket.placeOrder(_nextOrderId, contract, order);
            //    ibClient.ClientSocket.reqIds(-1);
        }

        public bool IsConnected()
        {
            return _messageCollector.IsConnected();
        }
    }
}
