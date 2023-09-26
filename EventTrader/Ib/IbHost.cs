using IbClient;
using IbClient.messages;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Eomn.Ib
{
    public class IbHost : IIbHost
    {
        IIBClient _ibClient;
        private IIbClientQueue _queue;
        private int _currentReqId = 0;


        private readonly string STK = "STK";
        private readonly string USD = "USD";
        private readonly string SMART = "SMART";

        public IbHost(IIbClientQueue queue)
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ConnectionClosed += _ibClient_ConnectionClosed;
            _ibClient.ContractDetails += _ibClient_ContractDetails;

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

        public IIbConsumer? Consumer { get; set; }

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

        public int RequestContractId(string ticker, int timeout)
        {
            // TODO make async

            var contract = new IBApi.Contract()
            {
                Symbol = ticker,
                SecType = STK,
                Currency = USD,
                Exchange = SMART
            };
            var reqId = ++_currentReqId;
            _ibClient.ClientSocket.reqContractDetails(reqId, contract);

            // TODO use IbClientQueue

            //var startTime = DateTime.Now;
            //while ((DateTime.Now - startTime).TotalMilliseconds < timeout && !Consumer.ConnectedToTws) 
            //{
            //    var contract = new Contract()
            //    {
            //        Symbol = ticker,
            //        SecType = STK,
            //        Currency = USD,
            //        Exchange = SMART
            //    };

            //    // TODO
            //    _ibClient.ClientSocket.reqContractDetails(-1, contract);
            //}

            // TODO
            return -1;
        }

        private void _ibClient_Error(int reqId, int code, string message, Exception exception)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageList?.Add($"ReqId:{reqId} code:{code} message:{message} exception:{exception}");
        }

        private void _ibClient_ManagedAccounts(ManagedAccountsMessage message)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageList?.Add($"ReqId:0 managed accounts: {message.ManagedAccounts.Aggregate((r, n) => r + "," + n)}");
        }

        private void _ibClient_NextValidId(ConnectionStatusMessage message)
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageList?.Add(message.IsConnected ? "CONNECTED!" : "NOT CONNECTED!");
            Consumer.ConnectedToTws = message.IsConnected;
        }

        private void _ibClient_ConnectionClosed()
        {
            if (Consumer == null)
            {
                throw new ApplicationException("Unexpected! Consumer is null");
            }
            Consumer.TwsMessageList?.Add("DISCONNECTED!");
            Consumer.ConnectedToTws = false;
        }

        private void _ibClient_ContractDetails(ContractDetailsMessage contractDetailsMessage)
        {
            _queue.Enqueue(contractDetailsMessage);    
        }
    }
}
