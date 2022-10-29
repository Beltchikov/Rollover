using IBApi;
using IbClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PrototypeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IIBClient _ibClient;
        private int _nextOrderId;
        private int _requestId;

        public MainWindow()
        {
            _ibClient = IBClient.CreateClient();

            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.Error += _ibClient_Error;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ContractDetails += _ibClient_ContractDetails;
            _ibClient.TickPrice += _ibClient_TickPrice;
            _ibClient.TickSize += _ibClient_TickSize;
            _ibClient.TickString += _ibClient_TickString;
            _ibClient.TickOptionCommunication += _ibClient_TickOptionCommunication;

            InitializeComponent();
        }

        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            _ibClient.ConnectAndStartReaderThread(            
                       tbHost.Text,
                       Int32.Parse(tbPort.Text),
                       Int32.Parse(tbClientId.Text));

        }
        private void btContractDetails_Click(object sender, RoutedEventArgs e)
        {
            var contract = new Contract()
            {
                Symbol = tbSymbol.Text,
                SecType = tbSecType.Text,
                Exchange = tbExchange.Text,
                Currency = tbCurrency.Text
            };

            _requestId++;
            _ibClient.ClientSocket.reqContractDetails(_requestId, contract);
        }

        private void btMarketData_Click(object sender, RoutedEventArgs e)
        {
            Contract contract = new Contract  
            {
                Exchange = tbExchangeMarketData.Text,
                ConId = Convert.ToInt32(tbConId.Text),
                LocalSymbol = tbLocalSymbol.Text
            };
            string genericTickList = string.Empty;
            bool snapshot = true; // set it to false to receive permanent stream of data

            _requestId++;
            _ibClient.ClientSocket.reqMktData(_requestId, contract, genericTickList, snapshot, false, new List<TagValue>());
        }

        private void _ibClient_NextValidId(IbClient.messages.ConnectionStatusMessage statusMessage)
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

        private void _ibClient_ManagedAccounts(IbClient.messages.ManagedAccountsMessage message)
        {
            if (!message.ManagedAccounts.Any())
            {
                throw new Exception("OnManagedAccounts: Unexpected");
            }

            string msg = Environment.NewLine + "OnManagedAccounts: Acounts found: " + message.ManagedAccounts.Aggregate((r, n) => r + ", " + n);
            AddLineToTextbox(tbMessages, msg);
        }

        private void _ibClient_ContractDetails(IbClient.messages.ContractDetailsMessage obj)
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

            AddLineToTextbox(tbMessages, msg);
        }

        private void _ibClient_TickString(int arg1, int arg2, string arg3)
        {
            var msg = $"TickString: arg1:{arg1} arg2:{arg2} arg3:{arg3}";
            AddLineToTextbox(tbMessages, msg.ToString());
        }

        private void _ibClient_TickSize(IbClient.messages.TickSizeMessage obj)
        {
            var msg = $"OnTickSize: obj.RequestId:{obj.RequestId} obj.Field:{obj.Field} obj.Size:{obj.Size}";
            AddLineToTextbox(tbMessages, msg.ToString());
        }

        private void _ibClient_TickPrice(IbClient.messages.TickPriceMessage obj)
        {
            var msg = $"OnTickPrice: obj.RequestId:{obj.RequestId} obj.Field:{obj.Field} obj.Price:{obj.Price} obj.Attribs:{obj.Attribs.toString()}";
            AddLineToTextbox(tbMessages, msg.ToString());
        }

        private void _ibClient_TickOptionCommunication(IbClient.messages.TickOptionMessage obj)
        {
            var msg = $"OnTickOptionCommunication: obj.RequestId:{obj.RequestId} " +
                $"obj.Field:{obj.Field} " +
                $"obj.ImpliedVolatility:{obj.ImpliedVolatility} obj.Delta:{obj.Delta} obj.OptPrice:{obj.OptPrice} obj.PvDividend:{obj.PvDividend} " +
                $"obj.Gamma:{obj.Gamma} obj.Vega:{obj.Vega} obj.Theta:{obj.Theta} obj.UndPrice:{obj.UndPrice}";
            AddLineToTextbox(tbMessages, msg.ToString());
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
