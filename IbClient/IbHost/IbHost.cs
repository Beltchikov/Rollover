using IBApi;
using IbClient.Events;
using IbClient.messages;
using IbClient.Types;
using IBSampleApp.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickType = IbClient.Types.TickType;

namespace IbClient.IbHost
{
    public class IbHost : IIbHost
    {
        IIBClient _ibClient;
        private IIbHostQueue _queueCommon;
        private IIbHostQueue _queueConnectionStatus;
        private IIbHostQueue _queueManagedAccounts;
        private IIbHostQueue _queueError;
        private IIbHostQueue _queueAccountSummary;
        private IIbHostQueue _queueAccountSummaryEnd;
        private IIbHostQueue _queuePositions;
        private IIbHostQueue _queuePositionsEnd;
        private IIbHostQueue _queueTickPriceMessage;
        private IIbHostQueue _queueMktDataErrors;
        private IIbHostQueue _queuePlaceOrderErrors;
        private IIbHostQueue _queueOrderOpenMessage;
        private IIbHostQueue _queueHistoricalData;
        private IIbHostQueue _queueHistoricalDataUpdate;
        private IIbHostQueue _queueHistoricalDataEnd;

        private IRequestResponseMapper _requestResponseMapper;
        private IRequestResponseMapper _orderResponseMapper;

        private int _currentReqId = 0;
        List<int> _mktDataReqIds;
        List<int> _placeOrderOrderIds;
        private List<ErrorMessage> _errorMessages;
        private int _nextOrderId;
        private int _lastOrderId;
        private Dictionary<int, Contract> _requestIdContract;
        private bool _mktDataCancelled;
        public static readonly string DEFAULT_SEC_TYPE = "STK";
        public static readonly string DEFAULT_CURRENCY = "USD";
        public static readonly string DEFAULT_EXCHANGE = "SMART";
        private readonly string PRESUBMITTED = "PreSubmitted";
        private readonly string EXCHANGE_SMART = "SMART";

        public IbHost()
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ConnectionClosed += _ibClient_ConnectionClosed;
            _ibClient.AccountSummary += _ibClient_AccountSummary;
            _ibClient.AccountSummaryEnd += _ibClient_AccountSummaryEnd;
            _ibClient.Position += _ibClient_Position;
            _ibClient.PositionEnd += _ibClient_PositionEnd;
            _ibClient.ContractDetails += _ibClient_ContractDetails;
            _ibClient.SymbolSamples += _ibClient_SymbolSamples;
            _ibClient.FundamentalData += _ibClient_FundamentalData;
            _ibClient.TickPrice += _ibClient_TickPrice;
            _ibClient.TickSize += _ibClient_TickSize;
            _ibClient.TickSnapshotEnd += _ibClient_TickSnapshotEnd;
            _ibClient.OpenOrder += _ibClient_OpenOrder;
            _ibClient.OrderStatus += _ibClient_OrderStatus;
            _ibClient.HistoricalData += _ibClient_HistoricalData;
            _ibClient.HistoricalDataUpdate += _ibClient_HistoricalDataUpdate;
            _ibClient.HistoricalDataEnd += _ibClient_HistoricalDataEnd;

            _queueCommon = new IbHostQueue();
            _queueConnectionStatus = new IbHostQueue();
            _queueManagedAccounts = new IbHostQueue();
            _queueError = new IbHostQueue();
            _queueAccountSummary = new IbHostQueue();
            _queueAccountSummaryEnd = new IbHostQueue();
            _queuePositions = new IbHostQueue();
            _queuePositionsEnd = new IbHostQueue();
            _queueTickPriceMessage = new IbHostQueue();
            _queueTickPriceMessage = new IbHostQueue();
            _queueMktDataErrors = new IbHostQueue();
            _queuePlaceOrderErrors = new IbHostQueue();
            _queueOrderOpenMessage = new IbHostQueue();
            _queueHistoricalData = new IbHostQueue();
            _queueHistoricalDataUpdate = new IbHostQueue();
            _queueHistoricalDataEnd = new IbHostQueue();

