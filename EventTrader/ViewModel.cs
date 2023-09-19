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
        private string _tickerStringYahoo;
        private ObservableCollection<string> _resultList;
        private string _messageYahoo;
        private int _decimalSeparatorSelectedIndex;

        public ICommand LastEpsCommand { get; }
        public ICommand ExpectedEpsCommand { get; }
        public ViewModel(IYahooProvider investingProvider)
        {
            investingProvider.Status += InvestingProvider_Status;
           
            LastEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndex = 0;
                ResultList = new ObservableCollection<string>(await investingProvider.LastEpsAsync(TickerList, 5));
            });

            ExpectedEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndex = 0;
                ResultList = new ObservableCollection<string>(await investingProvider.ExpectedEpsAsync(TickerList, 5));
            });

           TickerStringYahoo = " SKX\r\nPFS\r\nSLCA\r\n WT";
        }

        public List<string> TickerList
        {
            get => TickerStringYahoo.Split("\r\n", StringSplitOptions.TrimEntries).ToList();
        }

        public string TickerStringYahoo
        {
            get => _tickerStringYahoo;
            set
            {
                SetProperty(ref _tickerStringYahoo, value);
            }
        }

        public ObservableCollection<string> ResultList
        {
            get => _resultList;
            set
            {
                SetProperty(ref _resultList, value);
                OnPropertyChanged(nameof(ResultStringYahoo));
            }
        }

        public string ResultStringYahoo
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

        public string MessageYahoo
        {
            get => _messageYahoo;
            set
            {
                SetProperty(ref _messageYahoo, value);
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
            MessageYahoo = message;
        }
    }
}
