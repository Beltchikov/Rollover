using IbClient;
using SsbHedger.Model;
using System;
using System.Linq;
using IbClient.messages;
using SsbHedger.Configuration;

namespace SsbHedger
{
    public class IbHost : IIbHost
    {
        IIBClient _ibClient;
        string _host = null!;
        int _port;
        int _clientId;

        public IbHost(IConfiguration configuration)
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ConnectionClosed += _ibClient_ConnectionClosed;
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
    }
}
