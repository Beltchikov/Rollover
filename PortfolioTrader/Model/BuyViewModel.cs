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

namespace PortfolioTrader.Model
{
    public class BuyViewModel : ObservableObject, IIbConsumer, IBuyModelVisitor
    {
        IIbHostQueue ibHostQueue = null!;
        IIbHost ibHost = null!;

        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = new ObservableCollection<string>();
        private string _longSymbolsAsString;
        private string _longSymbolsResolved;
        private string _longSymbolsUnresolved;
        private string _longSymbolsMultiple;
        private string _shortSymbolsAsString;
        private string _shortSymbolsResolved;
        private string _shortSymbolsUnresolved;
        private string _shortSymbolsMultiple;
        private bool _symbolsChecked;

        public ICommand ConnectToTwsCommand { get; }
        public ICommand SymbolCheckCommand { get; }
        public ICommand OrderConfirmationCommand { get; }


        public BuyViewModel()
        {
            IIbHostQueue ibHostQueue = new IbHostQueue();
            if (ibHostQueue != null) ibHost = new IbHost();
            ibHost.Consumer = ibHost.Consumer ?? this;

            ConnectToTwsCommand = new RelayCommand(() => ConnectToTws.Run(this));
            SymbolCheckCommand = new RelayCommand(async () =>
                {
                    if (!ConnectedToTws) ConnectToTwsCommand?.Execute(null);
                    await SymbolCheck.Run(this);
                });
            OrderConfirmationCommand = new RelayCommand(() => Commands.OrderConfirmationCommand.Run(this));

            //LongSymbolsAsString = TestDataLong();
            //LongSymbolsAsString = TestDataLongRepository();
            //LongSymbolsAsString = TestDataLongNotTradeable();
            LongSymbolsAsString = TestDataLong10Symbols();
            //LongSymbolsAsString = TestDataLongNotResolved();
            
            //ShortSymbolsAsString = TestDataShort();
            //ShortSymbolsAsString = TestDataShortRepository();
            //ShortSymbolsAsString = TestDataShortNotTradeable();
            ShortSymbolsAsString = TestDataShort10Symbols();
            //ShortSymbolsAsString = TestDataShortNotResolved();
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
        public string LongSymbolsAsString
        {
            get => _longSymbolsAsString;
            set
            {
                SetProperty(ref _longSymbolsAsString, value);
            }
        }

        public string LongSymbolsResolved
        {
            get => _longSymbolsResolved;
            set
            {
                SetProperty(ref _longSymbolsResolved, value);
            }
        }

        public string LongSymbolsUnresolved
        {
            get => _longSymbolsUnresolved;
            set
            {
                SetProperty(ref _longSymbolsUnresolved, value);
            }
        }

        public string LongSymbolsMultiple
        {
            get => _longSymbolsMultiple;
            set
            {
                SetProperty(ref _longSymbolsMultiple, value);
            }
        }

        public string ShortSymbolsAsString
        {
            get => _shortSymbolsAsString;
            set
            {
                SetProperty(ref _shortSymbolsAsString, value);
            }
        }

        public string ShortSymbolsResolved
        {
            get => _shortSymbolsResolved;
            set
            {
                SetProperty(ref _shortSymbolsResolved, value);
            }
        }

        public string ShortSymbolsUnresolved
        {
            get => _shortSymbolsUnresolved;
            set
            {
                SetProperty(ref _shortSymbolsUnresolved, value);
            }
        }

        public string ShortSymbolsMultiple
        {
            get => _shortSymbolsMultiple;
            set
            {
                SetProperty(ref _shortSymbolsMultiple, value);
            }
        }

        public bool SymbolsChecked
        {
            get => _symbolsChecked;
            set
            {
                SetProperty(ref _symbolsChecked, value);
            }
        }

        private string TestDataLong()
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

        private string TestDataLong15Symbols()
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
";
        }

        private string TestDataLong10Symbols()
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
";
        }

        private string TestDataLong6Symbols()
        {
            return @"PYPL	4
CRM	3
ABT	3
JNJ	3
MA	3
NOW	3

";
        }

        private string TestDataLong4Symbols()
        {
            return @"PYPL	4
CRM	3
ABT	3
JNJ	3

";
        }

        private string TestDataLongRepository()
        {
            return @"PYPL	4
NVDA	3
MSFT	3
AMZN	3
GOOG	3
NOW	3

";
        }

        private string TestDataLongNotTradeable()
        {
            return @"NVDA	3
IDIG	3
ARRT	3
GOOG	3

";
        }

        private string TestDataLongNotResolved()
        {
            return @"CSCO	3
CS	3
";
        }

