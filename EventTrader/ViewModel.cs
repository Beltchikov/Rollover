using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventTrader.EconomicData;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EventTrader
{
    public class ViewModel : ObservableObject
    {
        private IEconDataLoop _econDataLoop;
        private string _tradeStatus = "";
        private Dispatcher _dispatcher;
        private int _frequency;
        private bool _stopSessionEnabled;
        private Countries _countries;
        private string _dataType;

        public RelayCommand StartSessionCommand { get; }
        public RelayCommand StopSessionCommand { get; }
        public ICommand TestDataSourceCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public ViewModel(IEconDataLoop econDataLoop)
        {
            _econDataLoop = econDataLoop;
            _econDataLoop.Status += _requestLoop_Status;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _frequency = 2000;
            _countries = new Countries();
            _dataType = DataTypeEnum.NonFarmEmploymentChange.ToString();

            StartSessionCommand = new RelayCommand(
                () =>
                {
                    _econDataLoop.StartAsync(Frequency, DataType);
                    StopSessionEnabled = true;
                },
                () => !_econDataLoop.IsRunning);
            StopSessionCommand = new RelayCommand(
                () =>
                {
                    StopSessionEnabled = false;
                    _econDataLoop.Stopped = true;
                    StopSessionCommand?.NotifyCanExecuteChanged();
                    StartSessionCommand.NotifyCanExecuteChanged();
                },
                () => _econDataLoop.IsRunning);
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

        public bool StopSessionEnabled
        {
            get => _stopSessionEnabled;
            set
            {
                SetProperty(ref _stopSessionEnabled, value);
            }
        }

        public Dictionary<string, string> DataTypesUs => _countries.All.First(c => c.Symbol == "US")
           .DataList.Select(l => new { Key = l.Type.ToString(), Value = l.Name })
            .ToDictionary(o=>o.Key, o=>o.Value);

        public string DataType
        {
            get => _dataType;
            set
            {
                SetProperty(ref _dataType, value);
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
