using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBApi;
using IbClient.IbHost;
using IbClient.messages;
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
        const int TIMEOUT = 1000;

        IIbHostQueue ibHostQueue = null!;
        IIbHost ibHost = null!;

        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = new ObservableCollection<string>();
        private string _symbolsAsString;

        public ICommand ConnectToTwsCommand { get; }
        public ICommand SymbolCheckCommand { get; }

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

            SymbolCheckCommand = new RelayCommand(async () =>
            {
                //MessageBox.Show("SymbolCheckCommand");

                ibHost.Consumer = ibHost.Consumer ?? this;

                var symbolAndScoreArray = SymbolsAsString.Split(Environment.NewLine)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .ToList();
                var symbolAndScoreAsDictionary = symbolAndScoreArray
                    .Select(s => { 
                        var splitted = s.Split([' ', '\t']).Select(s=>s.Trim()).ToList();
                        if (splitted != null) return new KeyValuePair<string, int>(splitted[0], Convert.ToInt32(splitted[1]));
                        throw new Exception($"Unexpected. Can not build key value pair from the string {s}");
                    })
                    .ToDictionary();

                var resolved = new Dictionary<string, int>();
                var notResolved = new Dictionary<string, int>();
                var multiple = new Dictionary<string, int>();

                await Task.Run(async () => {
                    foreach (var symbol in symbolAndScoreAsDictionary.Keys)
                    {
                        SymbolSamplesMessage symbolSamplesMessage = await ibHost.RequestMatchingSymbolsAsync(symbol, TIMEOUT);
                        if (symbolSamplesMessage == null)
                        {
                            notResolved.Add(symbol, symbolAndScoreAsDictionary[symbol]);
                        }
                        else
                        {
                            if (symbolSamplesMessage.ContractDescriptions.Count() == 1)
                            {
                                resolved.Add(symbol, symbolAndScoreAsDictionary[symbol]);
                            }
                            else if (symbolSamplesMessage.ContractDescriptions.Count() == 0)
                            {
                                notResolved.Add(symbol, symbolAndScoreAsDictionary[symbol]);
                            }
                            else
                            {
                                multiple.Add(symbol, symbolAndScoreAsDictionary[symbol]);
                            }
                        }

                        Thread.Sleep(TIMEOUT * 2);
                    }

                });
                

                var t = 0;

            });

            SymbolsAsString = TestData();
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
        public string SymbolsAsString
        {
            get => _symbolsAsString;
            set
            {
                SetProperty(ref _symbolsAsString, value);
            }
        }

        private string TestData()
        {
            return @"PYPL	4
CRM	3
ABT	3
JNJ	3
MA	3
NOW	3
PG	3
AAPL	2
ABBV	2
ACN	2
ADBE	2
AMZN	2
BAC	2
BMY	2
CMCSA	2
CSCO	2
DIS	2
GOOGL	2
HD	2
INTC	2
JPM	2
KO	2
MRK	2
MSFT	2
NFLX	2
NVDA	2
PEP	2
SBUX	2
T	2
TSLA	2
UNH	2
UPS	2
V	2
VZ	2
WMT	2
AMD	2
COST	2
FB	2
FDX	2
GS	2
MCD	2
NKE	2
NOC	2
ORCL	2
QCOM	2
TGT	2
CI	1
LOW	1
TMO	1
TXN	1
ADP	1
AMGN	1
ANTM	1
BDX	1
BRK-A	1
BRK-B	1
CAT	1
CHTR	1
CVX	1
D	1
DE	1
DHR	1
HON	1
IBM	1
ICE	1
INTU	1
ISRG	1
LIN	1
LLY	1
LMT	1
MMM	1
MO	1
MU	1
NEE	1
PM	1
RIVN	1
ROKU	1
RTX	1
SO	1
SYK	1
UNP	1
VRTX	1
XOM	1
ZM	1
ZTS	1
AGN	1
ASAN	1
AVGO	1
BABA	1
BAND	1
BKNG	1
BLK	1
BNY	1
CCI	1
CVS	1
DDOG	1
DOCU	1
DUK	1
GLW	1
GM	1
GOEV	1
IBB	1
LCID	1
LYFT	1
MS	1
NIO	1
OKTA	1
PD	1
PFE	1
PINS	1
PLD	1
PLTR	1
SAP	1
SE	1
SHOP	1
SNOW	1
SPLK	1
SQ	1
TEAM	1
TMUS	1
TRLY	1
TTD	1
TWLO	1
TWTR	1
U	1
WFC	1
ZS	1
MDT	0
MMC	0
CRWD	0
F	0
FSLY	0
FUBO	0
HUM	0
JD	0
NET	0
SNAP	0
UBER	0
UPST	0
XPEV	0
DKNG	-1";
        }
    }
}
