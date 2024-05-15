﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using IbClient.messages;
using IBSampleApp.messages;
using SignalAdvisor.Commands;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Threading;
using Ta;

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
        private string _signalsAsText;
        private ObservableCollection<PositionMessage> _positions = [];
        private ObservableCollection<KeyValuePair<string, List<Bar>>> _bars = [];
        private ObservableCollection<KeyValuePair<string, List<int>>> _signals = [];
        private static System.Timers.Timer _timer;
        private static readonly string SESSION_START = "15:30";
        private Dispatcher _dispatcher = App.Current?.Dispatcher ?? throw new Exception("Unexpected. App.Current?.Dispatcher is null");

        ICommand RequestPositionsCommand;
        ICommand RequestHistoricalDataCommand;

        public AdvisorViewModel()
        {
            RequestPositionsCommand = new RelayCommand(async () => await RequestPositions.RunAsync(this));
            RequestHistoricalDataCommand = new RelayCommand(async () => await RequestHistoricalData.RunAsync(this));

            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            OpenOrders = 7;
            LastCheck = "";
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // TODO the slower timer

            if (!Positions.Any())
            {
                _dispatcher.Invoke(() => TwsMessageCollection?.Add($"No positions."));
                return;
            }
            _dispatcher.Invoke(() => { TwsMessageCollection?.Add($"Heartbeat"); });


            while (IbHost.QueueHistoricalDataUpdate.TryDequeue(out object message))
            {
                var liveDataMessage = message as LiveDataMessage ?? throw new Exception("Unexpected. liveDataMessage == null");

                AddBar(liveDataMessage.Contract, liveDataMessage.HistoricalDataMessage);

                var lastBar = Bars.First(kvp => kvp.Key == liveDataMessage.Contract.ToString()).Value.Last();
                // TODO Indexe als extension method: Bars[0]
                var barBefore = Bars.First(kvp => kvp.Key == liveDataMessage.Contract.ToString()).Value.SkipLast(1).Last();

                if (lastBar.Time != barBefore.Time) NewBar(liveDataMessage.Contract.Symbol, lastBar.Time);
            }
        }

        public async Task StartUpAsync()
        {
            await Task.Run(() => { RequestPositionsCommand.Execute(this); });
            await Task.Run(() => { while (!RequestPositionsExecuted) { } });

            await Task.Run(() => { RequestHistoricalDataCommand.Execute(this); });
            await Task.Run(() => { while (!RequestHistoricalDataExecuted) { } });
        }

        private void NewBar(string symbol, DateTimeOffset newBarTime)
        {
            LastCheck = newBarTime.ToString("HH:mm:ss");

            if (Ta.Signals.InsideUpDown(Bars.First(kvp => kvp.Key == symbol).Value, Signals.First(kvp => kvp.Key == symbol).Value) != 0)
            {
                // TODO
            }


            string[] symbols;
            if (InsideUpDown(out symbols))
            {
                foreach (var s in symbols)
                {
                    SignalsAsText = $"{LastCheck} POSITION {s} InsideUpDown {Environment.NewLine}{SignalsAsText}";
                }
            }
        }

        private bool InsideUpDown(out string[] symbols)
        {
            symbols = ["TEST", "RRR"];
            return true;

            //foreach (var kvp in Bars)
            //{
            //    var barData = kvp.Value;
            //    if (InsideUpDown(barData))
            //    {

            //    }
            //}
        }

        //private bool InsideUpDown(List<Bar> bars)
        //{
        //    var sessionStart = DateTime.Now;
        //    sessionStart = sessionStart.Date;
        //    sessionStart = sessionStart.AddHours(15);
        //    sessionStart = sessionStart.AddMinutes(30);

        //    for (int i = bars.Count - 1; i >= 0; i--)
        //    {
        //        var bar = bars[i];

        //        for (int ii = i - 1; ii >= 0; ii--)
        //        {
        //            var barBefore = bars[ii];
        //            if (barBefore.Time < sessionStart)
        //                return false;

        //            if(bar.High <= barBefore)
        //        }
        //    }
        //}

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
            var bar = new Bar(message.Open, message.High, message.Low, message.Close, DateTimeOffsetFromString(message.Date));
            var contractOriginalString = contract.ToString();

            if (!Bars.Any(kvp => kvp.Key == contractOriginalString))
                Bars.Add(new KeyValuePair<string, List<Bar>>(contractOriginalString, []));
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

        public string SignalsAsText
        {
            get => _signalsAsText;
            set
            {
                SetProperty(ref _signalsAsText, value);
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

        public ObservableCollection<KeyValuePair<string, List<Bar>>> Bars
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


        public bool RequestPositionsExecuted { get; set; }
        public bool RequestHistoricalDataExecuted { get; set; }

        private static DateTimeOffset DateTimeOffsetFromString(string dateTimeOffsetString)
        {
            var dtoStringSplitted = dateTimeOffsetString
                          .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => s.Trim())
                          .ToArray();
            var timeZoneString = dtoStringSplitted[2];
            var dateTimeString = dtoStringSplitted
                .Take(2)
                .Aggregate((r, n) => r + " " + n);

            var dateTime = DateTime.ParseExact(dateTimeString, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
            var result = new DateTimeOffset(dateTime, timeZoneInfo.BaseUtcOffset);

            return result;
        }
    }
}
