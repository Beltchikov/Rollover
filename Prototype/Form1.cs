using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using System;
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
            this.btListPositions.Enabled = enable;

            this.txtSymbol.Enabled = enable;
            this.tbSecType.Enabled = enable;
            this.txtCurrency.Enabled = enable;
            this.tbExchange.Enabled = enable;

            this.btGetConnId.Enabled = enable;
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

        private void AddLineToTextbox(TextBox textBox, string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text += Environment.NewLine;
            }

            textBox.Text += msg;
        }

        private void btCheckSymbol_Click(object sender, EventArgs e)
        {

            //int reqID = 70100001;
            var symbol = txtSymbol.Text;
            //var exchange = tbExchange.Text;
            //var secType = tbSecType.Text;
            
            // TODO
            //ibClient.ClientSocket.reqSecDefOptParams(reqId, symbol, exchange, secType, conId);

            ibClient.ClientSocket.reqMatchingSymbols(++activeReqId, symbol);
        }

        private void btListPositions_Click(object sender, EventArgs e)
        {
            ibClient.ClientSocket.reqPositions();
        }
    }
}
