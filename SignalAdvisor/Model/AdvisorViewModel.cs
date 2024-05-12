using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using SignalAdvisor.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SignalAdvisor.Model
{
    public class AdvisorViewModel : ObservableObject, IIbConsumer, ITwsVisitor, IPositionsVisitor
    {
        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = [];
        private int _openOrders;
        private int _lastCheck;
        private ObservableCollection<string> _positions = [];

        public AdvisorViewModel()
        {
            //RequestPositionsCommand = new RelayCommand(() => RequestPositions.Run(this));

            OpenOrders = 7;
            LastCheck = 0;
        }

        public async Task StartUpAsync()
        {
            //RequestPositionsCommand.Execute(this);

            bool positionsRequested = false;
            await IbHost.RequestPositions(
               (p) =>
               {
                   Positions.Add(p.Contract.Symbol);
               },
               () =>
               {
                   positionsRequested = true;
               });

            await Task.Run(() => { while (!positionsRequested) { }; });
            OnPropertyChanged(nameof(Positions));
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

        public ObservableCollection<string> Positions
        {
            get => _positions;
            set
            {
                SetProperty(ref _positions, value);
            }
        }
    }
}
