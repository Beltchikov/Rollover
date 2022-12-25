using IbClient;
using SsbHedger2.Model;
using System;

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

        private void _ibClient_ManagedAccounts(IbClient.messages.ManagedAccountsMessage obj)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_NextValidId(IbClient.messages.ConnectionStatusMessage obj)
        {
            throw new NotImplementedException();
        }

    }
}
