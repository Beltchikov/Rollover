using SsbHedger.Model;
using SsbHedger.WpfIbClient;
using SsbHedger.WpfIbClient.ResponseObservers;
using System.Windows;

namespace ViewModel.ListBinding
{
    /// <summary>
    /// Interaction logic for ListBindingWindow.xaml
    /// </summary>
    public partial class ListBindingWindow : Window
    {
        IWpfIbClient _ibClient;
        ConnectionObserver _connectionObserver;

        public ListBindingWindow()
        {
            InitializeComponent();

            _ibClient = WpfIbClient.Create(() => 1 == 0, Dispatcher);
            _connectionObserver = new ConnectionObserver();
            _connectionObserver.Subscribe(_ibClient);
            _ibClient.Connect("localhost", 4001, 1);


            // Obsolete
            _ibClient.Error += _logic_Error;
        }

        private void _logic_Error(int reqId, string message)
        {
            ((MainWindowViewModel)DataContext).Messages.Add(new Message { ReqId = reqId, Body = message });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
