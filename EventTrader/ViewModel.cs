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
        private int _frequency;

        public RelayCommand StartSessionCommand { get; }
        public RelayCommand StopSessionCommand { get; }
        public ICommand TestDataSourceCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public ViewModel(IInfiniteLoop requestLoop)
        {
            _requestLoop = requestLoop;
            _requestLoop.Status += _requestLoop_Status;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _frequency = 2000;

            StartSessionCommand = new RelayCommand(
                () => _requestLoop.StartAsync(() => { }, new object[] { Frequency }),
                () => !_requestLoop.IsRunning);
            StopSessionCommand = new RelayCommand(
                () =>
                {
                    _requestLoop.Stopped = true;
                    StopSessionCommand?.NotifyCanExecuteChanged();
                    StartSessionCommand.NotifyCanExecuteChanged();
                },
                () => _requestLoop.IsRunning);
            TestDataSourceCommand = new RelayCommand(() => MessageBox.Show("TestDataSourceCommand"));
            TestConnectionCommand = new RelayCommand(() => MessageBox.Show("TestConnectionCommand"));
        }

        public int Frequency
        {
            get => _frequency;
            set
            {
                SetProperty(ref _frequency, value);
            }
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
            _dispatcher.Invoke(() =>
            {
                StopSessionCommand.NotifyCanExecuteChanged();
                StartSessionCommand.NotifyCanExecuteChanged();
                TradeStatus = message;
            });
        }

        #endregion
    }
}
