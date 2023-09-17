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
        private ObservableCollection<string> _tickerListWithExpectedEps;

        public ICommand TestDataSourceCommand { get; }
        public ICommand ExpectedEpsCommand { get; }

        public ViewModel(IYahooProvider investingProvider)
        {
            ExpectedEpsCommand = new RelayCommand(async () =>
            {
                TickerListWithExpectedEps = new ObservableCollection<string>(await investingProvider.ExpectedEpsAsync(TickerList));
            });

            // TODO DEV remove later
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

        public ObservableCollection<string> TickerListWithExpectedEps
        {
            get => _tickerListWithExpectedEps;
            set
            {
                SetProperty(ref _tickerListWithExpectedEps, value);
                OnPropertyChanged(nameof(TickerWithExpectedEps));
            }
        }

        public string TickerWithExpectedEps
        {
            get => TickerListWithExpectedEps?.Aggregate((r,n) => r + "\r\n" +n);
        }
    }
}
