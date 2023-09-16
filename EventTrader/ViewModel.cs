using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dsmn.EconomicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Dsmn
{
    public class ViewModel : ObservableObject
    {
        private IEconDataLoop _econDataLoop;
        private string _tradeStatus = "";
        private Dispatcher _dispatcher;
        private int _frequency;
        private bool _stopSessionEnabled;
        private string _dataType;
        private string _url;
        private string _xPathActual;
        private string _xPathExpected;
        private string _xPathPrevious;
        private string _nullPlaceholder;

        public RelayCommand StartSessionCommand { get; }
        public RelayCommand StopSessionCommand { get; }
        public ICommand TestDataSourceCommand { get; }
        public ViewModel(IEconDataLoop econDataLoop)
        {
            _econDataLoop = econDataLoop;
            _econDataLoop.Status += _requestLoop_Status;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _frequency = 2000;
            _dataType = DataTypeEnum.NonFarmEmploymentChange.ToString();

            StartSessionCommand = new RelayCommand(
                () =>
                {
                    _econDataLoop.StartAsync(
                        Frequency,
                        DataType,
                        Url,
                        XPathActual,
                        XPathExpected,
                        XPathPrevious,
                        NullPlaceholder);
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
            
            // TODO DEV remove later
            Url = "https://www.investing.com/economic-calendar/";
            XPathActual = "//*[@id=\"eventActual_479408\"]";
            XPathExpected = "//*[@id=\"eventForecast_479408\"]";
            XPathPrevious = "//*[@id=\"eventPrevious_479408\"]";
            NullPlaceholder = "&nbsp;";
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

        public Dictionary<string, string> DataTypesUs => Countries.All.First(c => c.Symbol == "US")
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

        public string Url
        {
            get => _url;
            set
            {
                SetProperty(ref _url, value);
            }
        }

        public string XPathActual
        {
            get => _xPathActual;
            set
            {
                SetProperty(ref _xPathActual, value);
            }
        }

        public string XPathExpected
        {
            get => _xPathExpected;
            set
            {
                SetProperty(ref _xPathExpected, value);
            }
        }

        public string XPathPrevious
        {
            get => _xPathPrevious;
            set
            {
                SetProperty(ref _xPathPrevious, value);
            }
        }

        public string NullPlaceholder
        {
            get => _nullPlaceholder;
            set
            {
                SetProperty(ref _nullPlaceholder, value);
            }
        }

        private void SetProperty(ref object nullPlaceHolder, string value)
        {
            throw new NotImplementedException();
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
