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
        private int _decimalSeparatorSelectedIndex;

        private string _tickerStringOptionStrat = null!;
        private string _messageOptionStrat = null!;

        public ICommand LastEpsCommand { get; }
        public ICommand ExpectedEpsCommand { get; }
        public ICommand OptionsWarningsCommand { get; }
        public ViewModel(IYahooProvider investingProvider)
        {
            investingProvider.Status += InvestingProvider_Status;

            LastEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndex = 0;
                ResultListYahoo = new ObservableCollection<string>(await investingProvider.LastEpsAsync(TickerListYahoo, 5));
            });

            ExpectedEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndex = 0;
                ResultListYahoo = new ObservableCollection<string>(await investingProvider.ExpectedEpsAsync(TickerListYahoo, 5));
            });

            OptionsWarningsCommand = new RelayCommand(async () =>
            {
                MessageBox.Show("OptionsWarningsCommand");
                //ResultListOptionStrat= new ObservableCollection<string>(await investingProvider.ExpectedEpsAsync(TickerList, 5));
            });

            TickerStringYahoo = " SKX\r\nPFS\r\nSLCA\r\n WT";
        }

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

        public string TickerStringOptionStrat
        {
            get => _tickerStringOptionStrat;
            set
            {
                SetProperty(ref _tickerStringOptionStrat, value);
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

        public string MessageOptionStrat
        {
            get => _messageOptionStrat;
            set
            {
                SetProperty(ref _messageOptionStrat, value);
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
