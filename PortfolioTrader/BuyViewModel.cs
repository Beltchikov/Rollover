using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PortfolioTrader
{
    public class BuyViewModel : ObservableObject, IIbConsumer
    {
        IIbHostQueue ibHostQueue = null!;
        IIbHost ibHost = null!;
       
        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = new ObservableCollection<string>();

        public ICommand ConnectToTwsCommand { get; }

        public BuyViewModel()
        {
            IIbHostQueue ibHostQueue = new IbHostQueue();
            if (ibHostQueue != null) ibHost = new IbHost(ibHostQueue);

            ConnectToTwsCommand = new RelayCommand(() =>
            {
                ibHost.Consumer = ibHost.Consumer ?? this;
                if (!ibHost.Consumer.ConnectedToTws)
                {
                    ibHost.ConnectAndStartReaderThread(Host, Port, ClientId, 1000);
                }
                else
                {
                    ibHost.Disconnect();
                }
            });
        }

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

        public ObservableCollection<string> TwsMessageCollection
        {
            get => _twsMessageColllection;
            set
            {
                SetProperty(ref _twsMessageColllection, value);
            }
        }


       

        

        

       

    }
}
