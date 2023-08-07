using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventTrader.Requests;
using System.Windows;
using System.Windows.Input;

namespace EventTrader
{
    public class ViewModel : ObservableObject
    {
        private IInfiniteLoop _requestLoop;
        private string _tradeStatus;
        private bool _requestLoopRunning;

        public ICommand StartSessionCommand { get; }
        public RelayCommand StopSessionCommand { get; }
        public ICommand TestDataSourceCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public ViewModel(IInfiniteLoop requestLoop)
        {
            _requestLoop = requestLoop;
            _requestLoop.Status += _requestLoop_Status;

            StartSessionCommand = new RelayCommand(() => _requestLoop.StartAsync(() => { }, new object[] { }));
            StopSessionCommand = new RelayCommand(() => _requestLoop.Stopped = true, () => RequestLoopRunning);
            TestDataSourceCommand = new RelayCommand(() => MessageBox.Show("TestDataSourceCommand"));
            TestConnectionCommand = new RelayCommand(() => MessageBox.Show("TestConnectionCommand"));
        }

        public string TradeStatus
        {
            get => _tradeStatus;
            set
            {
                SetProperty(ref _tradeStatus, value);
            }
        }

        public bool RequestLoopRunning
        {
            get => _requestLoopRunning;
            set
            {
                SetProperty(ref _requestLoopRunning, value);
            }
        }

        private void _requestLoop_Status(string message)
        {
            RequestLoopRunning = true;
            //StopSessionCommand.NotifyCanExecuteChanged();
            TradeStatus = message;
        }
    }
}
