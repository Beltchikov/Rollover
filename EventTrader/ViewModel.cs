using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventTrader.Requests;
using System.Windows;
using System.Windows.Input;

namespace EventTrader
{
    public class ViewModel: ObservableObject
    {
        private IInfiniteLoop _requestLoop;

        public ICommand StartSessionCommand { get; }
        public ICommand StopSessionCommand { get; }
        public ICommand TestDataSourceCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public ViewModel(IInfiniteLoop requestLoop)
        {
            _requestLoop = requestLoop;
            _requestLoop.Status += _requestLoop_Status;

            StartSessionCommand = new RelayCommand(() => _requestLoop.Start(() => { }, new object[] {}));
            StopSessionCommand = new RelayCommand(() => _requestLoop.Stopped = true, () => _requestLoop.IsRunning);
            TestDataSourceCommand = new RelayCommand(() => MessageBox.Show("TestDataSourceCommand"));
            TestConnectionCommand = new RelayCommand(() => MessageBox.Show("TestConnectionCommand"));
        }

        private void _requestLoop_Status(string message)
        {
            MessageBox.Show(message);
        }
    }
}