            _requestResponseMapper = new RequestResponseMapper();
            _orderResponseMapper = new RequestResponseMapper();

            _errorMessages = new List<ErrorMessage>();
            _mktDataReqIds = new List<int> { };
            _placeOrderOrderIds = new List<int> { };
            _requestIdContract = new Dictionary<int, Contract>();

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

        public IIbHostQueue QueueHistoricalDataUpdate => _queueHistoricalDataUpdate;

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

        public async Task ConnectAndStartReaderThread(
             string host,
             int port,
             int clientId,
             Action<ConnectionStatusMessage> connectionStatusCallback,
             Action<ManagedAccountsMessage> managedAccountsCallback,
             Action<ErrorMessage> errorMessageCallback)
        {
            await Task.Run(async () =>
            {
                if (Consumer == null)
                {
                    throw new ApplicationException("Unexpected! Consumer is null");
                }

                _ibClient.ConnectAndStartReaderThread(host, port, clientId);

                object endMessage = null;
                await Task.Run(() =>
                {
                    while (!_queueConnectionStatus.TryDequeue(out endMessage)) { }

                });

                object dataMessage = null;
                while (_queueManagedAccounts.TryDequeue(out dataMessage))
                {
                    managedAccountsCallback((ManagedAccountsMessage)dataMessage);
                }

                object errorMessage = null;
                while (_queueError.TryDequeue(out errorMessage))
                {
                    errorMessageCallback((ErrorMessage)errorMessage);
                }

                var endMessageTyped = endMessage as ConnectionStatusMessage;
                connectionStatusCallback(endMessageTyped);

                return endMessageTyped.IsConnected;
            });
        }

        public bool IsConnected => _ibClient.ClientSocket.IsConnected();
        public int NextOrderId => _ibClient.NextOrderId;

        public void Disconnect()
        {
            _ibClient.Disconnect();
        }

        public async Task RequestAccountSummaryAsync(
            string group,
            string tags,
            Action<AccountSummaryMessage> accountSummaryCallback,
            Action<AccountSummaryEndMessage> accountSummaryEndCallback)
        {
            var reqId = ++_currentReqId;
            _ibClient.ClientSocket.reqAccountSummary(reqId, group, tags);

            object endMessage = null;
            await Task.Run(() =>
            {
                while (!_queueAccountSummaryEnd.TryDequeue(out endMessage)) { }

            });

            object dataMessage = null;
            while (_queueAccountSummary.TryDequeue(out dataMessage))
            {
                accountSummaryCallback(dataMessage as AccountSummaryMessage);
            }

            var endMessageTyped = endMessage as AccountSummaryEndMessage;
            accountSummaryEndCallback(endMessageTyped);
        }


        public async Task RequestPositions(Action<PositionMessage> positionCallback, Action positionEndCallback)
        {
            _ibClient.ClientSocket.reqPositions();

            object endMessage = null;
            await Task.Run(() =>
            {
                while (!_queuePositionsEnd.TryDequeue(out endMessage)) { }

            });

            object dataMessage = null;
            while (_queuePositions.TryDequeue(out dataMessage))
            {
                positionCallback(dataMessage as PositionMessage);
            }

            var endMessageTyped = (int)endMessage;
            if (endMessageTyped != 1) throw new Exception("Unexpected. endMessageTyped != 1");
            positionEndCallback();
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
                    && !_queueCommon.DequeueMessage(reqId, out contractDetailsMessage)) { }

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
                while (!_queueCommon.DequeueMessage(reqId, out symbolSamplesMessage)
                    && (DateTime.Now - startTime).TotalMilliseconds < timeout) { }

