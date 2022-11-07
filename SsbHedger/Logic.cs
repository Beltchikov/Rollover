﻿using IbClient;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing;
using SsbHedger.ResponseProcessing.Mapper;
using System;
using System.Windows.Threading;

namespace SsbHedger
{
    public class Logic : ILogic
    {
        IIBClient _ibClient;
        IResponseLoop _responseLoop;
        IResponseHandler _responseHandler;
        IResponseMapper _responseMapper;
        IResponseProcessor _responseProcessor;

        public Logic(
            IIBClient ibClient,
            IResponseLoop responseLoop,
            IResponseHandler responseHandler,
            IResponseMapper responseMapper,
            IResponseProcessor responseProcessor)
        {
            _ibClient = ibClient;
            _responseLoop = responseLoop;
            _responseHandler = responseHandler;
            _responseMapper = responseMapper;
            _responseProcessor = responseProcessor;

            _responseProcessor.SetLogic(this);

            _responseLoop.BreakCondition =
                () => 1==0;
            _responseLoop.Actions = () =>
            {
                var message = responseHandler.ReaderQueue.Dequeue();
                if (message == null)
                {
                    return;
                }
                responseMapper.AddResponse(message);
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

            _responseLoop.Start();

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