        private string TestDataShort()
        {
            return @"MATIC	4
SOS	3
YFI	3
1INCH	2
AAVE	2
ADA	2
ALGO	2
ATOM	2
AVAX	2
AXS	2
BNB	2
BTC	2
CAKE	2
COMP	2
DOGE	2
DOT	2
EOS	2
ETH	2
FIL	2
FSR	2
FTT	2
JMIA	2
LINK	2
LTC	2
LUNA	2
NEAR	2
NKLA	2
REN	2
RIDE	2
RUNE	2
SAND	2
SHIB	2
SNX	2
SOL	2
SUSHI	2
THETA	2
TRX	2
UNI	2
VET	2
WKHS	2
XLM	2
DKNG	1
AAL	1
ACB	1
AMC	1
ANKR	1
APHA	1
BAL	1
BB	1
BBBY	1
BCH	1
BLNK	1
BNGO	1
BTT	1
BYND	1
CCIV	1
CCL	1
CHZ	1
CLNE	1
COIN	1
CRO	1
CRSP	1
CRSR	1
CRV	1
DAL	1
DASH	1
EGLD	1
EH	1
ENJ	1
ENPH	1
ETC	1
EXPR	1
FCEL	1
GE	1
GME	1
GNO	1
GRT	1
GRWG	1
GSAT	1
HEXO	1
HNT	1
HT	1
HUYA	1
HYLN	1
ICP	1
IOST	1
IQ	1
KHC	1
KNC	1
KNDI	1
KSM	1
LAZR	1
LI	1
LVS	1
MANA	1
MGM	1
MKR	1
MRNA	1
MRO	1
MVIS	1
NCLH	1
NOK	1
OCGN	1
OXY	1
PDD	1
PENN	1
PLUG	1
PSTH	1
PTON	1
QS	1
QTUM	1
RCL	1
RIOT	1
RKT	1
RMO	1
RSR	1
SAVE	1
SKLZ	1
SNDL	1
SPCE	1
SPG	1
TFUEL	1
TIGR	1
TLRY	1
UAL	1
UWMC	1
VIAC	1
VXRT	1
WATT	1
WAVES	1
WISH	1
WORK	1
WYNN	1
X	1
XMR	1
ZRX	1
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
CI	-1
LOW	-1
TMO	-1
TXN	-1
ADP	-1
AMGN	-1
ANTM	-1
BDX	-1
BRK-A	-1
BRK-B	-1
CAT	-1
CHTR	-1
CVX	-1
D	-1
DE	-1
DHR	-1
HON	-1
IBM	-1
ICE	-1
INTU	-1
ISRG	-1
LIN	-1
LLY	-1
LMT	-1
MMM	-1
MO	-1
MU	-1
NEE	-1
PM	-1
RIVN	-1
ROKU	-1
RTX	-1
SO	-1
SYK	-1
UNP	-1
VRTX	-1
XOM	-1
ZM	-1
ZTS	-1
AAPL	-2
ABBV	-2
ACN	-2
ADBE	-2
AMZN	-2
BAC	-2
BMY	-2
CMCSA	-2
CSCO	-2
DIS	-2
GOOGL	-2
HD	-2
INTC	-2
JPM	-2
KO	-2
MRK	-2
MSFT	-2
NFLX	-2
NVDA	-2
PEP	-2
SBUX	-2
T	-2
TSLA	-2
UNH	-2
UPS	-2
V	-2
VZ	-2
WMT	-2
CRM	-3
PYPL	-4";
        }

        private string TestDataShort15Symbols()
        {
            return @"MATIC	4
SOS	3
YFI	3
1INCH	2
AAVE	2
ADA	2
ALGO	2
ATOM	2
AVAX	2
AXS	2
BNB	2
BTC	2
CAKE	2
COMP	2
DOGE	2
";
        }

        private string TestDataShort10Symbols()
        {
            return @"MATIC	4
SOS	3
YFI	3
1INCH	2
AAVE	2
ADA	2
ALGO	2
ATOM	2
AVAX	2
AXS	2
";
        }

        private string TestDataShort6Symbols()
        {
            return @"MATIC	4
SOS	3
YFI	3
1INCH	2
AAVE	2
ADA	2

";
        }

        private string TestDataShort4Symbols()
        {
            return @"MATIC	4
SOS	3
YFI	3
1INCH	2
";
        }

        private string TestDataShortRepository()
        {
            return @"MATIC	4
AAPL	3
TSLA	3
IBM	2
CAT	2
ADA	2

";
        }

        private string TestDataShortNotTradeable()
        {
            return @"AAPL	3
ATWT	3
AAVE	2
CAT	2


";
        }

        private string TestDataShortNotResolved()
        {
            return @"TWTR	3
LILM	3

";
        }

    }
}
