using IbClient;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing;
using SsbHedger.ResponseProcessing.Mapper;
using System;

namespace SsbHedger
{
    public class Logic : ILogic
    {
        IIBClient _ibClient;
        IResponseLoop _responseLoop;
        IResponseHandler _responseHandler;
        IResponseMapper _responseMapper;
        IResponseProcessor _responseProcessor;
        IBackgroundWorkerAbstraction _backgroundWorker;

        public Logic(
            IIBClient ibClient,
            IResponseLoop responseLoop,
            IResponseHandler responseHandler,
            IResponseMapper responseMapper,
            IResponseProcessor responseProcessor,
            IBackgroundWorkerAbstraction backgroundWorker)
        {
            _ibClient = ibClient;
            _responseLoop = responseLoop;
            _responseHandler = responseHandler;
            _responseMapper = responseMapper;
            _responseProcessor = responseProcessor;
            _backgroundWorker = backgroundWorker;

            _responseProcessor.SetLogic(this);

            _responseLoop.Actions = () =>
            {
                var message = responseHandler.ReaderQueue.Dequeue();
                if (message == null)
                {
                    return;
                }
                _responseMapper.AddResponse(message);
                var responses = _responseMapper.GetGrouppedResponses();
                foreach (var response in responses)
                {
                    _responseProcessor.Process(response);
                }
            };
        }

        public event Action<bool> NextValidId;
        public event Action<string> Error;

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

        public void InvokeError(string message)
        {
            Error.Invoke(message);
        }

    }
}
