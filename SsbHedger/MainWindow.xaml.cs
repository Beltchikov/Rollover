using IbClient;
using SsbHedger;
using SsbHedger.Abstractions;
using SsbHedger.Model;
using SsbHedger.ResponseProcessing;
using SsbHedger.ResponseProcessing.Mapper;
using SsbHedger.WpfIbClient;
using System.Windows;

namespace ViewModel.ListBinding
{
    /// <summary>
    /// Interaction logic for ListBindingWindow.xaml
    /// </summary>
    public partial class ListBindingWindow : Window
    {
        IWpfIbClient _logic;

        public ListBindingWindow()
        {
            InitializeComponent();

           _logic = new WpfIbClient(
                IBClient.CreateClient(),
                new ResponseLoop() { BreakCondition = () => 1 == 0 },
                new ResponseHandler(new ReaderThreadQueue()),
                new ResponseMapper(),
                new ResponseProcessor(new DispatcherAbstraction(Dispatcher)),
                new BackgroundWorkerAbstraction());
            _logic.Execute();

            _logic.Error += _logic_Error;
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
