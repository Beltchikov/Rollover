using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventTrader.Requests;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EventTrader
{
    public class ViewModel : ObservableObject
    {
        private IInfiniteLoop _requestLoop;
        private string _tradeStatus = "";
        private Dispatcher _dispatcher;
        
        public ICommand StartSessionCommand { get; }
        public RelayCommand StopSessionCommand { get; }
        public ICommand TestDataSourceCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public ViewModel(IInfiniteLoop requestLoop)
        {
            _requestLoop = requestLoop;
            _requestLoop.Status += _requestLoop_Status;
            _dispatcher = Dispatcher.CurrentDispatcher; 

            StartSessionCommand = new RelayCommand(() => _requestLoop.StartAsync(() => { }, new object[] { }));
            StopSessionCommand = new RelayCommand(() => _requestLoop.Stopped = true, () => _requestLoop.IsRunning);
            TestDataSourceCommand = new RelayCommand(() => MessageBox.Show("TestDataSourceCommand"));
            TestConnectionCommand = new RelayCommand(() => MessageBox.Show("TestConnectionCommand"));
        }

        #region Critical section - called from other thread

        public string TradeStatus
        {
            get => _tradeStatus;
            set
            {
                SetProperty(ref _tradeStatus, value);
            }
        }
                
        private void _requestLoop_Status(string message)
        {
            _dispatcher.Invoke(() => StopSessionCommand.NotifyCanExecuteChanged());
            TradeStatus = message;
        }

        #endregion
    }
}
