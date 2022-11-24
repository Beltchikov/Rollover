using IbClient;
using SsbHedger;
using SsbHedger.Abstractions;
using SsbHedger.Model;
using SsbHedger.ResponseProcessing;
using SsbHedger.ResponseProcessing.Mapper;
using SsbHedger.WpfIbClient;
using System.Net;
using System.Windows;

namespace ViewModel.ListBinding
{
    /// <summary>
    /// Interaction logic for ListBindingWindow.xaml
    /// </summary>
    public partial class ListBindingWindow : Window
    {
        IWpfIbClient _ibClient;

        public ListBindingWindow()
        {
            InitializeComponent();

            _ibClient = WpfIbClient.Create(() => 1 == 0, Dispatcher);
            _ibClient.Execute();
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
