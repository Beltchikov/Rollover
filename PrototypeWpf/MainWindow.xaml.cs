using IBApi;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TwsApi;

namespace PrototypeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IBClient _ibClient;
        private EReaderMonitorSignal _signal;
        private int _nextOrderId;

        public MainWindow()
        {
            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);

            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.Error += _ibClient_Error;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;

            InitializeComponent();
        }

        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ibClient.ClientSocket.eConnect(
                       tbHost.Text,
                       Int32.Parse(tbPort.Text),
                       Int32.Parse(tbClientId.Text));

                // The EReader Thread
                var reader = new EReader(_ibClient.ClientSocket, _signal);
                reader.Start();
                new Thread(() =>
                {
                    while (_ibClient.ClientSocket.IsConnected())
                    {
                        _signal.waitForSignal();
                        reader.processMsgs();
                    }
                })
                { IsBackground = true }
                .Start();
            }
            catch (Exception ex)
            {
                tbMessages.Text = ex.ToString() 
                    + (String.IsNullOrWhiteSpace(tbMessages.Text)
                        ? String.Empty
                        : Environment.NewLine + tbMessages.Text);
            }
        }

        private void _ibClient_NextValidId(TwsApi.messages.ConnectionStatusMessage statusMessage)
        {
            _nextOrderId = _ibClient.NextOrderId;
            string msg = statusMessage.IsConnected
                ? $"OnNextValidId: Connected! Your client Id: {_ibClient.ClientId}  NextOrderId: {_nextOrderId}"
                : "nNextValidId: Disconnected...";

            AddLineToTextbox(tbMessages, msg);
        }

        private void _ibClient_Error(int id, int errorCode, string msg, Exception ex)
        {
            AddLineToTextbox(tbMessages, $"OnError: id={id} errorCode={errorCode} msg={msg} exception={ex}");
        }

        private void _ibClient_ManagedAccounts(TwsApi.messages.ManagedAccountsMessage message)
        {
            if (!message.ManagedAccounts.Any())
            {
                throw new Exception("OnManagedAccounts: Unexpected");
            }

            string msg = Environment.NewLine + "OnManagedAccounts: Acounts found: " + message.ManagedAccounts.Aggregate((r, n) => r + ", " + n);
            AddLineToTextbox(tbMessages, msg);
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
    }
}
