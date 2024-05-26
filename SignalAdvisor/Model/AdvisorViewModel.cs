using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBApi;
using IbClient.IbHost;
using IbClient.messages;
using IBSampleApp.messages;
using SignalAdvisor.Commands;
using SignalAdvisor.Extensions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

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
        private string _lastCheck;
        private string _symbols;
        private ObservableCollection<PositionMessage> _positions = [];
        private ObservableCollection<KeyValuePair<string, List<Ta.Bar>>> _bars = [];
        private ObservableCollection<KeyValuePair<string, List<int>>> _signals = [];
        private ObservableCollection<Instrument> _instruments = [];
        private static System.Timers.Timer _timer;
        private static readonly string SESSION_START = "15:30";
        private Dispatcher _dispatcher = App.Current?.Dispatcher ?? throw new Exception("Unexpected. App.Current?.Dispatcher is null");

        ICommand RequestPositionsCommand;
        ICommand RequestHistoricalDataCommand;
        public ICommand UpdateSymbolsCommand { get; }
        public ICommand ConnectToTwsCommand { get; }

        public AdvisorViewModel()
        {
            RequestPositionsCommand = new RelayCommand(async () => await RequestPositions.RunAsync(this));
            RequestHistoricalDataCommand = new RelayCommand(async () => await RequestHistoricalData.RunAsync(this));
            ConnectToTwsCommand = new RelayCommand(() => ConnectToTws.Run(this));
            UpdateSymbolsCommand = new RelayCommand(() => UpdateSymbols.Run(this));

            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            OpenOrders = 7;
            LastCheck = "";


            // Test Data
            //Symbols = "NVDA\r\nMSFT";
            Symbols = TestData();
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // TODO the slower timer

            if (!Positions.Any())
            {
                _dispatcher.Invoke(() => TwsMessageCollection?.Add($"No positions."));
                return;
            }
            if (!RequestHistoricalDataExecuted)
            {
                _dispatcher.Invoke(() => TwsMessageCollection?.Add($"Retrieving historical data."));
                return;
            }
            _dispatcher.Invoke(() => { TwsMessageCollection?.Add($"Heartbeat"); });


            while (IbHost.QueueHistoricalDataUpdate.TryDequeue(out object message))
            {
                var liveDataMessage = message as LiveDataMessage ?? throw new Exception("Unexpected. liveDataMessage == null");

                AddBar(liveDataMessage.Contract, liveDataMessage.HistoricalDataMessage);

                var lastBar = Bars.For(liveDataMessage.Contract.ToString()).Last();
                var barBefore = Bars.For(liveDataMessage.Contract.ToString()).SkipLast(1).Last();

                if (lastBar.Time != barBefore.Time)
                    NewBar(liveDataMessage.Contract, lastBar.Time);
            }
        }

        public async Task StartUpAsync()
        {
            await Task.Run(() => { RequestPositionsCommand.Execute(this); });
            await Task.Run(() => { while (!RequestPositionsExecuted) { } });

            await Task.Run(() => { RequestHistoricalDataCommand.Execute(this); });
            await Task.Run(() => { while (!RequestHistoricalDataExecuted) { } });
        }

        private void NewBar(Contract contract, DateTimeOffset newBarTime)
        {
            LastCheck = newBarTime.ToString("HH:mm:ss");
            var contractString = contract.ToString();

            if (Ta.Signals.OppositeColor(forLongTrade: true, Bars.For(contractString), Signals.For(contractString)) != 0)
            {
                //SignalsAsText = $"{LastCheck} POSITION {contractString} OppositeColor {Environment.NewLine}{SignalsAsText}";
            }

            if (Ta.Signals.InsideUpDown(Bars.For(contractString), Signals.For(contractString)) != 0)
            {
                //SignalsAsText = $"{LastCheck} POSITION {contractString} InsideUpDown {Environment.NewLine}{SignalsAsText}";
            }
        }

        public IIbHost IbHost { get; private set; }

        public void SetIbHost(IIbHost ibHost)
        {
            IbHost = ibHost;
            IbHost.Consumer = this;
        }

        void IPositionsVisitor.OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public void AddBar(IBApi.Contract contract, HistoricalDataMessage message)
        {
            var bar = new Ta.Bar(message.Open, message.High, message.Low, message.Close, message.Date.DateTimeOffsetFromString());
            var contractOriginalString = contract.ToString();

            if (!Bars.Any(kvp => kvp.Key == contractOriginalString))
                Bars.Add(new KeyValuePair<string, List<Ta.Bar>>(contractOriginalString, []));
            Bars.First(kvp => kvp.Key == contractOriginalString).Value.Add(bar);
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

        public string LastCheck
        {
            get => _lastCheck;
            set
            {
                SetProperty(ref _lastCheck, value);
            }
        }


        public string Symbols
        {
            get => _symbols;
            set
            {
                SetProperty(ref _symbols, value);
            }
        }

        public ObservableCollection<PositionMessage> Positions
        {
            get => _positions;
            set
            {
                SetProperty(ref _positions, value);
            }
        }

        public ObservableCollection<KeyValuePair<string, List<Ta.Bar>>> Bars
        {
            get => _bars;
            set
            {
                SetProperty(ref _bars, value);
            }
        }

        public ObservableCollection<KeyValuePair<string, List<int>>> Signals
        {
            get => _signals;
            set
            {
                SetProperty(ref _signals, value);
            }
        }

        public ObservableCollection<Instrument> Instruments
        {
            get => _instruments;
            set
            {
                SetProperty(ref _instruments, value);
            }
        }


        public bool RequestPositionsExecuted { get; set; }
        public bool RequestHistoricalDataExecuted { get; set; }

        private string TestData()
        {
            return @"229612256	AIR	 1400 	 14 	0
29612111	BN	 420 	 5 	0

";
        }
    }
}
