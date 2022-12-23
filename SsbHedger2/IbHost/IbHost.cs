using IbClient;
using SsbHedger2.Model;
using System;
using System.Windows.Threading;

namespace SsbHedger2.IbHost
{
    internal class IbHost : IIbHost
    {
        IIBClient _ibClient;

        public IbHost(MainWindowViewModel viewModel, string host, int port, int clientId)
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;

            _ibClient.ConnectAndStartReaderThread(host, port, clientId);
        }

        private void _ibClient_Error(int reqId, int code, string message, Exception exception)
        {
            throw new NotImplementedException();
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
