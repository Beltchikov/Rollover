using IbClient;
using SsbHedger.Model;
using System;
using System.Linq;
using IbClient.messages;

namespace SsbHedger
{
    public class IbHost : IIbHost
    {
        IIBClient _ibClient;
        IConfiguration _configuration;

        public IbHost(IConfiguration configuration)
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;

            _configuration = configuration;
        }

        public MainWindowViewModel? ViewModel { get; set; }

        public void ConnectAndStartReaderThread(string host, int port, int clientId)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            _ibClient.ConnectAndStartReaderThread(host, port, clientId);
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

            var host = _configuration.GetValue("Host");
            var port = _configuration.GetValue("Port");
            var clientId = _configuration.GetValue("ClientId");
            ViewModel.ConnectionMessage = isConnected
                    ? $"CONNECTED! {host}, {port}, client ID: {clientId}"
                    : $"NOT CONNECTED! {host}, {port}, client ID: {clientId}";
        }
    }
}
