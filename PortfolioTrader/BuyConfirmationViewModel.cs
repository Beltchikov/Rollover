using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBApi;
using IbClient.IbHost;
using IbClient.messages;
using PortfolioTrader.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PortfolioTrader
{
    public class BuyConfirmationViewModel : ObservableObject, IIbConsumer, IBuyConfirmationModelVisitor
    {
        const int TIMEOUT = 1000;
        IIbHostQueue ibHostQueue = null!;
        IIbHost ibHost = null!;

        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = new ObservableCollection<string>();

        private int _investmentAmount = 0;
        private string _businessLogicInformation;
        private string _tooExpensiveStocks;
        private string _stocksToBuyAsString;
        private string _stocksToSellAsString;

        public ICommand ConnectToTwsCommand { get; }

        public BuyConfirmationViewModel()
        {
            IIbHostQueue ibHostQueue = new IbHostQueue();
            if (ibHostQueue != null) ibHost = new IbHost(ibHostQueue);
            ibHost.Consumer = ibHost.Consumer ?? this;

            ConnectToTwsCommand = new RelayCommand(() => ConnectToTws.Run(this));

            InvestmentAmount = 100000;
            BusinessLogicInformation = "TODO";
            TooExpensiveStocks = "TODO 2";
            StocksToBuyAsString = "TODO 3";
            StocksToBuyAsString = "TODO 4";
        }

        public IIbHost IbHost => ibHost;
        public int Timeout => TIMEOUT;

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

        public string TooExpensiveStocks
        {
            get => _tooExpensiveStocks;
            set
            {
                SetProperty(ref _tooExpensiveStocks, value);
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
        




    }
}
