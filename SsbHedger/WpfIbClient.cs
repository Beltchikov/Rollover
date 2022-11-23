using IbClient;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing;
using SsbHedger.ResponseProcessing.Mapper;
using System;
using System.Windows.Threading;

namespace SsbHedger
{
    public class WpfIbClient : ILogic
    {
        IIBClient _ibClient;
        IResponseLoop _responseLoop;
        IResponseHandler _responseHandler;
        IResponseMapper _responseMapper;
        IResponseProcessor _responseProcessor;
        IBackgroundWorkerAbstraction _backgroundWorker;

        public WpfIbClient(
            Func<bool> responseLoopBreakCondition,
            Dispatcher dispatcher,
            IBackgroundWorkerAbstraction backgroundWorker)
        {
            _ibClient = IBClient.CreateClient();
            _responseLoop = new ResponseLoop() { BreakCondition = responseLoopBreakCondition };
            _responseHandler = new ResponseHandler(new ReaderThreadQueue());
            _responseMapper = new ResponseMapper();
            _responseProcessor = new ResponseProcessor(new DispatcherAbstraction(dispatcher));
            _backgroundWorker = backgroundWorker;

            _responseProcessor.SetLogic(this);

            _responseLoop.Actions = () =>
            {
                var message = _responseHandler.ReaderQueue.Dequeue();
                if (message == null)
                {
                    return;
                }
                _responseMapper.AddResponse(message);
                var responses = _responseMapper.GetGrouppedResponses();
                foreach (ReqIdAndResponses response in responses)
                {
                    _responseProcessor.Process(response);
                }
            };
        }

        public event Action<int, bool> NextValidId;
        public event Action<int, string> Error;

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
