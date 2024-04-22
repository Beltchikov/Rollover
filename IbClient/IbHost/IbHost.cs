using IBApi;
using IbClient.messages;
using IbClient.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TickType = IbClient.Types.TickType;

namespace IbClient.IbHost
{
    public class IbHost : IIbHost
    {
        IIBClient _ibClient;
        private IIbHostQueue _queue;
        private int _currentReqId = 0;
        private IResponses _responses;
        private List<ErrorMessage> _errorMessages;
        private int _nextOrderId;
        private int _lastOrderId;
        public static readonly string DEFAULT_SEC_TYPE = "STK";
        public static readonly string DEFAULT_CURRENCY = "USD";
        public static readonly string DEFAULT_EXCHANGE = "SMART";
        private readonly int ERROR_CODE_10168 = 10168;
        private readonly int ERROR_CODE_354 = 354;
        private readonly int ERROR_CODE_201 = 201;

        public IbHost(IIbHostQueue queue)
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ConnectionClosed += _ibClient_ConnectionClosed;
            _ibClient.ContractDetails += _ibClient_ContractDetails;
            _ibClient.SymbolSamples += _ibClient_SymbolSamples;
            _ibClient.FundamentalData += _ibClient_FundamentalData;
            _ibClient.TickPrice += _ibClient_TickPrice;
            _ibClient.TickSnapshotEnd += _ibClient_TickSnapshotEnd;
            _ibClient.OpenOrder += _ibClient_OpenOrder;

            _queue = queue;
            _responses = new Responses();
            _errorMessages = new List<ErrorMessage>();

            //_ibClient.HistoricalData += _ibClient_HistoricalData;
            //_ibClient.HistoricalDataUpdate += _ibClient_HistoricalDataUpdate;
            //_ibClient.HistoricalDataEnd += _ibClient_HistoricalDataEnd;
            //_ibClient.Position += _ibClient_Position;
            //_ibClient.PositionEnd += _ibClient_PositionEnd;
            //_ibClient.ContractDetails += _ibClient_ContractDetails;
            //_ibClient.ContractDetailsEnd += _ibClient_ContractDetailsEnd;
            //_ibClient.TickPrice += _ibClient_TickPrice;
            //_ibClient.TickSize += _ibClient_TickSize;

            //_ibClient.TickString += _ibClient_TickString;
            //_ibClient.TickOptionCommunication += _ibClient_TickOptionCommunication;
        }

        public IIbConsumer Consumer { get; set; }

