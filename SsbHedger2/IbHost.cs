using SsbHedger2.WpfIbClient;
using System.Windows.Threading;

namespace SsbHedger2
{
    internal class IbHost : IIbHost
    {
        IWpfIbClient _ibClient;

        public IbHost(string host, int port, int clientId)
        {
            //_ibClient = WpfIbClient.WpfIbClient.Create(() => 1 == 0, Dispatcher);
            //_ibClient.Execute(host, port, clientId);
            //_ibClient.Error += _ibClient_Error;
            //_ibClient.NextValidId += _ibClient_NextValidId;
            //_ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
        }

        private void _ibClient_ManagedAccounts(IbClient.messages.ManagedAccountsMessage obj)
        {
            throw new System.NotImplementedException();
        }

        private void _ibClient_NextValidId(IbClient.messages.ConnectionStatusMessage obj)
        {
            throw new System.NotImplementedException();
        }

        private void _ibClient_Error(int arg1, string arg2)
        {
            throw new System.NotImplementedException();
        }
    }
}
