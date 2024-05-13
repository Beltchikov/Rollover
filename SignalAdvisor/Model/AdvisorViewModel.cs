using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using IbClient.messages;
using SignalAdvisor.Commands;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
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
        private ObservableCollection<PositionMessage> _positions = [];
        private ObservableCollection<KeyValuePair<string, List<Bar>>> _bars = [];

        ICommand RequestPositionsCommand;
        ICommand RequestHistoricalDataCommand;

        public AdvisorViewModel()
        {
            RequestPositionsCommand = new RelayCommand(async () => await RequestPositions.RunAsync(this));
            RequestHistoricalDataCommand = new RelayCommand(async () => await RequestHistoricalData.RunAsync(this));

            OpenOrders = 7;
            LastCheck = "";
        }

        public async Task StartUpAsync()
        {
            await Task.Run(() => { RequestPositionsCommand.Execute(this); });
            await Task.Run(() => { while (!RequestPositionsExecuted) { } });

            await Task.Run(() => { RequestHistoricalDataCommand.Execute(this); });
            await Task.Run(() => { while (!RequestHistoricalDataExecuted) { } });

            new Thread(() =>
            {
                while (IbHost.QueueHistoricalDataUpdate.TryDequeue(out object message))
                {
                    var liveDataMessage = message as LiveDataMessage;
                    if (liveDataMessage == null) throw new Exception("Unexpected. liveDataMessage == null");
                        
                    AddBar(liveDataMessage.Contract, liveDataMessage.HistoricalDataMessage);
                    var lastBar = Bars.First(kvp => kvp.Key == liveDataMessage.Contract.ToString()).Value.Last();
                    var barBefore = Bars.First(kvp => kvp.Key == liveDataMessage.Contract.ToString()).Value.SkipLast(1).Last();
                    
                    if (lastBar.Time != barBefore.Time) NewBar(lastBar.Time);

                }
            })
            { IsBackground = true }
            .Start();
        }

        private void NewBar(DateTime newBarTime)
        {
            LastCheck = newBarTime.ToString();
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
            var bar = new Bar(message.Open, message.High, message.Low, message.Close, TimeFromString(message.Date));
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

        public bool RequestPositionsExecuted { get; set; }
        public bool RequestHistoricalDataExecuted { get; set; }

        private static DateTime TimeFromString(string dateTimeString)
        {
            var timeString = dateTimeString
                         .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                         .Select(s => s.Trim())
                         .ToArray()
                         .Aggregate((r, n) => r + " " + n);

            return DateTime.ParseExact(timeString, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
