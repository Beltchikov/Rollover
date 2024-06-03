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
        private string _lastCheck = "";
        private string _symbols = "";
        private string _symbolsShort = "";
        private ObservableCollection<PositionMessage> _positions = [];
        private ObservableCollection<KeyValuePair<string, List<Ta.Bar>>> _bars = [];
        private ObservableCollection<KeyValuePair<string, List<Dictionary<string, int>>>> _signals = [];
        private ObservableCollection<Instrument> _instruments = [];
        private static System.Timers.Timer _timer = null!;
        private Dispatcher _dispatcher = App.Current?.Dispatcher ?? throw new Exception("Unexpected. App.Current?.Dispatcher is null");

        ICommand RequestPositionsCommand;
        ICommand RequestHistoricalDataCommand;
        public ICommand ConnectToTwsCommand { get; }
        public ICommand UpdateSymbolsCommand { get; }
        public ICommand SendOrdersCommand { get; }
        public ICommand SendNonBracketOrdersCommand { get; }
        public ICommand SendOrders2StdDevCommand { get; }

        public AdvisorViewModel()
        {
            RequestPositionsCommand = new RelayCommand(async () => await RequestPositions.RunAsync(this));
            RequestHistoricalDataCommand = new RelayCommand(async () => await RequestHistoricalData.RunAsync(this));
            ConnectToTwsCommand = new RelayCommand(() => ConnectToTws.Run(this));
            UpdateSymbolsCommand = new RelayCommand(() => UpdateSymbols.Run(this));
            SendOrdersCommand = new RelayCommand(async () => await SendOrders.RunAsync(this));
            SendNonBracketOrdersCommand = new RelayCommand(async () => await SendNonBracketOrders.RunAsync(this));
            SendOrders2StdDevCommand = new RelayCommand(async () => await SendOrders2StdDev.RunAsync(this));

            _timer = new System.Timers.Timer(60000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            OpenOrders = 7;
            LastCheck = "";
            IbHost = null!;
            InstrumentToTrade = null!;

            // Test Data
            //Symbols = TestDataEu();
            //Symbols = TestDataUs();
            Symbols = TestDataUs2();
        }

        public IIbHost IbHost { get; private set; }
        public void SetIbHost(IIbHost ibHost)
        {
            IbHost = ibHost;
            IbHost.Consumer = this;
        }
        public async Task StartUpAsync()
        {
            await Task.Run(() => { RequestPositionsCommand.Execute(this); });
            await Task.Run(() => { while (!RequestPositionsExecuted) { } });

            await Task.Run(() => { RequestHistoricalDataCommand.Execute(this); });
            await Task.Run(() => { while (!RequestHistoricalDataExecuted) { } });
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

        public string SymbolsShort
        {
            get => _symbolsShort;
            set
            {
                SetProperty(ref _symbolsShort, value);
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

        public ObservableCollection<KeyValuePair<string, List<Dictionary<string, int>>>> Signals
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
        public Instrument InstrumentToTrade { get; set; }
        public int OrdersSent { get; set; }

        public void TickPriceCallback(TickPriceMessage message)
        {
            var askPriceTickType = 2;
            if (message.Field != askPriceTickType) return;

            var instrument = Instruments.FirstOrDefault(i => i.RequestIdMktData == message.RequestId);
            if (instrument != null)
            {
                instrument.AskPrice = message.Price;
            }
        }

        private void NewBar(Contract contract, DateTimeOffset newBarTime)
        {
            LastCheck = newBarTime.ToString("HH:mm:ss");
            var contractString = contract.ToString();

            //if (Ta.Signals.OppositeColor(forLongTrade: true, Bars.For(contractString), Signals.For(contractString)) != 0)
            //{
            //    //SignalsAsText = $"{LastCheck} POSITION {contractString} OppositeColor {Environment.NewLine}{SignalsAsText}";
            //}

            //if (Ta.Signals.InsideUpDown(Bars.For(contractString), Signals.For(contractString)) != 0)
            //{
            //    //SignalsAsText = $"{LastCheck} POSITION {contractString} InsideUpDown {Environment.NewLine}{SignalsAsText}";
            //}
        }

        void IPositionsVisitor.OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
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
            //_dispatcher.Invoke(() => { TwsMessageCollection?.Add($"Heartbeat"); });


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

        private string TestDataEu()
        {
            return @"29612256	AIR	EUR	SBF	1400	14	190
29612111	BN	EUR	SBF	420	5	500

";
        }

        private string TestDataUs()
        {
            return @"4815747	NVDA	USD	NASDAQ	10652	107	88
265598	AAPL	USD	NASDAQ	1264	13	744
107113386	META	USD	NASDAQ	4539	46	207
272093	MSFT	USD	NASDAQ	2512	26	374
15124833	NFLX	USD	NASDAQ	6408	65	147
265768	ADBE	USD	NASDAQ	3797	38	248
208813720	GOOG	USD	NASDAQ	1225	13	767
";
        }

        private string TestDataUs2()
        {
            return @"265598	AAPL	USD	SMART	1272	13	315
266093	AMAT	USD	SMART	1697	17	240
313130367	AVGO	USD	SMART	7677	77	52
271308	LRCX	USD	SMART	7711	78	52
107113386	META	USD	SMART	4502	46	90
272093	MSFT	USD	SMART	2506	26	160
15124833	NFLX	USD	SMART	6486	65	62
4815747	NVDA	USD	SMART	11488	115	35
199169591	PYPL	USD	SMART	529	6	670


";
        }

    }
}
