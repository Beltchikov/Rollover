using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dsmn.DataProviders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Dsmn
{
    public class ViewModel : ObservableObject
    {
        private string _tickerStringYahoo = null!;
        private ObservableCollection<string> _resultListYahoo = null!;
        private string _messageYahoo = null!;
        private int _decimalSeparatorSelectedIndexYahoo;

        private string _tickerStringOptionStrat = null!;
        private string _messageOptionStrat = null!;

        public ICommand LastEpsCommand { get; }
        public ICommand ExpectedEpsCommand { get; }
        public ICommand OptionsWarningsCommand { get; }
        public ViewModel(IYahooProvider yahooProvider)
        {
            yahooProvider.Status += YahooProvider_Status;

            LastEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndexYahoo = 0;
                ResultListYahoo = new ObservableCollection<string>(await yahooProvider.LastEpsAsync(TickerListYahoo, 5));
            });

            ExpectedEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndexYahoo = 0;
                ResultListYahoo = new ObservableCollection<string>(await yahooProvider.ExpectedEpsAsync(TickerListYahoo, 5));
            });

            OptionsWarningsCommand = new RelayCommand(async () =>
            {
                MessageBox.Show("OptionsWarningsCommand");
                //ResultListOptionStrat= new ObservableCollection<string>(await investingProvider.ExpectedEpsAsync(TickerList, 5));
            });

            TickerStringYahoo = " SKX\r\nPFS\r\nSLCA\r\n WT";
        }

        #region Yahoo

        public List<string> TickerListYahoo
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

        public ObservableCollection<string> ResultListYahoo
        {
            get => _resultListYahoo;
            set
            {
                SetProperty(ref _resultListYahoo, value);
                OnPropertyChanged(nameof(ResultStringYahoo));
            }
        }

        public string ResultStringYahoo
        {
            get
            {
                if (ResultListYahoo == null)
                {
                    return string.Empty;
                }
                else
                {
                    if (!ResultListYahoo.Any())
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return ResultListYahoo.Aggregate((r, n) => r + "\r\n" + n);
                    }
                }
            }
            set
            {
                SetProperty(ref _resultListYahoo, new ObservableCollection<string>());
                OnPropertyChanged(nameof(ResultListYahoo));
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

        public int DecimalSeparatorSelectedIndexYahoo
        {
            get => _decimalSeparatorSelectedIndexYahoo;
            set
            {
                SetProperty(ref _decimalSeparatorSelectedIndexYahoo, value);
            }
        }

        private void YahooProvider_Status(string message)
        {
            MessageYahoo = message;
        }

        #endregion  Yahoo

        #region OptionStrat

        public string TickerStringOptionStrat
        {
            get => _tickerStringOptionStrat;
            set
            {
                SetProperty(ref _tickerStringOptionStrat, value);
            }
        }
        
        public string MessageOptionStrat
        {
            get => _messageOptionStrat;
            set
            {
                SetProperty(ref _messageOptionStrat, value);
            }
        }

        #endregion OptionStrat
    }
}
