using IBApi;
using IbClient.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IbClient.IbHost
{
    public class IbHost : IIbHost
    {
        IIBClient _ibClient;
        private IIbHostQueue _queue;
        private int _currentReqId = 0;


        private readonly string STK = "STK";
        private readonly string USD = "USD";
        private readonly string SMART = "SMART";

        public IbHost(IIbHostQueue queue)
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ConnectionClosed += _ibClient_ConnectionClosed;
            _ibClient.ContractDetails += _ibClient_ContractDetails;
            _ibClient.FundamentalData += _ibClient_FundamentalData;

            _queue = queue;

            //_ibClient.HistoricalData += _ibClient_HistoricalData;
            //_ibClient.HistoricalDataUpdate += _ibClient_HistoricalDataUpdate;
            //_ibClient.HistoricalDataEnd += _ibClient_HistoricalDataEnd;
            //_ibClient.Position += _ibClient_Position;
            //_ibClient.PositionEnd += _ibClient_PositionEnd;
            //_ibClient.ContractDetails += _ibClient_ContractDetails;
            //_ibClient.ContractDetailsEnd += _ibClient_ContractDetailsEnd;
            //_ibClient.TickPrice += _ibClient_TickPrice;
            //_ibClient.TickSize += _ibClient_TickSize;
            //_ibClient.TickPrice += _ibClient_TickPrice;
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

        public async Task<ContractDetails> RequestContractDetailsAsync(
            string ticker,
            int timeout,
            string currency,
            string secType,
            string exchange)
        {
            ContractDetails contractDetails = null;

            var contract = new Contract()
            {
                Symbol = ticker,
                Currency = currency ?? USD,
                SecType = secType ?? STK,
                Exchange = exchange ?? SMART
            };
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

        public async Task<string> RequestFundamentalDataAsync(
            string ticker,
            string reportType,
            int timeout,
            string currency,
            string secType,
            string exchange)
        {
            string fundamentalsMessageString = null;

            var contract = new Contract()
            {
                Symbol = ticker,
                SecType = STK,
                Currency = USD,
                Exchange = SMART
            };
            var reqId = ++_currentReqId;
            _ibClient.ClientSocket.reqFundamentalData(reqId, contract, reportType, new List<TagValue>());

            await Task.Run(() =>
            {
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !HasMessageInQueue<FundamentalsMessage>()) { }

                if (_queue.Dequeue() is FundamentalsMessage fundamentalsMessage)
                {
                    fundamentalsMessageString = fundamentalsMessage.Data;
                    if (!fundamentalsMessageString.Contains(ticker))
                    {
                        fundamentalsMessageString = ticker + " ERROR!";
                    }
                }
            });

            return fundamentalsMessageString;
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
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            _queue.Enqueue(fundamentalsMessage);
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

            return true;
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
    }
}
