using IbClient;
using SsbHedger.Model;
using System;
using System.Linq;
using IbClient.messages;
using SsbHedger.SsbConfiguration;
using IBApi;
using System.Collections.Generic;

namespace SsbHedger
{
    public class IbHost : IIbHost
    {
        IConfiguration _configuration;

        IIBClient _ibClient;
        string _host = null!;
        int _port;
        int _clientId;

        int _reqIdHistoricalData = 1000;
        Dictionary<string, Contract> _contractDict = null!;
        Contract _contractUnderlying = null!;

        public IbHost(IConfiguration configuration)
        {
            _configuration = configuration;

            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ConnectionClosed += _ibClient_ConnectionClosed;
            _ibClient.HistoricalData += _ibClient_HistoricalData;
            _ibClient.HistoricalDataUpdate += _ibClient_HistoricalDataUpdate;
            _ibClient.HistoricalDataEnd += _ibClient_HistoricalDataEnd;

            _contractDict = new Dictionary<string, Contract>
            {
                {"SPY", new Contract(){Symbol = "SPY", SecType = "STK", Currency="USD", Exchange = "SMART"} }
            };

            _contractUnderlying = _contractDict[(string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL)];

        }

        public MainWindowViewModel? ViewModel { get; set; }

        public void ConnectAndStartReaderThread(string host, int port, int clientId)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            _host = host;
            _port = port;
            _clientId = clientId;
            _ibClient.ConnectAndStartReaderThread(host, port, clientId);
        }

        public void Disconnect()
        {
            _ibClient.Disconnect();
        }

        public void ReqHistoricalData()
        {
            _reqIdHistoricalData++;
            //_ibClient.ClientSocket.reqHistoricalData(currentTicker + HISTORICAL_ID_BASE, contract, endDateTime, durationString, barSizeSetting, whatToShow, useRTH, 1, keepUpToDate, new List<TagValue>());
        }

        private void _ibClient_Error(int reqId, int code, string message, Exception exception)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message
            { ReqId = reqId, Body = $"Code:{code} message:{message} exception:{exception}" });
            UpdateConnectionMessage(ViewModel.Connected);
        }

        private void _ibClient_ManagedAccounts(ManagedAccountsMessage message)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message
            {
                ReqId = 0,
                Body = $"Managed accounts: {message.ManagedAccounts.Aggregate((r, n) => r + "," + n)}"
            });
        }

        private void _ibClient_NextValidId(ConnectionStatusMessage message)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message
            {
                ReqId = 0,
                Body = message.IsConnected ? "CONNECTED!" : "NOT CONNECTED!"
            });
            ViewModel.Connected = message.IsConnected;
            UpdateConnectionMessage(message.IsConnected);
        }

        private void UpdateConnectionMessage(bool isConnected)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.ConnectionMessage = isConnected
                    ? $"CONNECTED! {_host}, {_port}, client ID: {_clientId}"
                    : $"NOT CONNECTED! {_host}, {_port}, client ID: {_clientId}";
        }

        private void _ibClient_ConnectionClosed()
        {
            const string DISCONNECTED = "DISCONNECTED!";


            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message
            {
                ReqId = 0,
                Body = DISCONNECTED
            });
            ViewModel.Connected = false;
            ViewModel.ConnectionMessage = DISCONNECTED;
        }

        private void _ibClient_HistoricalData(HistoricalDataMessage obj)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_HistoricalDataUpdate(HistoricalDataMessage obj)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_HistoricalDataEnd(HistoricalDataEndMessage obj)
        {
            throw new NotImplementedException();
        }
    }
}
