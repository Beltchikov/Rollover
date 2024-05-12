using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using SignalAdvisor.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SignalAdvisor.Model
{
    public class AdvisorViewModel : ObservableObject, IIbConsumer, ITwsVisitor
    {
        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = new ObservableCollection<string>();
        private int _openPositions;
        private int _openOrders;
        private int _lastCheck;

        public ICommand RequestPositionsCommand { get; }


        public AdvisorViewModel()
        {
            //IIbHostQueue ibHostQueue = new IbHostQueue();

            RequestPositionsCommand = new RelayCommand(() => RequestPositions.Run(this));

            OpenPositions = 21;
            OpenOrders = 7;
            LastCheck = 0;
        }

        public void StartUp()
        {
            RequestPositionsCommand.Execute(this);
        }

        public IIbHost IbHost { get; private set; }

        public void SetIbHost(IIbHost ibHost)
        {
            IbHost = ibHost;
            IbHost.Consumer = this;
        }

        public int Timeout => App.TIMEOUT;

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

        public int OpenPositions
        {
            get => _openPositions;
            set
            {
                SetProperty(ref _openPositions, value);
            }
        }

        public int OpenOrders
        {
            get => _openOrders;
            set
            {
                SetProperty(ref _openOrders, value);
            }
        }

        public int LastCheck
        {
            get => _lastCheck;
            set
            {
                SetProperty(ref _lastCheck, value);
            }
        }
    }
}
