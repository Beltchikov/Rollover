using IbClient;
using SsbHedger2.Model;
using System;
using System.Linq;
using IbClient.messages;

namespace SsbHedger2
{
    public class IbHost : IIbHost
    {
        IIBClient _ibClient;

        public IbHost()
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
        }

        public MainWindowViewModel? ViewModel { get; set; }

        public string DefaultHost => "localhost";

        public int DefaultPort => 4001;

        public int DefaultClientId => 1;

        public void ConnectAndStartReaderThread(string host, int port, int clientId)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Host = host;
            ViewModel.Port = port;
            ViewModel.ClientId = clientId;
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
        }
    }
}