        public async Task<bool> ConnectAndStartReaderThread(string host, int port, int clientId, int timeout)
        {
            return await Task.Run(() =>
            {
                if (Consumer == null)
                {
                    throw new ApplicationException("Unexpected! Consumer is null");
                }

                _ibClient.ConnectAndStartReaderThread(host, port, clientId);

                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !Consumer.ConnectedToTws) { }
                return Consumer.ConnectedToTws;
            });
        }

        public void Disconnect()
        {
            _ibClient.Disconnect();
        }

        public async Task<ContractDetails> RequestContractDetailsAsync(Contract contract, int timeout)
        {
            ContractDetails contractDetails = null;
            ContractDetailsMessage contractDetailsMessage = null;
            var reqId = ++_currentReqId;
            _ibClient.ClientSocket.reqContractDetails(reqId, contract);

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout
                    && !_queue.DequeueMessage(reqId, out contractDetailsMessage)) { }

                if (contractDetailsMessage != null)
                {
                    contractDetails = contractDetailsMessage.ContractDetails;
                }
            });

            return contractDetails;
        }

        public async Task<SymbolSamplesMessage> RequestMatchingSymbolsAsync(string symbol, int timeout)
        {
            SymbolSamplesMessage symbolSamplesMessage = null;
            var reqId = ++_currentReqId;
            _ibClient.ClientSocket.reqMatchingSymbols(reqId, symbol);

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while (!_queue.DequeueMessage(reqId, out symbolSamplesMessage)
                    && (DateTime.Now - startTime).TotalMilliseconds < timeout) { }

                return symbolSamplesMessage;
            });

            return symbolSamplesMessage;
        }

        public async Task<string> RequestFundamentalDataAsync(Contract contract, string reportType, int timeout)
        {
            FundamentalsMessage fundamentalsMessage = null;
            string fundamentalsMessageString = null;

            var reqId = ++_currentReqId;
            _ibClient.ClientSocket.reqFundamentalData(reqId, contract, reportType, new List<TagValue>());

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout
                    && !_queue.DequeueMessage(out fundamentalsMessage)) { }

                if (fundamentalsMessage != null)
                {
                    fundamentalsMessageString = fundamentalsMessage.Data;
                    if (!MessageForRightTickerContains(fundamentalsMessageString, contract.Symbol))
                    {
                        fundamentalsMessageString = contract.Symbol + " ERROR!";
                    }
                }
            });

            return fundamentalsMessageString;
        }

        // full list of tick types: https://interactivebrokers.github.io/tws-api/tick_types.html</param>
        public async Task<(double?, TickType?, MarketDataType?)> RequestMarketDataSnapshotAsync(
            Contract contract,
            MarketDataType[] marketDataTypes,
            int timeout)
        {
            double? price = null;
            TickType? tickType = null;
            MarketDataType? marketDataType = marketDataTypes[0];
            await Task.Run(() =>
            {
                while (price == null)
                {
                    _ibClient.ClientSocket.reqMarketDataType(((int)marketDataType));

                    var reqId = ++_currentReqId;
                    _ibClient.ClientSocket.reqMktData(
                        reqId,
                        contract,
                        "",
                        true,
                        false,
                        new List<TagValue>());

                    while (!(_responses.TryGetValidPrice(reqId, m => m.Price > 0, out price, out tickType)
                        || _responses.SnaphotEnded(reqId)
                        || HasErrorMessage(reqId, ERROR_CODE_10168, out _)
                        || HasErrorMessage(reqId, ERROR_CODE_354, out _)
                        || HasErrorMessage(reqId, ERROR_CODE_201, out _))) { };


                    // TickSnapshotEndMessage
                    // TickPriceMessage

                    //while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !DequeueMessage<OpenOrderMessage>(_ibClient.NextOrderId, out openOrderMessage)
                    //&& !DequeueMessage<ErrorMessage>(_ibClient.NextOrderId, out errorMessage)) { }

                    if (price == null)
                    {
                        marketDataType = marketDataTypes[1];
                    }
                }
            });

            return (price, tickType, marketDataType);
        }

        // full list of tick types: https://interactivebrokers.github.io/tws-api/tick_types.html</param>
        public async Task<double?> RequestMarketDataSnapshotAsync(Contract contract, TickType tickType, int timeout)
        {
            double? price = null;
            TickType? tickTypeReceived = null;
            await Task.Run(() =>
            {
                while (price == null)
                {
                    var reqId = ++_currentReqId;
                    _ibClient.ClientSocket.reqMktData(
                        reqId,
                        contract,
                        "",
                        true,
                        false,
                        new List<TagValue>());

                    while (!(_responses.TryGetValidPrice(reqId, m => m.Field == (int)tickType, out price, out tickTypeReceived)
                        || _responses.SnaphotEnded(reqId)
                        || HasErrorMessage(reqId, ERROR_CODE_10168, out _)
                        || HasErrorMessage(reqId, ERROR_CODE_354, out _)
                        || HasErrorMessage(reqId, ERROR_CODE_201, out _)))
                    { };
                }
            });

            return price;
        }


        public async Task<OrderStateOrError> WhatIfOrderStateFromContract(Contract contract, int qty, int timeout)
        {
            _ibClient.ClientSocket.reqIds(-1);

            _lastOrderId = _nextOrderId;
            await Task.Run(() =>
            {
                while (_lastOrderId == _nextOrderId) { _nextOrderId = _ibClient.NextOrderId; }
            });

            Order order = new Order()
            {
                OrderId = _nextOrderId,
                Action = "BUY",
                OrderType = "MARKET",
                TotalQuantity = qty,
                WhatIf = true
            };

            _ibClient.ClientSocket.placeOrder(_ibClient.NextOrderId, contract, order);

            OpenOrderMessage openOrderMessage = null;
            ErrorMessage errorMessage = null;
            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !_queue.DequeueMessage<OpenOrderMessage>(_ibClient.NextOrderId, out openOrderMessage)
                    && !_queue.DequeueMessage<ErrorMessage>(_ibClient.NextOrderId, out errorMessage)) { }
            });

            if (openOrderMessage == null)
            {
                if (errorMessage == null)
                {
                    return new OrderStateOrError(null, $"Timeout exceeded.");
                }
                else
                {
                    return new OrderStateOrError(null, $"ReqId:{errorMessage.RequestId} Code:{errorMessage.ErrorCode} {errorMessage.Message}");
                }
            }
            else
            {
                if (errorMessage != null)
                {
                    throw new ApplicationException("Unexpected! Both OrderState and errorMessage are not null.");
                }
                return new OrderStateOrError(openOrderMessage.OrderState, "");
            }
        }

        public async Task<double?> RateOfExchange(string currency, int timeout)
        {
            double? result;
            if (currency.ToUpper() == "USD")
            {
                result = 1;
            }
            else
            {
                (Contract currencyPairContract, bool usdIsInDenominator) = CurrencyPair.ContractFromCurrency(currency);
                MarketDataType[] marketDataTypes = new[] { MarketDataType.Live, MarketDataType.DelayedFrozen };
                (var currentPrice, _, _) = await RequestMarketDataSnapshotAsync(currencyPairContract, marketDataTypes, timeout);
                result = usdIsInDenominator ? currentPrice : Math.Round(1 / currentPrice.Value, 5);
            }
            return result;
        }

        private void _ibClient_Error(int reqId, int code, string message, Exception exception)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"ReqId:{reqId} code:{code} message:{message} exception:{exception}");

            if (code == ERROR_CODE_10168)
            {
                _errorMessages.Add(new ErrorMessage(reqId, code, message));
            }
            if (code == ERROR_CODE_354)
            {
                _errorMessages.Add(new ErrorMessage(reqId, code, message));
            }
            if (code == ERROR_CODE_201)
            {
                _errorMessages.Add(new ErrorMessage(reqId, code, message));
            }
        }

        private void _ibClient_ManagedAccounts(ManagedAccountsMessage message)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"ReqId:0 managed accounts: {message.ManagedAccounts.Aggregate((r, n) => r + "," + n)}");
        }

        private void _ibClient_NextValidId(ConnectionStatusMessage message)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add(message.IsConnected ? "CONNECTED!" : "NOT CONNECTED!");
            Consumer.ConnectedToTws = message.IsConnected;
        }

        private void _ibClient_ConnectionClosed()
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add("DISCONNECTED!");
            Consumer.ConnectedToTws = false;
        }

        private void _ibClient_ContractDetails(ContractDetailsMessage contractDetailsMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"ContractDetailsMessage for {contractDetailsMessage.ContractDetails.Contract.LocalSymbol} " +
                $"conId:{contractDetailsMessage.ContractDetails.Contract.ConId}");
            _queue.Enqueue(contractDetailsMessage);
        }

        private void _ibClient_SymbolSamples(SymbolSamplesMessage symbolSamplesMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"SymbolSamplesMessage for {symbolSamplesMessage.ReqId}");
            _queue.Enqueue(symbolSamplesMessage);
        }

        private void _ibClient_FundamentalData(FundamentalsMessage fundamentalsMessage)
        {
            _queue.Enqueue(fundamentalsMessage);
        }

        private void _ibClient_TickPrice(TickPriceMessage tickPriceMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"TickPriceMessage for {tickPriceMessage.RequestId} " +
                $"field:{tickPriceMessage.Field} price:{tickPriceMessage.Price}");
            _queue.Enqueue(tickPriceMessage);
        }

        private void _ibClient_TickSnapshotEnd(int reqId)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"TickSnapshotEnd for {reqId} ");
            _queue.Enqueue(new TickSnapshotEndMessage(reqId));
        }

        private void _ibClient_OpenOrder(OpenOrderMessage openOrderMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"OpenOrderMessage for {openOrderMessage.Contract.LocalSymbol} " +
                $"conId:{openOrderMessage.Contract.ConId}");
            _queue.Enqueue(openOrderMessage);
        }
        private bool MessageForRightTickerContains(string fundamentalsMessageString, string ticker)
        {
            if (ticker == "ALD1")
            {
                return fundamentalsMessageString.Contains("ALD");
            }

            return fundamentalsMessageString.Contains(ticker) || fundamentalsMessageString.Contains(ticker.ToUpper());
        }

        private bool HasErrorMessage(int reqId, int errorCode, out ErrorMessage errorMessage)
        {
            bool searchFunction(ErrorMessage c) => c.RequestId == reqId && c.ErrorCode == errorCode;
            return HasErrorMessage(reqId, searchFunction, out errorMessage);
        }


        private bool HasErrorMessage(int reqId, Func<ErrorMessage, bool> errorFilterFunction, out ErrorMessage errorMessage)
        {
            var errorMessagesCopy = _errorMessages.ToArray();
            bool result = errorMessagesCopy.Any(errorFilterFunction);
            errorMessage = result
                ? errorMessagesCopy.Single(errorFilterFunction)
                : null;
            return result;
        }
    }
}
