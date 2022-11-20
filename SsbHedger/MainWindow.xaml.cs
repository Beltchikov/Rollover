using IbClient;
using SsbHedger;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing.Mapper;
using SsbHedger.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SsbHedger.Model;

namespace ViewModel.ListBinding
{
    /// <summary>
    /// Interaction logic for ListBindingWindow.xaml
    /// </summary>
    public partial class ListBindingWindow : Window
    {
        ILogic _logic;

        public ListBindingWindow()
        {
            InitializeComponent();

            var readerQueue = new ReaderThreadQueue();
            var dispatcher = new DispatcherAbstraction(Dispatcher);

            _logic = new Logic(
                IBClient.CreateClient(),
                new ResponseLoop() { BreakCondition = () => 1 == 0 },
                new ResponseHandler(readerQueue),
                new ResponseMapper(),
                new ResponseProcessor(dispatcher),
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
