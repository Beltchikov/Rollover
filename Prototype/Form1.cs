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
    public partial class Form1 : Form
    {
        private EReaderMonitorSignal signal;
        private IBClient ibClient;
        int activeReqId = 0;

        public const int RT_BARS_ID_BASE = 40000000;

        public Form1()
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

            txtSymbol.Enabled = enable;
            btGetConnId.Enabled = enable;

            txtReqId.Enabled = enable;
            txtSymbolStrike.Enabled = enable;
            txtExchange.Enabled = enable;
            txtSecType.Enabled = enable;
            txtConId.Enabled = enable;
            btStrikes.Enabled = enable;

            txtGenericTickList.Enabled = enable;
            txtSymbolRealTime.Enabled = enable;
            txtSecTypeRealTime.Enabled = enable;
            txtCurrencyRealTime.Enabled = enable;
            txtExchangeRealTime.Enabled = enable;
            txtLocalSymbolRealTime.Enabled = enable;
            btReqRealTime.Enabled = enable;
            btCancelRealTime.Enabled = enable;
        }

        private void OnError(int id, int errorCode, string msg, Exception ex)
        {
            AddLineToTextbox(txtMessage, msg);

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
            string msg = statusMessage.IsConnected
                ? "Connected! Your client Id: " + ibClient.ClientId
                : "Disconnected...";

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
                throw new Exception("Unexpected");
            }

            string msg = Environment.NewLine + "Acounts found: " + message.ManagedAccounts.Aggregate((r, n) => r + ", " + n);
            AddLineToTextbox(txtMessage, msg);

            //orderManager.ManagedAccounts = message.ManagedAccounts;
            //accountManager.ManagedAccounts = message.ManagedAccounts;
            //exerciseAccount.Items.AddRange(message.ManagedAccounts.ToArray());
        }

        private void OnPosition(PositionMessage obj)
        {
            string msg = $"Local Symbol:{obj.Contract.LocalSymbol} " +
                $"ConId:{obj.Contract.ConId} " +
                $"Avg.price:{obj.AverageCost} " +
                $"Symbol:{obj.Contract.Symbol} " +
                $"Sec. type:{obj.Contract.SecType} " +
                $"Multiplier:{obj.Contract.Multiplier}";

            AddLineToTextbox(txtMessage, msg);
        }

        private void OnPositionEnd()
        {
            string msg = "End of all positions";
            AddLineToTextbox(txtMessage, msg);
        }

        private void OnSymbolSamples(SymbolSamplesMessage obj)
        {
            var msg = new StringBuilder();

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
            string msg = $"ReqId:{obj.ReqId} expirations:{expirations} strikes:{strikes}";

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
            var msg = $"OnTickPrice: obj.RequestId:{obj.RequestId} obj.Field:{obj.Field} obj.Price:{obj.Price}";
            AddLineToTextbox(txtMessage, msg.ToString());
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
            var symbol = txtSymbol.Text;
            ibClient.ClientSocket.reqMatchingSymbols(++activeReqId, symbol);
        }

        private void btListPositions_Click(object sender, EventArgs e)
        {
            ibClient.ClientSocket.reqPositions();
        }

        private void btStrikes_Click(object sender, EventArgs e)
        {
            int reqId = Convert.ToInt32(txtReqId.Text);
            string symbol = txtSymbolStrike.Text;
            string exchange = txtExchange.Text;
            string secType = txtSecType.Text;
            int conId = Convert.ToInt32(txtConId.Text);

            ibClient.ClientSocket.reqSecDefOptParams(reqId, symbol, exchange, secType, conId);
        }

        private void btReqRealTime_Click(object sender, EventArgs e)
        {
            int nextReqId = Convert.ToInt32(txtReqId.Text);
            Contract contract = new Contract
            {
                Symbol = txtSymbolRealTime.Text,
                Currency = txtCurrencyRealTime.Text,
                Exchange = txtExchangeRealTime.Text,
                SecType = txtSecTypeRealTime.Text,
                LocalSymbol = txtLocalSymbolRealTime.Text,
            };
            string genericTickList = this.txtGenericTickList.Text;

            ibClient.ClientSocket.reqMktData(nextReqId, contract, genericTickList, false, false, new List<TagValue>());
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
    }
}
