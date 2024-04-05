using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Prototype
{
    public partial class FormMain : Form
    {
        private EReaderMonitorSignal signal;
        private IBClient ibClient;
        int _activeReqId = 700;
        int _broadNewsReqId = 800;
        int _nextOrderId = 0;

        public const int RT_BARS_ID_BASE = 40000000;

        FormContractId _formContractId;
        private int reqScannerSubscription;

        public FormMain()
        {
            InitializeComponent();

            EnableControls(false);

            signal = new EReaderMonitorSignal();
            ibClient = new IBClient(signal);

            ibClient.Error += OnError;
            ibClient.NextValidId += OnNextValidId;
            ibClient.ManagedAccounts += OnManagedAccounts;
            ibClient.Position += OnPosition;
            ibClient.PositionEnd += OnPositionEnd;
            ibClient.SymbolSamples += OnSymbolSamples;
            ibClient.SecurityDefinitionOptionParameter += OnSecurityDefinitionOptionParameter;
            ibClient.SecurityDefinitionOptionParameterEnd += OnSecurityDefinitionOptionParameterEnd;
            ibClient.TickPrice += OnTickPrice;
            ibClient.TickSize += OnTickSize;
            ibClient.TickString += TickString;
            ibClient.TickGeneric += OnTickGeneric;
            ibClient.ContractDetails += OnContractDetails;
            ibClient.OpenOrder += OnOpenOrder;
            ibClient.OpenOrderEnd += OnOpenOrderEnd;
            ibClient.OrderStatus += OnOrderStatus;
            ibClient.ExecDetails += OnExecDetails;
            ibClient.ExecDetailsEnd += OnExecDetailsEnd;
            ibClient.ScannerData += OnScannerData;
            ibClient.ScannerDataEnd += OnScannerDataEnd;
            ibClient.ScannerParameters += OnScannerParameters;
            ibClient.TickNews += IbClient_TickNews;
        }

        private void OnOpenOrder(OpenOrderMessage obj)
        {
            var msg = $"OnOpenOrder: MaintMarginChange:{obj.OrderState.MaintMarginChange} " +
                $"Commission:{obj.OrderState.Commission}";
            AddLineToTextbox(txtMessage, msg);
        }

        private void OnOpenOrderEnd()
        {
            var msg = $"OnOpenOrderEnd: ";
            AddLineToTextbox(txtMessage, msg);
        }

        private void OnOrderStatus(OrderStatusMessage obj)
        {
            var msg = $"OnOrderStatus: Status:{obj.Status} " +
               $"Filled:{obj.Filled} Remaining:{obj.Remaining}";
            AddLineToTextbox(txtMessage, msg);
        }

        private void OnExecDetailsEnd(int obj)
        {
            var msg = $"OnExecDetailsEnd: ";
            AddLineToTextbox(txtMessage, msg);
        }

        private void OnExecDetails(ExecutionMessage obj)
        {
            var msg = $"OnExecDetails: Price:{obj.Execution.Price} " +
               $"AvgPrice:{obj.Execution.AvgPrice} ";
            AddLineToTextbox(txtMessage, msg);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string host = txtHost.Text;
                int port = Int32.Parse(txtPort.Text);
                int clientId = Int32.Parse(txtClientId.Text);

                ibClient.ClientSocket.eConnect(host, port, ibClient.ClientId);

                // The EReader Thread
                var reader = new EReader(ibClient.ClientSocket, signal);
                reader.Start();
                new Thread(() =>
                {
                    while (ibClient.ClientSocket.IsConnected())
                    {
                        signal.waitForSignal();
                        reader.processMsgs();
                    }
                })
                { IsBackground = true }
                .Start();

                EnableControls(true);
            }
            catch (Exception ex)
            {
                txtMessage.Text += Environment.NewLine + ex.ToString();
            }
        }

        private void EnableControls(bool enable)
        {
            btListPositions.Enabled = enable;

            txtSymbolCheckSymbol.Enabled = enable;
            btGetConnId.Enabled = enable;

            txtSymbolStrike.Enabled = enable;
            txtExchange.Enabled = enable;
            txtSecType.Enabled = enable;
            txtConId.Enabled = enable;
            btStrikes.Enabled = enable;

            txtGenericTickListMarketData.Enabled = enable;
            btReqRealTime.Enabled = enable;
            btCancelRealTime.Enabled = enable;

            btPlaceBasicOrder.Enabled = enable;
            btComboOrder.Enabled = enable;
            btReqContractDetails.Enabled = enable;

            btReqScannerSubscription.Enabled = enable;
            btCancelScannerSubscription.Enabled = enable;
        }

        private void OnError(int id, int errorCode, string msg, Exception ex)
        {
            AddLineToTextbox(txtMessage, $"OnError: id={id} errorCode={errorCode} msg={msg} exception={ex}");

            //if (ex != null)
            //{
            //    addTextToBox("Error: " + ex);

            //    return;
            //}

            //if (id == 0 || errorCode == 0)
            //{
            //    addTextToBox("Error: " + str + "\n");

            //    return;
            //}

            //ErrorMessage error = new ErrorMessage(id, errorCode, str);

            //HandleErrorMessage(error);
        }

        private void OnNextValidId(ConnectionStatusMessage statusMessage)
        {
            _nextOrderId = ibClient.NextOrderId;
            string msg = statusMessage.IsConnected
                ? $"OnNextValidId: Connected! Your client Id: {ibClient.ClientId}  NextOrderId: {_nextOrderId}"
                : "nNextValidId: Disconnected...";

            AddLineToTextbox(txtMessage, msg);

            //IsConnected = statusMessage.IsConnected;

            //if (statusMessage.IsConnected)
            //{
            //    status_CT.Text = "Connected! Your client Id: " + ibClient.ClientId;
            //    connectButton.Text = "Disconnect";
            //}
            //else
            //{
            //    status_CT.Text = "Disconnected...";
            //    connectButton.Text = "Connect";
            //}
        }

        private void OnManagedAccounts(ManagedAccountsMessage message)
        {
            if (!message.ManagedAccounts.Any())
            {
                throw new Exception("OnManagedAccounts: Unexpected");
            }

            string msg = Environment.NewLine + "OnManagedAccounts: Acounts found: " + message.ManagedAccounts.Aggregate((r, n) => r + ", " + n);
            AddLineToTextbox(txtMessage, msg);

            //orderManager.ManagedAccounts = message.ManagedAccounts;
            //accountManager.ManagedAccounts = message.ManagedAccounts;
            //exerciseAccount.Items.AddRange(message.ManagedAccounts.ToArray());
        }

        private void OnPosition(PositionMessage obj)
        {
            string msg = $"OnPosition: Local Symbol:{obj.Contract.LocalSymbol} " +
                $"ConId:{obj.Contract.ConId} " +
                $"Avg.price:{obj.AverageCost} " +
                $"Symbol:{obj.Contract.Symbol} " +
                $"Sec. type:{obj.Contract.SecType} " +
                $"Multiplier:{obj.Contract.Multiplier}";

            AddLineToTextbox(txtMessage, msg);
        }

        private void OnPositionEnd()
        {
            string msg = "OnPositionEnd: End of all positions";
            AddLineToTextbox(txtMessage, msg);
        }

        private void OnContractDetails(ContractDetailsMessage obj)
        {
            string msg = $"OnContractDetails :ConId:{obj.ContractDetails.Contract.ConId} " +
                $"LocalSymbol:{obj.ContractDetails.Contract.LocalSymbol} " +

                $"Symbol:{obj.ContractDetails.Contract.Symbol} " +
                $"SecType:{obj.ContractDetails.Contract.SecType} " +
                $"Currency:{obj.ContractDetails.Contract.Currency} " +
                $"Exchange:{obj.ContractDetails.Contract.Exchange} " +
                $"PrimaryExch:{obj.ContractDetails.Contract.PrimaryExch} " +

                $"Right:{obj.ContractDetails.Contract.Right} " +
                $"LastTradeDate:{obj.ContractDetails.Contract.LastTradeDateOrContractMonth} " +
                $"Strike:{obj.ContractDetails.Contract.Strike} " +
                $"";

            AddLineToTextbox(txtMessage, msg);
        }

        private void OnSymbolSamples(SymbolSamplesMessage obj)
        {
            var msg = new StringBuilder("OnSymbolSamples: ");

            var contracts = obj.ContractDescriptions.Select(d => d.Contract).ToList();
            foreach (var contract in contracts)
            {
                msg.AppendLine($"{contract.Symbol} {contract.SecType} {contract.Currency} {contract.Exchange} ConId:{contract.ConId}");
            }

            AddLineToTextbox(txtMessage, msg.ToString());
        }

        private void OnSecurityDefinitionOptionParameter(SecurityDefinitionOptionParameterMessage obj)
        {
            var expirations = obj.Expirations.Aggregate((r, n) => r + "," + n);
            var strikes = obj.Strikes.Select(s => s.ToString()).Aggregate((r, n) => r + "," + n);
            string msg = $"OnSecurityDefinitionOptionParameter: ReqId:{obj.ReqId} expirations:{expirations} strikes:{strikes}";

            AddLineToTextbox(txtMessage, msg);
        }

        private void OnSecurityDefinitionOptionParameterEnd(int obj)
        {
            AddLineToTextbox(txtMessage, "OnSecurityDefinitionOptionParameterEnd: all strikes are listed");
        }

        private void OnTickGeneric(int arg1, int arg2, double arg3)
        {
            var msg = $"OnTickGeneric: arg1:{arg1} arg2:{arg2} arg3:{arg3}";
            AddLineToTextbox(txtMessage, msg.ToString());
        }

        private void TickString(int arg1, int arg2, string arg3)
        {
            var msg = $"TickString: arg1:{arg1} arg2:{arg2} arg3:{arg3}";
            AddLineToTextbox(txtMessage, msg.ToString());
        }

        private void OnTickSize(TickSizeMessage obj)
        {
            var msg = $"OnTickSize: obj.RequestId:{obj.RequestId} obj.Field:{obj.Field} obj.Size:{obj.Size}";
            AddLineToTextbox(txtMessage, msg.ToString());
        }

        private void OnTickPrice(TickPriceMessage obj)
        {
            var msg = $"OnTickPrice: obj.RequestId:{obj.RequestId} obj.Field:{obj.Field} obj.Price:{obj.Price} obj.Attribs:{obj.Attribs.toString()}";
            AddLineToTextbox(txtMessage, msg.ToString());
        }

        private void OnScannerParameters(string obj)
        {
            var msg = $"OnScannerParameters: {obj}";
            AddLineToTextbox(txtMessage, msg.ToString());
        }

        private void OnScannerDataEnd(int obj)
        {
            var msg = $"OnScannerDataEnd: {obj}";
            AddLineToTextbox(txtMessage, msg.ToString());
        }

        private void OnScannerData(ScannerMessage obj)
        {
            var msg = $"OnScannerData: Rank={obj.Rank}, Symbol={obj.ContractDetails.Contract.Symbol}";
            AddLineToTextbox(txtMessage, msg.ToString());
        }

        private void IbClient_TickNews(TickNewsMessage obj)
        {
            var newsLine = String.Format("Tick News. Ticker Id: {0}, Time Stamp: {1}, Provider Code: {2}, Article Id: {3}, headline: {4}, extraData: {5}",
                obj.TickerId, Util.LongMaxString(obj.TimeStamp), obj.ProviderCode, obj.ArticleId, obj.Headline, obj.ExtraData);

            AddLineToTextbox(txtMessage, newsLine);
        }

        private void AddLineToTextbox(TextBox textBox, string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = msg;
            }
            else
            {
                var fullMsg = msg + Environment.NewLine + textBox.Text;
                fullMsg = fullMsg.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

                textBox.Text = fullMsg;
            }
        }

        private void btCheckSymbol_Click(object sender, EventArgs e)
        {
            var contract = new Contract()
            {
                Symbol = txtSymbolCheckSymbol.Text,
                SecType = txtSecTypeCheckSymbol.Text,
                Currency = txtCurrencyCheckSymbol.Text,
                Exchange = txtExchangeCheckSymbol.Text
            };

            contract.LastTradeDateOrContractMonth =
                !string.IsNullOrWhiteSpace(txtLastTradeCheckSymbol.Text) ?
                txtLastTradeCheckSymbol.Text :
                null;

            contract.Strike =
                !string.IsNullOrWhiteSpace(txtStrikeCheckSymbol.Text) ?
                Convert.ToDouble(txtStrikeCheckSymbol.Text) :
                default(double);

            contract.Right =
                !string.IsNullOrWhiteSpace(txtRightCheckSymbol.Text) ?
                txtRightCheckSymbol.Text :
                null;

            ibClient.ClientSocket.reqContractDetails(60000001, contract);
        }

        private void btListPositions_Click(object sender, EventArgs e)
        {
            ibClient.ClientSocket.reqPositions();
        }

        private void btStrikes_Click(object sender, EventArgs e)
        {
            string symbol = txtSymbolStrike.Text;
            string exchange = txtExchange.Text;
            string secType = txtSecType.Text;
            int conId = Convert.ToInt32(txtConId.Text);

            ibClient.ClientSocket.reqSecDefOptParams(_activeReqId, symbol, exchange, secType, conId);
            _activeReqId++;
        }

        private void btReqRealTime_Click(object sender, EventArgs e)
        {
            // The API can request Live, Frozen, Delayed and Delayed Frozen market data from Trader Workstation
            // by switching market data type via the
            // IBApi.EClient.reqMarketDataType
            // before making a market data request with reqMktData.

            // https://interactivebrokers.github.io/tws-api/market_data_type.html

            // client.reqMarketDataType(2);

            Contract contract = new Contract  // ConId and Exchange is enough
            {
                Exchange = txtExchangeMarketData.Text,
                ConId = Convert.ToInt32(txtConItMarketData.Text)
            };
            string genericTickList = this.txtGenericTickListMarketData.Text;
            bool snapshot = true; // set it to false to receive permanent stream of data

            ibClient.ClientSocket.reqMktData(_activeReqId, contract, genericTickList, snapshot, false, new List<TagValue>());
        }

        private void btCancelRealTime_Click(object sender, EventArgs e)
        {
            var currentTicker = 1;
            ibClient.ClientSocket.cancelRealTimeBars(currentTicker + RT_BARS_ID_BASE);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMessage.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btPlaceOrder_Click(object sender, EventArgs e)
        {
            Contract contract = new Contract
            {
                Exchange = string.IsNullOrWhiteSpace(txtExchangeBasicOrder.Text)
                    ? null
                    : txtExchangeBasicOrder.Text,
                ConId = string.IsNullOrWhiteSpace(txtConIdBasicOrder.Text)
                    ? 0
                    : Convert.ToInt32(txtConIdBasicOrder.Text)
            };

            Order order = new Order
            {
                Action = txtActionBasicOrder.Text,
                OrderType = "LMT",
                TotalQuantity = Convert.ToInt32(txtQuantityBasicOrder.Text),
                LmtPrice = Double.Parse(txtLimitPriceBasicOrder.Text)
            };

            ibClient.ClientSocket.placeOrder(_nextOrderId, contract, order);
            ibClient.ClientSocket.reqIds(-1);
        }

        private void btDocumentationCheckSymbol_Click(object sender, EventArgs e)
        {
            _formContractId ??= new FormContractId();
            _formContractId.Show();
        }

        private void btComboOrder_Click(object sender, EventArgs e)
        {
            var exchange = string.IsNullOrWhiteSpace(txtExchageComboOrder.Text)
                    ? null
                    : txtExchageComboOrder.Text;

            Contract contract = new Contract
            {
                Symbol = txtSymbolComboOrder.Text,
                SecType = txtSecTypeComboOrder.Text,
                Exchange = txtExchageComboOrder.Text,
                Currency = txtCurrencyComboBox.Text
            };

            // Add legs
            var sellLeg = new ComboLeg()
            {
                Action = "SELL",
                ConId = Convert.ToInt32(txtSellLegConId.Text),
                Ratio = 1,
                Exchange = exchange
            };
            var buyLeg = new ComboLeg()
            {
                Action = "BUY",
                ConId = Convert.ToInt32(txtBuyLegConId.Text),
                Ratio = 1,
                Exchange = exchange
            };
            contract.ComboLegs = new List<ComboLeg>();
            contract.ComboLegs.AddRange(new List<ComboLeg> { sellLeg, buyLeg });

            Order order = new Order
            {
                Action = txtActionComboBox.Text,
                OrderType = "LMT",
                TotalQuantity = Convert.ToInt32(txtQuantityComboOrder.Text),
                LmtPrice = Double.Parse(txtLimitPriceComboOrder.Text)
            };

            ibClient.ClientSocket.placeOrder(_nextOrderId, contract, order);
            ibClient.ClientSocket.reqIds(-1);
        }

        private void btReqContractDetails_Click(object sender, EventArgs e)
        {
            var contract = new Contract()
            {
                ConId = Convert.ToInt32(txtConIdContractDetails.Text),
                Exchange = txtExchangeContractDetails.Text
            };

            ibClient.ClientSocket.reqContractDetails(60000001, contract);
        }

        private void btReqScannerSubscription_Click(object sender, EventArgs e)
        {
            ScannerSubscription scannerSubscription = new ScannerSubscription
            {
                ScanCode = "TOP_PERC_GAIN",
                Instrument = "STK",
                LocationCode = "STK.US, STK.NYSE, STK.AMEX, STK.ARCA, STK.NASDAQ.NMS, STK.NASDAQ.SC",
                NumberOfRows = 50  // Number of rows is capped to 50
            };

            List<TagValue> filterOptions = new List<TagValue>
            {
                new TagValue{Tag = "avgVolumeAbove", Value="2000000"}
            };

            List<TagValue> scannerSubscriptionOptions = new List<TagValue> { };

            reqScannerSubscription++;
            ibClient.ClientSocket.reqScannerSubscription(reqScannerSubscription, scannerSubscription, scannerSubscriptionOptions, filterOptions);
        }

        private void btCancelScannerSubscription_Click(object sender, EventArgs e)
        {
            ibClient.ClientSocket.cancelScannerSubscription(reqScannerSubscription);
        }

        private void btBroadTapeNews_Click(object sender, EventArgs e)
        {
            var symbols = new string[] { "BRFG:BRFG_ALL", "BRFUPDN:BRFUPDN_ALL", "DJNL:DJNL_ALL", "BZ:BZ_ALL", "DJTOP:DJTOP_ALL" };
            var exchanges = symbols.Select(s => s[..s.IndexOf(":")]).ToList();

            for (int i = 0; i < symbols.Length; i++)
            {
                _broadNewsReqId++;
                ibClient.ClientSocket.reqMktData(_broadNewsReqId, new Contract()
                {
                    Symbol = symbols[i],
                    SecType = "NEWS",
                    Exchange = exchanges[i],
                }, "mdoff,292", false, false, null);
            }
        }
    }
}
