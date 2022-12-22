using IbClient;
using IbClient.messages;
using SsbHedger2.Abstractions;
using SsbHedger2.ResponseProcessing;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Threading;

[assembly: InternalsVisibleTo("SsbHedger2.UnitTests")]

namespace SsbHedger2.WpfIbClient
{
    public class WpfIbClient : IWpfIbClient
    {
        internal IIBClient _ibClient;
        IResponseLoop _responseLoop;
        IResponseHandler _responseHandler;
        IBackgroundWorkerAbstraction _backgroundWorker;

        public event Action<ConnectionStatusMessage> NextValidId = null!;
        public event Action<int, string> Error = null!;
        public event Action<ManagedAccountsMessage> ManagedAccounts = null!;

        internal WpfIbClient(
            IIBClient ibClient,
            IResponseLoop responseLoop,
            IResponseHandler responseHandler,
            IBackgroundWorkerAbstraction backgroundWorker)
        {
            _ibClient = ibClient;
            _responseLoop = responseLoop;
            _responseHandler = responseHandler;
            _responseHandler.SetClient(this);
            _backgroundWorker = backgroundWorker;

            _responseLoop.Actions = () =>
            {
                _responseHandler.HandleNextMessage();
            };
        }

        public static IWpfIbClient Create(Func<bool> breakCondition, Dispatcher dispatcher)
        {
            return new WpfIbClient(
                 IBClient.CreateClient(),
                 new ResponseLoop() { BreakCondition = breakCondition },
                 new ResponseHandler(
                     new ReaderThreadQueue(),
                     new DispatcherAbstraction(dispatcher)),
                 new BackgroundWorkerAbstraction());
        }


        public void Execute(string host, int port, int clientId)
        {
            _ibClient.NextValidId += _responseHandler.OnNextValidId;
            _ibClient.Error += _responseHandler.OnError;
            _ibClient.ManagedAccounts += _responseHandler.OnManagedAccounts;
            _ibClient.OpenOrder += _responseHandler.OnOpenOrder;
            _ibClient.OpenOrderEnd += _responseHandler.OnOpenOrderEnd;
            _ibClient.OrderStatus += _responseHandler.OnOrderStatus;

            _ibClient.ConnectAndStartReaderThread(host ,port ,clientId);

            _backgroundWorker.SetDoWorkEventHandler((s, e) =>
            {
                _responseLoop.Start();
            });
            _backgroundWorker.RunWorkerAsync();
        }

        //private void btPlaceOrder_Click(object sender, EventArgs e)
        //{
        //    Contract contract = new Contract
        //    {
        //        Exchange = string.IsNullOrWhiteSpace(txtExchangeBasicOrder.Text)
        //            ? null
        //            : txtExchangeBasicOrder.Text,
        //        ConId = string.IsNullOrWhiteSpace(txtConIdBasicOrder.Text)
        //            ? 0
        //            : Convert.ToInt32(txtConIdBasicOrder.Text)
        //    };

        //    Order order = new Order
        //    {
        //        Action = txtActionBasicOrder.Text,
        //        OrderType = "LMT",
        //        TotalQuantity = Convert.ToInt32(txtQuantityBasicOrder.Text),
        //        LmtPrice = Double.Parse(txtLimitPriceBasicOrder.Text)
        //    };

        //    ibClient.ClientSocket.placeOrder(_nextOrderId, contract, order);
        //    ibClient.ClientSocket.reqIds(-1);
        //}

        public void InvokeError(int reqId, string message)
        {
            Error?.Invoke(reqId, message);
        }

        public void InvokeNextValidId(ConnectionStatusMessage message)
        {
            NextValidId?.Invoke(message);
        }

        public void InvokeManagedAccounts(ManagedAccountsMessage message)
        {
            ManagedAccounts?.Invoke(message);
        }
    }
}
