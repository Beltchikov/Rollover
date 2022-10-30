using IbClient;
using IbClient.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger
{
    public class Logic : ILogic
    {
        IIBClient _ibClient;

        public Logic(IIBClient ibClient)
        {
            _ibClient = ibClient;
        }

        public void Execute()
        {
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.Error += _ibClient_Error;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ContractDetails += _ibClient_ContractDetails;
            _ibClient.TickPrice += _ibClient_TickPrice;
            _ibClient.TickSize += _ibClient_TickSize;
            _ibClient.TickString += _ibClient_TickString;
            _ibClient.TickOptionCommunication += _ibClient_TickOptionCommunication;
            _ibClient.OpenOrder += _ibClient_OpenOrder;
            _ibClient.OpenOrderEnd += _ibClient_OpenOrderEnd;
            _ibClient.OrderStatus += _ibClient_OrderStatus;


            _ibClient.ConnectAndStartReaderThread(
                       "localhost",
                       4001,
                       1);

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

        private void _ibClient_OrderStatus(IbClient.messages.OrderStatusMessage obj)
        {
            // TODO
        }

        private void _ibClient_OpenOrderEnd()
        {
            // TODO
        }

        private void _ibClient_OpenOrder(IbClient.messages.OpenOrderMessage obj)
        {
            // TODO
        }

        private void _ibClient_TickOptionCommunication(IbClient.messages.TickOptionMessage obj)
        {
            // TODO
        }

        private void _ibClient_TickString(int arg1, int arg2, string arg3)
        {
            // TODO
        }

        private void _ibClient_TickSize(IbClient.messages.TickSizeMessage obj)
        {
            // TODO
        }

        private void _ibClient_TickPrice(IbClient.messages.TickPriceMessage obj)
        {
            // TODO
        }

        private void _ibClient_ContractDetails(IbClient.messages.ContractDetailsMessage obj)
        {
            // TODO
        }

        private void _ibClient_ManagedAccounts(IbClient.messages.ManagedAccountsMessage obj)
        {
            // TODO
        }

        private void _ibClient_Error(int arg1, int arg2, string arg3, Exception arg4)
        {
            // TODO
        }

        private void _ibClient_NextValidId(ConnectionStatusMessage obj)
        {
            // TODO
        }
    }
}
