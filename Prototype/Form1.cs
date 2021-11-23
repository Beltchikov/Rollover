using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototype
{
    public partial class Form1 : Form
    {
        private EReaderMonitorSignal signal;
        private IBClient ibClient;


        public Form1()
        {
            InitializeComponent();

            this.btListPositions.Enabled = false;
            this.txtSymbol.Enabled = false;
            this.btCheckSymbol.Enabled = false;

            signal = new EReaderMonitorSignal();
            ibClient = new IBClient(signal);

            ibClient.Error += OnError;
            ibClient.NextValidId += OnNextValidId;
            ibClient.ManagedAccounts += OnManagedAccounts;
            ibClient.Position += OnPosition;
            ibClient.PositionEnd += OnPositionEnd;
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

                //
                this.btListPositions.Enabled = true;
                this.txtSymbol.Enabled = true;
                this.btCheckSymbol.Enabled = true;
            }
            catch (Exception ex)
            {
                txtMessage.Text += Environment.NewLine + ex.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
            string msg = obj.Contract.LocalSymbol;
            AddLineToTextbox(txtMessage, msg);
        }

        private void OnPositionEnd()
        {
            string msg = "End of all positions";
            AddLineToTextbox(txtMessage, msg);
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
            throw new NotImplementedException();
        }

        private void btListPositions_Click(object sender, EventArgs e)
        {
            ibClient.ClientSocket.reqPositions();
        }
    }
}
