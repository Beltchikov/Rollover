using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dsmn.DataProviders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Dsmn
{
    public class ViewModel : ObservableObject
    {
        private string _tickerString;
        private ObservableCollection<string> _resultList;
        private string _message;

        public ICommand LastEpsCommand { get; }
        public ICommand ExpectedEpsCommand { get; }
        public ViewModel(IYahooProvider investingProvider)
        {
            investingProvider.Status += InvestingProvider_Status;
           
            LastEpsCommand = new RelayCommand(async () =>
            {
                ResultList = new ObservableCollection<string>(await investingProvider.LastEpsAsync(TickerList));
            });

            ExpectedEpsCommand = new RelayCommand(async () =>
            {
                ResultList = new ObservableCollection<string>(await investingProvider.ExpectedEpsAsync(TickerList));
            });

           TickerString = " SKX\r\nPFS\r\nSLCA\r\n WT";
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

        public ObservableCollection<string> ResultList
        {
            get => _resultList;
            set
            {
                SetProperty(ref _resultList, value);
                OnPropertyChanged(nameof(ResultString));
            }
        }

        public string ResultString
        {
            get
            {
                return ResultList == null 
                    ? string.Empty 
                    : ResultList.Aggregate((r, n) => r + "\r\n" + n);
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                SetProperty(ref _message, value);
            }
        }

        private void InvestingProvider_Status(string message)
        {
            Message = message;
        }
    }
}
