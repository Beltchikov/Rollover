using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using PortfolioTrader.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PortfolioTrader.Model
{
    public class BuyConfirmationViewModel : ObservableObject, IIbConsumer, IBuyConfirmationModelVisitor
    {
        IIbHostQueue ibHostQueue = null!;
        IIbHost ibHost = null!;

        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = new ObservableCollection<string>();

        private int _investmentAmount = 0;
        private string _businessLogicInformation;
        private string _stocksWithoutPrice;
        private string _stocksWithoutMargin;
        private string _stocksToBuyAsString;
        private string _stocksToSellAsString;
        private string _ordersLongWithError;
        private string _ordersShortWithError;
        private bool _positionsCalculated;

        public ICommand ConnectToTwsCommand { get; }
        public ICommand CalculatePositionsCommand { get; }
        public ICommand SendLimitOrdersCommand { get; }
        public ICommand SendStopLimitOrdersCommand { get; }


        public BuyConfirmationViewModel()
        {
            IIbHostQueue ibHostQueue = new IbHostQueue();
            if (ibHostQueue != null) ibHost = new IbHost();
            ibHost.Consumer = ibHost.Consumer ?? this;

            ConnectToTwsCommand = new RelayCommand(() => ConnectToTws.Run(this));
            CalculatePositionsCommand = new RelayCommand(async () => await CalculatePositions.RunAsync(this));
            SendLimitOrdersCommand = new RelayCommand(async () => await SendLimitOrders.RunAsync(this));
            SendStopLimitOrdersCommand = new RelayCommand(async () => await SendStopLimitOrders.RunAsync(this));

            InvestmentAmount = 400000;
            BusinessLogicInformation = BuildBusinessLogicInformation();
        }

        private string BuildBusinessLogicInformation()
        {
            return @$"1. The number of stocks 
in the sell and buy list 
is reduced to {App.MAX_BUY_SELL}.

2. The number of stocks 
in the sell and buy list 
is kept equal.
";
        }

        public IIbHost IbHost => ibHost;
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

        public int InvestmentAmount
        {
            get => _investmentAmount;
            set
            {
                SetProperty(ref _investmentAmount, value);
            }
        }

        public string BusinessLogicInformation
        {
            get => _businessLogicInformation;
            set
            {
                SetProperty(ref _businessLogicInformation, value);
            }
        }

        public string StocksWithoutPrice
        {
            get => _stocksWithoutPrice;
            set
            {
                SetProperty(ref _stocksWithoutPrice, value);
            }
        }

        public string StocksWithoutMargin
        {
            get => _stocksWithoutMargin;
            set
            {
                SetProperty(ref _stocksWithoutMargin, value);
            }
        }

        public string StocksToBuyAsString
        {
            get => _stocksToBuyAsString;
            set
            {
                SetProperty(ref _stocksToBuyAsString, value);
            }
        }

        public string StocksToSellAsString
        {
            get => _stocksToSellAsString;
            set
            {
                SetProperty(ref _stocksToSellAsString, value);
            }
        }


        public string OrdersLongWithError
        {
            get => _ordersLongWithError;
            set
            {
                SetProperty(ref _ordersLongWithError, value);
            }
        }

        public string OrdersShortWithError
        {
            get => _ordersShortWithError;
            set
            {
                SetProperty(ref _ordersShortWithError, value);
            }
        }

        public bool PositionsCalculated
        {
            get => _positionsCalculated;
            set
            {
                SetProperty(ref _positionsCalculated, value);
            }
        }

        public void CalculateWeights()
        {
            if (!string.IsNullOrWhiteSpace(StocksToBuyAsString))
                StocksToBuyAsString = ConvertScoreToWeights(StocksToBuyAsString);
            if (!string.IsNullOrWhiteSpace(StocksToSellAsString))
                StocksToSellAsString = ConvertScoreToWeights(StocksToSellAsString);
        }

        public void ClearQueueOrderOpenMessage()
        {
            ibHost.ClearQueueOrderOpenMessage();
        }

        private static string ConvertScoreToWeights(string stocksAsString)
        {
            const int hundert = 100;
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var scaler = (double)stocksDictionary.Values.Select(p => p.NetBms).Sum() / 1d;
            var stocksDictionaryWithWeights = new Dictionary<string, Position>();
            foreach (var kvp in stocksDictionary)
            {
                stocksDictionaryWithWeights.Add(kvp.Key, new Position()
                {
                    NetBms = kvp.Value.NetBms,
                    ConId = kvp.Value.ConId,
                    Weight = (int)Math.Round(kvp.Value.NetBms * hundert / scaler, 2),
                    PriceInCents = kvp.Value.PriceInCents,
                    PriceType = kvp.Value.PriceType,
                    Quantity = kvp.Value.Quantity,
                    Margin = kvp.Value.Margin,
                });
            }

            var sumOfWeights = stocksDictionaryWithWeights.Values.Select(p => p.Weight).Sum();
            if (sumOfWeights < hundert)
            {
                var correction = hundert - sumOfWeights;

                var groupsDictionary = stocksDictionaryWithWeights
                    .Select(kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value.Weight == null ? 0 : kvp.Value.Weight.Value))
                    .GroupBy(a => a.Value)
                    .ToDictionary(grp => grp.Key, grp => grp.Count());

                var keyOfLargestGroup = groupsDictionary.MaxBy(entry => entry.Value).Key;
                var keyToApplyCorrection = stocksDictionaryWithWeights.Where(kvp => kvp.Value.Weight == keyOfLargestGroup)
                    .First()
                    .Key;

                stocksDictionaryWithWeights[keyToApplyCorrection].Weight += correction;
            }

            if (stocksDictionaryWithWeights.Values.Select(p => p.Weight).Sum() != hundert)
                throw new Exception($"Unexpected. Weights do not sum up to {hundert}");

            return SymbolsAndScore.PositionDictionaryToString(stocksDictionaryWithWeights);
        }
    }
}
