using System;

namespace SsbHedger2.CommandHandler
{
    internal class InitializeCommandHandler
    {
        internal static void Handle(IIbHost ibHost)
        {
            throw new NotImplementedException();

            //_ibClient = WpfIbClient.WpfIbClient.Create(() => 1 == 0, Dispatcher);
            //_ibClient.Execute(host, port, clientId);
            //_ibClient.Error += _logic_Error;
            //_ibClient.NextValidId += _ibClient_NextValidId;
            //_ibClient.ManagedAccounts += _ibClient_ManagedAccounts;

            //((MainWindowViewModel)DataContext).Host = host;
            //((MainWindowViewModel)DataContext).Port = port;
            //((MainWindowViewModel)DataContext).ClientId = clientId;

            //_configurationWindow = new(host, port, clientId);
        }
    }
}
