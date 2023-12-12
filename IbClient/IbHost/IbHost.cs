using IBApi;
using IbClient.messages;
using IbClient.Types;
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
        private IIbHostQueue _queue;
        private int _currentReqId = 0;
        private IResponses _responses;
        private List<ErrorMessage> _errorMessages;
        private int _nextOrderId;
        private int _lastOrderId;
        public static readonly string DEFAULT_SEC_TYPE = "STK";
        public static readonly string DEFAULT_CURRENCY = "USD";
        public static readonly string DEFAULT_EXCHANGE = "SMART";

        public IbHost(IIbHostQueue queue)
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ConnectionClosed += _ibClient_ConnectionClosed;
            _ibClient.ContractDetails += _ibClient_ContractDetails;
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
            var reqId = ++_currentReqId;
            _ibClient.ClientSocket.reqContractDetails(reqId, contract);

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !HasMessageInQueue<ContractDetailsMessage>(reqId)) { }

                if (_queue.Dequeue() is ContractDetailsMessage contractDetailsMessage)
                {
                    contractDetails = contractDetailsMessage.ContractDetails;
                }
            });

            return contractDetails;
        }

        public async Task<string> RequestFundamentalDataAsync(Contract contract, string reportType, int timeout)
        {
            string fundamentalsMessageString = null;

            var reqId = ++_currentReqId;
            _ibClient.ClientSocket.reqFundamentalData(reqId, contract, reportType, new List<TagValue>());

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !HasMessageInQueue<FundamentalsMessage>()) { }

                if (_queue.Dequeue() is FundamentalsMessage fundamentalsMessage)
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
        public async Task<(double?, TickType?, MarketDataType?)> RequestMarketDataSnapshotAsync(Contract contract, MarketDataType[] marketDataTypes)
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
                        || HasErrorMessage(reqId, 10168)
                        || HasErrorMessage(reqId, 354))) { };

                    if (price == null)
                    {
                        marketDataType = marketDataTypes[1];
                    }
                }
            });

            return (price, tickType, marketDataType);
        }

        // full list of tick types: https://interactivebrokers.github.io/tws-api/tick_types.html</param>
        public async Task<double?> RequestMarketDataSnapshotAsync(Contract contract, TickType tickType)
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
                        || HasErrorMessage(reqId, 10168)
                        || HasErrorMessage(reqId, 354))) { };
                }
            });

            return price;
        }

        public async Task<OrderState> WhatIfOrderStateFromContract(Contract contract, int qty, int timeout)
        {
            OrderState orderState = null;
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

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !HasMessageInQueue<OpenOrderMessage>()) { }

                if (_queue.Dequeue() is OpenOrderMessage openOrderMessage)
                {
                    orderState = openOrderMessage.OrderState;
                }
            });

            return orderState;
        }

        private void _ibClient_Error(int reqId, int code, string message, Exception exception)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"ReqId:{reqId} code:{code} message:{message} exception:{exception}");

            if (code == 10168)
            {
                _errorMessages.Add(new ErrorMessage(reqId, code, message));
            }
            if (code == 354)
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
            _responses.AddTickPriceMessage(tickPriceMessage);
        }

        private void _ibClient_TickSnapshotEnd(int reqId)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"TickSnapshotEnd for {reqId} ");
            _responses.SetSnapshotEnd(reqId);
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

        private bool HasMessageInQueue<T>(int reqId)
        {
            var message = _queue.Peek();
            if (message == null)
            {
                return false;
            }

            if (!(message is T))
            {
                return false;
            }

            if (message is ContractDetailsMessage)
            {
                var contractDetailsMessage = message as ContractDetailsMessage;
                return contractDetailsMessage?.RequestId == reqId;
            }

            if (message is TickPriceMessage)
            {
                var tickPriceMessage = message as TickPriceMessage;
                return tickPriceMessage?.RequestId == reqId;
            }

            throw new NotImplementedException();
        }

        private bool HasMessageInQueue<T>()
        {
            var message = _queue.Peek();
            if (message == null)
            {
                return false;
            }

            if (!(message is T))
            {
                return false;
            }

            return true;
        }

        private bool MessageForRightTickerContains(string fundamentalsMessageString, string ticker)
        {
            if (ticker == "ALD1")
            {
                return fundamentalsMessageString.Contains("ALD");
            }

            return fundamentalsMessageString.Contains(ticker);
        }

        private bool HasErrorMessage(int reqId, int errorCode)
        {
            var errorMessagesCopy = _errorMessages.ToArray();
            return errorMessagesCopy.Any(c => c.RequestId == reqId && c.ErrorCode == errorCode);
        }
    }
}