                return symbolSamplesMessage;
            });

            return symbolSamplesMessage;
        }

        public async Task<int> ReqIdsAsync(int idParam)
        {
            _lastOrderId = _ibClient.NextOrderId;
            _nextOrderId = _ibClient.NextOrderId;

            _ibClient.ClientSocket.reqIds(idParam);

            await Task.Run(() =>
            {
                while (_lastOrderId == _nextOrderId) { _nextOrderId = _ibClient.NextOrderId; }
            });

            return _nextOrderId;
        }

        public void ReqIds(int idParam)
        {
            _ibClient.ClientSocket.reqIds(idParam);
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
                    && !_queueCommon.DequeueMessage(out fundamentalsMessage)) { }

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

        [Obsolete]
        public async Task<(double?, TickType?, string)> RequestMktData(
            Contract contract,
            string genericTickList,
            bool snapshot,
            bool regulatorySnapshot,
            List<TagValue> mktDataOptions,
            int timeout)
        {
            TickPriceMessage tickPriceMessage = null;
            ErrorMessage errorMessage = null;
            var reqId = ++_currentReqId;
            _mktDataReqIds.Add(reqId);

            _ibClient.ClientSocket.reqMktData(
                reqId, contract, genericTickList, snapshot, regulatorySnapshot, mktDataOptions);

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while (LoopMustGoOnMktData(
                    reqId,
                    startTime,
                    timeout,
                    (msg => msg.Price > 0),
                    out tickPriceMessage,
                    out errorMessage)) { }
            });

            var errorText = errorMessage == null
                ? "" : $"Error: reqId:{errorMessage.RequestId} code:{errorMessage.ErrorCode} message:{errorMessage.Message}";
            return (tickPriceMessage?.Price, (TickType?)tickPriceMessage?.Field, errorText);
        }

        public int RequestMktData(
           Contract contract,
           string genericTickList,
           bool snapshot,
           bool regulatorySnapshot,
           List<TagValue> mktDataOptions,
           Action<TickPriceMessage> tickPriceCallback,
           Action<TickSizeMessage> tickSizeCallback,
           Action<int, int, string, string, Exception> errorCallback)
        {
            var reqId = ++_currentReqId;
            _requestResponseMapper.AddRequestId(reqId);

            _ibClient.ClientSocket.reqMktData(
                reqId, contract, genericTickList, snapshot, regulatorySnapshot, mktDataOptions);

            Task.Run(() =>
            {
                while (!_mktDataCancelled)
                {
                    var responses = _requestResponseMapper.GetResponses(reqId);
                    if (responses == null) continue;

                    foreach (var response in responses)
                    {
                        if (!(response is TickPriceMessage)) continue;
                        var tickPriceMessage = response as TickPriceMessage;
                        tickPriceCallback(tickPriceMessage);
                    }
                }
            });

            return reqId;
        }

        public void CancelMktData(int requestId)
        {
            _mktDataCancelled = true;
            _ibClient.ClientSocket.cancelMktData(requestId);
        }

        public async Task RequestHistoricalAndSubscribeAsync(
            Contract contract,
            string endDateTime,
            string durationString,
            string barSizeSetting,
            string whatToShow,
            int useRTH,
            int formatDate,
            List<TagValue> tagValues,
            int timeout,
            Action<HistoricalDataMessage> historicalDataCallback,
            Action<HistoricalDataMessage> historicalDataUpdateCallback,
            Action<HistoricalDataEndMessage> historicalDataEndCallback)
        {
            var contractSmartExchange = new IBApi.Contract()
            {
                SecType = contract.SecType,
                Symbol = contract.Symbol,
                Currency = contract.Currency,
                Exchange = EXCHANGE_SMART
            };

            var reqId = ++_currentReqId;
            _requestIdContract[reqId] = contract;
            _ibClient.ClientSocket.reqHistoricalData(
                reqId,
                contractSmartExchange,
                endDateTime,
                durationString,
                barSizeSetting,
                whatToShow,
                useRTH,
                formatDate,
                true,
                tagValues);


            object endMessage = null;
            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while (!_queueHistoricalDataEnd.TryDequeue(out endMessage)) { }

            });

            object dataMessage = null;
            while (_queueHistoricalData.TryDequeue(out dataMessage))
            {
                historicalDataCallback((HistoricalDataMessage)dataMessage);
            }

            object updateMessage = null;
            while (_queueHistoricalDataUpdate.TryDequeue(out updateMessage))
            {
                // TODO remove the whole block
                //historicalDataUpdateCallback((HistoricalDataMessage)dataMessage);
            }

            historicalDataEndCallback((HistoricalDataEndMessage)endMessage);

        }

        // NO SUBSRIBE - hist data only
        public async Task RequestHistoricalDataAsync(
            Contract contract,
            string endDateTime,
            string durationString,
            string barSizeSetting,
            string whatToShow,
            int useRTH,
            int formatDate,
            List<TagValue> tagValues,
            int timeout,
            Action<HistoricalDataMessage> historicalDataCallback,
            Action<HistoricalDataMessage> historicalDataUpdateCallback,
            Action<HistoricalDataEndMessage> historicalDataEndCallback)
        {
            var contractSmartExchange = new IBApi.Contract()
            {
                SecType = contract.SecType,
                Symbol = contract.Symbol,
                Currency = contract.Currency,
                Exchange = EXCHANGE_SMART
            };

            var reqId = ++_currentReqId;
            _requestIdContract[reqId] = contract;
            _requestResponseMapper.AddRequestId(reqId);

            // TODO use for tests
            //AddTestNoiseData(_requestDictionary);

            _ibClient.ClientSocket.reqHistoricalData(
                reqId,
                contractSmartExchange,
                endDateTime,
                durationString,
                barSizeSetting,
                whatToShow,
                useRTH,
                formatDate,
                false,
                tagValues);

            List<HistoricalDataMessage> historicalDataMessages = null;
            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while (_requestResponseMapper.RemoveResponse<HistoricalDataEndMessage>(reqId) == null
                   && (DateTime.Now - startTime).TotalMilliseconds < timeout) { }

                try
                {
                    historicalDataMessages = _requestResponseMapper.GetResponses(reqId)
                                .Select(m => m as HistoricalDataMessage)
                                .ToList();
                }
                catch (Exception e)
                {

                    historicalDataEndCallback(new HistoricalDataEndMessage(reqId, "TIMEOUT", $"{e}"));
                }
            });

            if (historicalDataMessages != null)
            {
                foreach (var m in historicalDataMessages)
                {
                    historicalDataCallback(m);
                }
            }
        }

        // TODO
        // eventually return simply OrderState
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
                OutsideRth = true,
                WhatIf = true
            };
            OpenOrderMessage openOrderMessage = null;
            ErrorMessage errorMessage = null;

            _placeOrderOrderIds.Add(_nextOrderId);
            _ibClient.ClientSocket.placeOrder(_ibClient.NextOrderId, contract, order);

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while (LoopMustGoOnPlaceOrder(
                    _nextOrderId,
                    startTime,
                    timeout,
                    (msg) =>
                    {
                        if (msg.OrderState == null) return false;
                        if (msg.OrderState.Status == PRESUBMITTED) return true;
                        return false;
                    },
                    out openOrderMessage,
                    out errorMessage)) { }
            });

            _placeOrderOrderIds.Remove(_nextOrderId);
            if (openOrderMessage == null)
            {
                if (errorMessage != null)
                    return new OrderStateOrError(null, $"NextOrderId:{_nextOrderId} ReqId:{errorMessage.RequestId} code:{errorMessage.ErrorCode} message:{errorMessage.Message}");
                else
                    return new OrderStateOrError(null, $"NextOrderId:{_nextOrderId} Undefined error.");
            }
            else
            {
                return new OrderStateOrError(openOrderMessage.OrderState, "");
            }
        }

        public async Task<OrderStateOrError> PlaceOrderAsync(Contract contract, Order order, int timeout)
        {
            _placeOrderOrderIds.Add(order.OrderId);
            _ibClient.ClientSocket.placeOrder(order.OrderId, contract, order);
            OpenOrderMessage openOrderMessage = null;
            ErrorMessage errorMessage = null;

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while (LoopMustGoOnPlaceOrder(
                    _nextOrderId,
                    startTime,
                    timeout,
                    (msg) =>
                    {
                        if (msg.OrderState == null) return false;
                        if (msg.OrderState.Status == PRESUBMITTED) return true;
                        return false;
                    },
                    out openOrderMessage,
                    out errorMessage)) { }
            });

            _placeOrderOrderIds.Remove(order.OrderId);
            if (openOrderMessage == null)
            {
                if (errorMessage != null)
                    return new OrderStateOrError(null, $"NextOrderId:{_nextOrderId} ReqId:{errorMessage.RequestId} code:{errorMessage.ErrorCode} message:{errorMessage.Message}");
                else
                    return new OrderStateOrError(null, $"NextOrderId:{_nextOrderId} Undefined error.");
            }
            else
            {
                return new OrderStateOrError(openOrderMessage.OrderState, "");
            }
        }

        public async Task<double> PlaceOrderAndWaitExecution(Contract contract, Order order)
        {
            _orderResponseMapper.AddRequestId(order.OrderId);
            _ibClient.ClientSocket.placeOrder(order.OrderId, contract, order);

            bool orderExecuted = false;
            double avrFillPrice = 0;
            await Task.Run(() =>
            {
                while (!orderExecuted)
                {
                    var responses = _orderResponseMapper.GetResponses(order.OrderId);
                    var filledOrderStatusMessage = responses
                        .Where(r => r is OrderStatusMessage && (r as OrderStatusMessage).Status.ToUpper() == "FILLED")
                        .Select(m => m as OrderStatusMessage)
                        .FirstOrDefault();

                    if (filledOrderStatusMessage != null)
                    {
                        object lockObject = new object();
                        lock (lockObject)
                        {
                            avrFillPrice = filledOrderStatusMessage.AvgFillPrice;
                            orderExecuted = true;
                        }
                    }
                }
            });

            return avrFillPrice;
        }

        public void ClearQueueOrderOpenMessage()
        {
            _queueOrderOpenMessage.Clear();
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
                //(var currentPrice, _, _) = await RequestMarketDataSnapshotAsync(currencyPairContract, marketDataTypes, timeout);
                (var currentPrice, _, _) = await RequestMktData(currencyPairContract, "", true, false, null, timeout);
                result = usdIsInDenominator ? currentPrice : Math.Round(1 / currentPrice.Value, 5);
            }
            return result;
        }

        // TODO make generic
        private bool LoopMustGoOnMktData(
            int reqId,
            DateTime startTime,
            int timeout,
            Predicate<TickPriceMessage> messageIsValid,
            out TickPriceMessage tickPriceMessage,
            out ErrorMessage errorMessage)
        {
            _queueTickPriceMessage.DequeueAllTickPriceMessagesExcept(reqId);

            if (_queueTickPriceMessage.DequeueMessage(reqId, out tickPriceMessage))
            {
                if (tickPriceMessage != null)
                {
                    if (messageIsValid(tickPriceMessage))
                    {
                        errorMessage = null;
                        return false;
                    }
                    else return !HasErrorOrTimeout(_queueTickPriceMessage, reqId, startTime, timeout, out errorMessage);
                }
                else
                {
                    return !HasErrorOrTimeout(_queueTickPriceMessage, reqId, startTime, timeout, out errorMessage);
                }
            }
            else
            {
                return !HasErrorOrTimeout(_queueTickPriceMessage, reqId, startTime, timeout, out errorMessage);
            }
        }

        // TODO make generic
        private bool LoopMustGoOnPlaceOrder(
            int nextOrderId,
            DateTime startTime,
            int timeout,
            Predicate<OpenOrderMessage> messageIsValid,
            out OpenOrderMessage openOrderMessage,
            out ErrorMessage errorMessage)
        {
            if (_queueOrderOpenMessage.DequeueMessage(nextOrderId, out openOrderMessage))
            {
                if (openOrderMessage != null)
                {
                    if (messageIsValid(openOrderMessage))
                    {
                        errorMessage = null;
                        return false;
                    }
                    else return !HasErrorOrTimeout(_queueOrderOpenMessage, nextOrderId, startTime, timeout, out errorMessage);
                }
                else
                {
                    return !HasErrorOrTimeout(_queueOrderOpenMessage, nextOrderId, startTime, timeout, out errorMessage);
                }
            }
            else
            {
                return !HasErrorOrTimeout(_queueOrderOpenMessage, nextOrderId, startTime, timeout, out errorMessage);
            }
        }

        private bool HasErrorOrTimeout(IIbHostQueue _queue, int reqId, DateTime startTime, int timeout, out ErrorMessage errorMessage)
        {
            if (_queue.DequeueMessage(reqId, out errorMessage)) return true;
            else
            {
                if ((DateTime.Now - startTime).TotalMilliseconds >= timeout)
                {
                    errorMessage = new ErrorMessage(reqId, 0, $"Timeout {timeout} ms.", "");
                    return true;
                }
                else return false;
            }
        }

        private void _ibClient_ManagedAccounts(ManagedAccountsMessage message)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"ReqId:0 managed accounts: {message.ManagedAccounts.Aggregate((r, n) => r + "," + n)}");

            _queueManagedAccounts.Enqueue(message);
        }

        private void _ibClient_NextValidId(ConnectionStatusMessage message)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add(message.IsConnected ? "CONNECTED!" : "NOT CONNECTED!");
            Consumer.ConnectedToTws = message.IsConnected;

            _queueConnectionStatus.Enqueue(message);
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

        private void _ibClient_AccountSummary(AccountSummaryMessage obj)
        {
            _queueAccountSummary.Enqueue(obj);
        }

        private void _ibClient_AccountSummaryEnd(AccountSummaryEndMessage obj)
        {
            _queueAccountSummaryEnd.Enqueue(obj);
        }

        private void _ibClient_Position(PositionMessage obj)
        {
            _queuePositions.Enqueue(obj);
        }

        private void _ibClient_PositionEnd()
        {
            _queuePositionsEnd.Enqueue(1);
        }

        private void _ibClient_ContractDetails(ContractDetailsMessage contractDetailsMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"ContractDetailsMessage for {contractDetailsMessage.ContractDetails.Contract.LocalSymbol} " +
                $"conId:{contractDetailsMessage.ContractDetails.Contract.ConId}");
            _queueCommon.Enqueue(contractDetailsMessage);
        }

        private void _ibClient_SymbolSamples(SymbolSamplesMessage symbolSamplesMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"SymbolSamplesMessage for {symbolSamplesMessage.ReqId}");
            _queueCommon.Enqueue(symbolSamplesMessage);
        }

        private void _ibClient_FundamentalData(FundamentalsMessage fundamentalsMessage)
        {
            _queueCommon.Enqueue(fundamentalsMessage);
        }

        private void _ibClient_Error(int reqId, int code, string message, string todo, Exception exception)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }

            if (_mktDataReqIds.Contains(reqId))
                _queueMktDataErrors.Enqueue(new ErrorMessage(reqId, code, message, ""));

            if (_placeOrderOrderIds.Contains(reqId))
                _queuePlaceOrderErrors.Enqueue(new ErrorMessage(reqId, code, message, ""));

            Consumer.TwsMessageCollection?.Add($"ReqId:{reqId} code:{code} message:{message} exception:{exception}");

            _queueError.Enqueue(new ErrorMessage(reqId, code, message, ""));
        }

        private void _ibClient_TickPrice(TickPriceMessage tickPriceMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"TickPriceMessage for {tickPriceMessage.RequestId} " +
                $"field:{tickPriceMessage.Field} price:{tickPriceMessage.Price}");

            _queueTickPriceMessage.Enqueue(tickPriceMessage);
            _requestResponseMapper.AddResponse(tickPriceMessage.RequestId, tickPriceMessage);
        }

        private void _ibClient_TickSize(TickSizeMessage tickSizeMessage)
        {
            // TODO
        }

        private void _ibClient_TickSnapshotEnd(int reqId)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"TickSnapshotEnd for {reqId} ");
            _queueCommon.Enqueue(new TickSnapshotEndMessage(reqId));
        }

        private void _ibClient_OpenOrder(OpenOrderMessage openOrderMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"OpenOrderMessage for order id:{openOrderMessage.OrderId} {openOrderMessage.Contract.LocalSymbol} " +
                $"conId:{openOrderMessage.Contract.ConId}");
            _queueOrderOpenMessage.DequeueAllOpenOrderMessagesExcept(openOrderMessage.OrderId);
            _queueOrderOpenMessage.Enqueue(openOrderMessage);
            _orderResponseMapper.AddResponse(openOrderMessage.OrderId, openOrderMessage);
        }

        private void _ibClient_OrderStatus(OrderStatusMessage orderStatusMessage)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"OrderStatusMessage for oredr id:{orderStatusMessage.OrderId} " +
                  $"why held:{orderStatusMessage.WhyHeld}");

            _orderResponseMapper.AddResponse(orderStatusMessage.OrderId, orderStatusMessage);
        }

        private bool MessageForRightTickerContains(string fundamentalsMessageString, string ticker)
        {
            if (ticker == "ALD1")
            {
                return fundamentalsMessageString.Contains("ALD");
            }

            return fundamentalsMessageString.Contains(ticker) || fundamentalsMessageString.Contains(ticker.ToUpper());
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

        private void _ibClient_HistoricalData(HistoricalDataMessage message)
        {
            _queueHistoricalData.Enqueue(message);

            object lockObject = new object();
            lock (lockObject)
            {
                // TODO use for tests
                //AddTestNoiseData(_requestDictionary);

                _requestResponseMapper.AddResponse(message.RequestId, message);
            }
        }

        private void _ibClient_HistoricalDataUpdate(HistoricalDataMessage message)
        {
            LiveDataMessage liveDataMessage = new LiveDataMessage(_requestIdContract[message.RequestId], message);
            _queueHistoricalDataUpdate.Enqueue(liveDataMessage);

            object lockObject = new object();
            lock (lockObject)
            {
                // TODO use for tests
                //AddTestNoiseData(_requestDictionary);

                _requestResponseMapper.AddResponse(message.RequestId, message);
            }
        }

        private void _ibClient_HistoricalDataEnd(HistoricalDataEndMessage message)
        {
            _queueHistoricalDataEnd.Enqueue(message);

            object lockObject = new object();
            lock (lockObject)
            {
                // TODO use for tests
                //AddTestNoiseData(_requestDictionary);

                _requestResponseMapper.AddResponse(message.RequestId, message);
            }
        }


        //private void AddTestNoiseData(ConcurrentDictionary<int, ConcurrentBag<object>> requestDictionary)
        //{
        //    for (int i = 0; i < 50; i++)
        //    {
        //        Random random = new Random();
        //        int reqId = random.Next(1000, 2000);

        //        requestDictionary[reqId] = new ConcurrentBag<object>
        //        {
        //            new HistoricalDataMessage(reqId, new Bar("", 10.0, 13.9, 9.7, 9.8, 10000, 2, 34)),
        //            new HistoricalDataMessage(reqId, new Bar("", 10.0, 13.9, 9.7, 9.8, 10000, 2, 34)),
        //            new ErrorMessage(reqId, 23, "", ""),
        //            new HistoricalDataEndMessage(reqId, "", "")
        //        };

        //    }
        //}
    }
}
