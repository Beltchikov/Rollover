using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using IBSampleApp.messages;
using NpvManager.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NpvManager.Model
{
    public class NpvManagerViewModel : ObservableObject, IIbConsumer, IPositionsVisitor
    {

        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = [];
        private ObservableCollection<PositionMessage> _positions = [];

        public ICommand ConnectToTwsCommand { get; }
        public ICommand LoadOrdersCommand { get; }
        public ICommand LoadPositionsCommand { get; }

        public NpvManagerViewModel()
        {
            IbHost = null!;
            SetIbHost(new IbHost());

            ConnectToTwsCommand = new RelayCommand(() => ConnectToTws.Run(this));
            LoadOrdersCommand = new RelayCommand(() => LoadOrders.Run(this));
            LoadPositionsCommand = new RelayCommand(async () => await LoadPositions.RunAsync(this));
        }

        public IIbHost IbHost { get; private set; }
        public void SetIbHost(IIbHost ibHost)
        {
            IbHost = ibHost;
            IbHost.Consumer = this;
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

        public int Timeout => App.TIMEOUT;

        public ObservableCollection<PositionMessage> Positions
        {
            get => _positions;
            set
            {
                SetProperty(ref _positions, value);
            }
        }
    }
}
