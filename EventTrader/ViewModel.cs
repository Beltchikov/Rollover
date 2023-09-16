using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dsmn.DataProviders;
using Dsmn.EconomicData;
using Microsoft.AspNetCore.Http;
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
        private Dispatcher _dispatcher;
        private int _frequency;
        private bool _stopSessionEnabled;
        private string _dataType;
        private string _urlEpsExpected;
        private string _xPathEpsExpected;
        private string _xPathExpected;
        private string _xPathPrevious;
        private string _nullPlaceholder;
        private string _tickerString;
        private List<string> _tickerListWithExpectedEps;

        public RelayCommand StartSessionCommand { get; }
        public RelayCommand StopSessionCommand { get; }
        public ICommand TestDataSourceCommand { get; }
        public ICommand ExpectedEpsCommand { get; }

        public ViewModel(IEconDataLoop econDataLoop, IInvestingDataProvider investingProvider)
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
                        UrlEpsExpected,
                        XPathEpsExpected,
                        XPathExpected,
                        XPathPrevious,
                        NullPlaceholderEpsExpected);
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
            ExpectedEpsCommand = new RelayCommand(() => TickerListWithExpectedEps = investingProvider.ExpectedEps(UrlEpsExpected,
                                                                                                                  XPathEpsExpected,
                                                                                                                  NullPlaceholderEpsExpected,
                                                                                                                  TickerList));

            // TODO DEV remove later
            //Url = "https://www.investing.com/economic-calendar/";
            //XPathActual = "//*[@id=\"eventActual_479408\"]";
            //XPathExpected = "//*[@id=\"eventForecast_479408\"]";
            //XPathPrevious = "//*[@id=\"eventPrevious_479408\"]";
            //NullPlaceholder = "&nbsp;";

            TickerString = "PFS\r\nSLCA\r\n WT";
            UrlEpsExpected = "https://www.investing.com/earnings-calendar/";
            XPathEpsExpected = "//*[@id=\"earningsCalendarData\"]";
            NullPlaceholderEpsExpected = "&nbsp;";

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

        public string UrlEpsExpected
        {
            get => _urlEpsExpected;
            set
            {
                SetProperty(ref _urlEpsExpected, value);
            }
        }

        public string XPathEpsExpected
        {
            get => _xPathEpsExpected;
            set
            {
                SetProperty(ref _xPathEpsExpected, value);
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

        public string NullPlaceholderEpsExpected
        {
            get => _nullPlaceholder;
            set
            {
                SetProperty(ref _nullPlaceholder, value);
            }
        }

        public List<string> TickerList
        {
            get => TickerString.Split("\r\n", StringSplitOptions.TrimEntries).ToList();
        }

        public string TickerString
        {
            get => _tickerString;
            set
            {
                SetProperty(ref _tickerString, value);
            }
        }

        public List<string> TickerListWithExpectedEps
        {
            get => _tickerListWithExpectedEps;
            set
            {
                SetProperty(ref _tickerListWithExpectedEps, value);
            }
        }
        
        private void SetProperty(ref object nullPlaceHolder, string value)
        {
            throw new NotImplementedException();
        }

        #region Critical section - called from other thread

        private void _requestLoop_Status(string message)
        {
            _dispatcher.Invoke(() =>
            {
                StopSessionCommand.NotifyCanExecuteChanged();
                StartSessionCommand.NotifyCanExecuteChanged();
            });
        }

        #endregion
    }
}
