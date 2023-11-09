using IBApi;
using IbClient.messages;
using IbClient.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TickType = IbClient.Types.TickType;

namespace IbClient.IbHost
{
    public class IbHost : IIbHost
    {
        IIBClient _ibClient;
        private IIbHostQueue _queue;
        private IMarketDataResponseList _marketDataResponseList;
        private int _currentReqId = 0;
        private int _tickType;
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

            _queue = queue;
            _marketDataResponseList = new MarketDataResponseList();

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
                //while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !HasCompletedResponseInQueue(reqId)) { }
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

        /// <summary>
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="frozen"></param>
        /// <param name="tickType">Bid: 1, Ask: 2, 
        /// full list: https://interactivebrokers.github.io/tws-api/tick_types.html</param>
        /// <returns></returns>
        public async Task<double?> RequestMarketDataAsync(
            Contract contract,
            bool snapshot,
            int timeout)
        {
            MarketDataType[] marketDataTypes = new[] { MarketDataType.Live, MarketDataType.Live,
                MarketDataType.Frozen, MarketDataType.Delayed,
                MarketDataType.Delayed, MarketDataType.DelayedFrozen };
            TickType[] tickTypes = new[] { TickType.BidPrice, TickType.LastPrice,
                TickType.ClosePrice, TickType.DelayedBid,
                TickType.DelayedLast, TickType.DelayedClose };
            string[] comments = new[]
            { "LIVE BID", "LIVE LAST TRADED", "FROZEN CLOSE", "DELAYED BID", "DELAYED LAST TRADED", "FROZEN DELAYED CLOSE" };

            double? price = null;


            //MarketDataType[] marketDataTypes = new[] { MarketDataType.Live };
            //TickType[] tickTypes = new[] { TickType.BidPrice };
            //string[] comments = new[] { "LIVE BID" };



            await Task.Run(() =>
            {

                for (int i = 0; i < marketDataTypes.Length; i++)
                {
                    var marketDataType = marketDataTypes[i];
                    _ibClient.ClientSocket.reqMarketDataType(((int)marketDataType));

                    var reqId = ++_currentReqId;
                    _marketDataResponseList.Add(reqId);
                    _ibClient.ClientSocket.reqMktData(
                       reqId,
                       contract,
                       "",
                       snapshot,
                       false,
                       new List<TagValue>());
                    var startTime = DateTime.Now;
                    MarketDataSnapshotResponse marketDataSnapshotResponse = null;
                    while (_queue.Count() == 0 && (DateTime.Now - startTime).TotalMilliseconds < timeout) { };
                    GetCompletedResponseFromQueue(reqId, out marketDataSnapshotResponse);
                    if (marketDataSnapshotResponse != null)
                    {
                        var tickType = tickTypes[i];
                        price = marketDataSnapshotResponse.GetPrice(tickType);
                        break;
                    }
                }
            });

            return price;




            /////////////////////////////////////////////
            //_tickType = (int)tickType;
            //_ibClient.ClientSocket.reqMarketDataType(((int)marketDataType));

            //double? price = null;
            //var reqId = ++_currentReqId;
            //_marketDataResponseList.Add(reqId);
            //_ibClient.ClientSocket.reqMktData(
            //   reqId,
            //   contract,
            //   "",
            //   snapshot,
            //   false,
            //   new List<TagValue>());

            //await Task.Run(() =>
            //{
            //    var startTime = DateTime.Now;
            //    while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !HasCompletedResponseInQueue(reqId)) { }
            //    if (_queue.Dequeue() is MarketDataSnapshotResponse marketDataSnapshotResponse)
            //    {
            //        price = marketDataSnapshotResponse.GetPrice(tickType);
            //    }
            //});

            //return price;
        }

        private void _ibClient_Error(int reqId, int code, string message, Exception exception)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"ReqId:{reqId} code:{code} message:{message} exception:{exception}");
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
            _marketDataResponseList.UpdateResponse(tickPriceMessage);
        }

        private void _ibClient_TickSnapshotEnd(int reqId)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageCollection?.Add($"TickSnapshotEnd for {reqId} ");

            // TODO
            var response = _marketDataResponseList.SetCompleted(reqId);
            _queue.Enqueue(response);
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

        private bool GetCompletedResponseFromQueue(int reqId, out MarketDataSnapshotResponse marketDataSnapshotResponse)
        {
            marketDataSnapshotResponse = _queue.Dequeue() as MarketDataSnapshotResponse;
            if (marketDataSnapshotResponse == null)
            {
                return false;
            }
            if (marketDataSnapshotResponse.GetReqId() != reqId)
            {
                return false;
            }
            if (!marketDataSnapshotResponse.EndOfSnapshot())
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

        private bool IsValidPrice(double? price)
        {
            return price.HasValue && price.Value > 0;
        }
    }
}
