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
        private int _decimalSeparatorSelectedIndex;

        public ICommand LastEpsCommand { get; }
        public ICommand ExpectedEpsCommand { get; }
        public ViewModel(IYahooProvider investingProvider)
        {
            investingProvider.Status += InvestingProvider_Status;
           
            LastEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndex = 0;
                ResultList = new ObservableCollection<string>(await investingProvider.LastEpsAsync(TickerList));
            });

            ExpectedEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndex = 0;
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
                if (ResultList == null)
                {
                    return string.Empty;
                }
                else
                {
                    if (!ResultList.Any())
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return ResultList.Aggregate((r, n) => r + "\r\n" + n);
                    }
                }
            }
            set
            {
                SetProperty(ref _resultList, new ObservableCollection<string>());
                OnPropertyChanged(nameof(ResultList));
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

        public int DecimalSeparatorSelectedIndex
        {
            get => _decimalSeparatorSelectedIndex;
            set
            {
                SetProperty(ref _decimalSeparatorSelectedIndex, value);
            }
        }

        private void InvestingProvider_Status(string message)
        {
            Message = message;
        }
    }
}
