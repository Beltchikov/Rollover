﻿using IbClient;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

[assembly: InternalsVisibleTo("SsbHedger.UnitTests")]

namespace SsbHedger.WpfIbClient
{
    public class WpfIbClient : IWpfIbClient
    {
        internal IIBClient _ibClient;
        IResponseLoop _responseLoop;
        IResponseHandler _responseHandler;
        IBackgroundWorkerAbstraction _backgroundWorker;

        internal WpfIbClient(
            IIBClient ibClient,
            IResponseLoop responseLoop,
            IResponseHandler responseHandler,
            IBackgroundWorkerAbstraction backgroundWorker,
            IDispatcherAbstraction dispatcherAbstraction)
        {
            _ibClient = ibClient;
            _responseLoop = responseLoop;
            _responseHandler = responseHandler;
            _backgroundWorker = backgroundWorker;

            _responseLoop.Actions = () =>
            {
                var message = _responseHandler.Dequeue();
                if (message == null)
                {
                    return;
                }

                dispatcherAbstraction.Invoke(() => InvokeError(-1, message.ToString()));
            };
        }

        public event Action<int, bool> NextValidId;
        public event Action<int, string> Error;

        public static IWpfIbClient Create(Func<bool> breakCondition, Dispatcher dispatcher)
        {
            return new WpfIbClient(
                 IBClient.CreateClient(),
                 new ResponseLoop() { BreakCondition = breakCondition },
                 new ResponseHandler(new ReaderThreadQueue()),
                 new BackgroundWorkerAbstraction(),
                 new DispatcherAbstraction(dispatcher));
        }

        public void Execute()
        {
            _ibClient.NextValidId += _responseHandler.OnNextValidId;
            _ibClient.Error += _responseHandler.OnError;
            _ibClient.ManagedAccounts += _responseHandler.OnManagedAccounts;
            _ibClient.OpenOrder += _responseHandler.OnOpenOrder;
            _ibClient.OpenOrderEnd += _responseHandler.OnOpenOrderEnd;
            _ibClient.OrderStatus += _responseHandler.OnOrderStatus;

            _ibClient.ConnectAndStartReaderThread(
                       "localhost",
                       4001,
                       1);

            _backgroundWorker.SetDoWorkEventHandler((s, e) =>
            {
                _responseLoop.Start();
            });
            _backgroundWorker.RunWorkerAsync();


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
        }

        public void InvokeError(int reqId, string message)
        {
            Error.Invoke(reqId, message);
        }
    }
}
