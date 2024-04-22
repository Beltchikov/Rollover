using PortalOpener.Opener;
using System.Reflection;
using System.Text;

namespace PortalOpener
{
    public partial class FormMain : Form
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string[] openerNames;

        public FormMain()
        {
            InitializeComponent();

            LoadTestData();

            Type[] openerTypes = executingAssembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IOpener)))
                .ToArray();
            openerNames = openerTypes.Select(t => t.FullName == null? "NULL" : t.FullName).ToArray();

            cmbOpener.Items.AddRange(openerNames);
            cmbOpener.SelectedItem = openerNames.First(n => n.EndsWith("SeekingAlphaOpener"));
        }

        private void LoadTestData()
        {
            txtSymbols.Text = @"PYPL
CRM
ABT
JNJ
MA
NOW
PG
AAPL
ABBV
ACN
ADBE
AMZN
BAC
BMY
CMCSA
CSCO
DIS
GOOGL
HD
INTC
JPM
KO
MRK
MSFT
NFLX
NVDA
PEP
SBUX
T
TSLA
UNH
UPS
V
VZ
WMT
AMD
COST
FB
FDX
GS
MCD
NKE
NOC
ORCL
QCOM
TGT
CI
LOW
TMO
TXN
ADP
AMGN
ANTM
BDX
BRK-A
BRK-B
CAT
CHTR
CVX
D
DE
DHR
HON
IBM
ICE
INTU
ISRG
LIN
LLY
LMT
MMM
MO
MU
NEE
PM
RIVN
ROKU
RTX
SO
SYK
UNP
VRTX
XOM
ZM
ZTS
AGN
ASAN
AVGO
BABA
BAND
BKNG
BLK
BNY
CCI
CVS
DDOG
DOCU
DUK
GLW
GM
GOEV
IBB
LCID
LYFT
MS
NIO
OKTA
PD
PFE
PINS
PLD
PLTR
SAP
SE
SHOP
SNOW
SPLK
SQ
TEAM
TMUS
TRLY
TTD
TWLO
TWTR
U
WFC
ZS
MDT
MMC
CRWD
F
FSLY
FUBO
HUM
JD
NET
SNAP
UBER
UPST
XPEV
DKNG
";
        }

        private void btGo_Click(object sender, EventArgs e)
        {
            var assemblyName = executingAssembly.FullName;
            if (assemblyName == null) throw new Exception("Unexpected! assemblyName is null");

            var selectedItem = cmbOpener.SelectedItem;
            if (selectedItem == null) throw new Exception("Unexpected! selectedItem is null");

            var selectedOpenerFullName = selectedItem.ToString();
            if (selectedOpenerFullName == null) throw new Exception("Unexpected! selectedOpenerFullName is null");

            var openerWraped = Activator.CreateInstance(assemblyName, selectedOpenerFullName.ToString());
            if (openerWraped == null) throw new Exception("Unexpected! openerWraped is null");

            var openerUnwrapped = openerWraped.Unwrap();
            if (openerUnwrapped == null) throw new Exception("Unexpected! openerUnwrapped is null");

            var opener = (IOpener)openerUnwrapped;
            var openerResult = opener.Execute(new string[1]);

        }
    }
}
