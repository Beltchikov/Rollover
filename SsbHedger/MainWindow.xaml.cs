using SsbHedger;
using SsbHedger.Model;
using SsbHedger.WpfIbClient;
using System.Linq;
using System.Windows;

namespace ViewModel.ListBinding
{
    /// <summary>
    /// Interaction logic for ListBindingWindow.xaml
    /// </summary>
    public partial class ListBindingWindow : Window
    {
        IWpfIbClient _ibClient;
        ConfigurationWindow _configurationWindow = new ConfigurationWindow();

        public ListBindingWindow()
        {
            InitializeComponent();

            var host = "localhost";
            int port = 4001;
            int clientId = 1;

            _ibClient = WpfIbClient.Create(() => 1 == 0, Dispatcher);
            _ibClient.Execute(host, port, clientId);
            _ibClient.Error += _logic_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;

            ((MainWindowViewModel)DataContext).Host = host;
            ((MainWindowViewModel)DataContext).Port = port;
            ((MainWindowViewModel)DataContext).ClientId = clientId;
        }

        private void _logic_Error(int reqId, string message)
        {
            ((MainWindowViewModel)DataContext).Messages.Add(new Message { ReqId = reqId, Body = message });
        }

        private void _ibClient_NextValidId(IbClient.messages.ConnectionStatusMessage message)
        {
            var viewModel = ((MainWindowViewModel)DataContext);
            var connected = "CONNECTED!";

            viewModel.Messages.Add(
                new Message { ReqId = 0, Body = message.IsConnected ? connected : "NOT CONNECTED" });
            viewModel.Connected = message.IsConnected;
        }

        private void _ibClient_ManagedAccounts(IbClient.messages.ManagedAccountsMessage message)
        {
            ((MainWindowViewModel)DataContext).Messages.Add(
                new Message { 
                    ReqId = 0, 
                    Body = $"Managed accounts: {message.ManagedAccounts.Aggregate((r,n) => r + "," +n)}" 
                });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void btConfiguration_Click(object sender, RoutedEventArgs e)
        {
            _configurationWindow.ShowDialog();
        }
    }
}
