using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dsmn.DataProviders;
using Dsmn.Ib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Dsmn
{
    public class ViewModel : ObservableObject, IIbConsumer
    {
        private string _tickerStringYahoo = null!;
        private ObservableCollection<string> _resultListYahoo = null!;
        private string _messageYahoo = null!;
        private int _decimalSeparatorSelectedIndexYahoo;

        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private List<string>? _messages;

        public ICommand LastEpsCommand { get; }
        public ICommand ExpectedEpsCommand { get; }
        public ICommand ConnectToTwsCommand { get; }
        public ViewModel(IYahooProvider yahooProvider, IIbHost ibHost)
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

            ConnectToTwsCommand = new RelayCommand(() =>
            {
                ibHost.Consumer = this;
                ibHost.ConnectAndStartReaderThread(Host, Port, ClientId, 1000);
                //MessageBox.Show("ConnectToTwsCommand");
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

        #region TWS

        public string Host
        {
            get => _host;
            set
            {
                SetProperty(ref _host, value);
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                SetProperty(ref _port, value);
            }
        }

        public int ClientId
        {
            get => _clientId;
            set
            {
                SetProperty(ref _clientId, value);
            }
        }

        public bool ConnectedToTws
        {
            get => _connectedToTws;
            set
            {
                SetProperty(ref _connectedToTws, value);
            }
        }

        public List<string>? Messages
        {
            get => _messages;
            set
            {
                SetProperty(ref _messages, value);
            }
        }

        //bool IIbConsumer.ConnectedToTws { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //List<string>? IIbConsumer.Messages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        #endregion TWS
    }
}
