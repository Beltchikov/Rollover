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

        public ICommand TestDataSourceCommand { get; }
        public ICommand ExpectedEpsCommand { get; }

        public ViewModel(IYahooProvider investingProvider)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _frequency = 2000;
            _dataType = DataTypeEnum.NonFarmEmploymentChange.ToString();

            TestDataSourceCommand = new RelayCommand(() => MessageBox.Show("TestDataSourceCommand"));
            ExpectedEpsCommand = new RelayCommand(() => TickerListWithExpectedEps = investingProvider.ExpectedEps(TickerList));

            // TODO DEV remove later
            //Url = "https://www.investing.com/economic-calendar/";
            //XPathActual = "//*[@id=\"eventActual_479408\"]";
            //XPathExpected = "//*[@id=\"eventForecast_479408\"]";
            //XPathPrevious = "//*[@id=\"eventPrevious_479408\"]";
            //NullPlaceholder = "&nbsp;";

            TickerString = " SKX\r\nPFS\r\nSLCA\r\n WT";
            

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

        #endregion
    }
}
