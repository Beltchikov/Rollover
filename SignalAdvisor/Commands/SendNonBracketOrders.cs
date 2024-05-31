using IBApi;
using System.Windows;

namespace SignalAdvisor.Commands
{
    public class SendNonBracketOrders
    {
        static bool _orderIsFilled;
        static double _avrFillPrice = 0;
        static readonly string FILLED = "FILLED";


        public static async Task RunAsync(IPositionsVisitor visitor)
        {

            Order order = await CreateOrderAsync(visitor) ?? throw new Exception();
            Contract contract = new Contract()
            {
                ConId = visitor.InstrumentToTrade.ConId,
                Symbol = visitor.InstrumentToTrade.Symbol,
                SecType = App.SEC_TYPE_STK,
                Currency = visitor.InstrumentToTrade.Currency,
                Exchange = visitor.InstrumentToTrade.Exchange
            };

            visitor.IbHost.OrderStatus += IbHost_OrderStatus;

            await DoSendOrder(visitor, contract, order);
            visitor.OrdersSent++;

            await Task.Run(() =>
            {
                while (!_orderIsFilled) { };
            });

            // TP order
            var tpPriceInCents = Math.Ceiling(_avrFillPrice * 100 + visitor.InstrumentToTrade.TakeProfitInCents);
            var tpPrice = Math.Round(tpPriceInCents / 100, 2);
            Order tpOrder = await CreateTpOrderAsync(visitor, tpPrice) ?? throw new Exception();
            await DoSendOrder(visitor, contract, tpOrder);

        }

        private static void IbHost_OrderStatus(object? sender, IbClient.Events.OrderStatusEventArgs e)
        {
            object lockObjekt = new object();

            lock (lockObjekt)
            {
                var orderStatusMessage = e.Message;
                if (orderStatusMessage.Status.ToUpper() == FILLED)
                {
                    _orderIsFilled = true;
                    _avrFillPrice = orderStatusMessage.AvgFillPrice;
                }
            }
        }

        private static async Task<Order?> CreateOrderAsync(IPositionsVisitor visitor)
        {
            var askPrice = visitor.InstrumentToTrade.AskPrice;
            if (askPrice <= 0)
            {
                MessageBox.Show($"The ask price is {askPrice}. The market is probably closed. The execution of the command stops.");
                return (null);
            }

            if (visitor.OrdersSent > 0)
                await visitor.IbHost.ReqIdsAsync(-1);
            var orderId = visitor.IbHost.NextOrderId;

            //
            var order = new Order()
            {
                OrderId = orderId,
                Action = "BUY",
                OrderType = "LMT",
                LmtPrice = askPrice,
                TotalQuantity = visitor.InstrumentToTrade.Quantity,
                Transmit = true,
                OutsideRth = true,
            };

            return (order);
        }

        private static async Task<Order?> CreateTpOrderAsync(IPositionsVisitor visitor, double price)
        {
            if (visitor.OrdersSent > 0)
                await visitor.IbHost.ReqIdsAsync(-1);
            var orderId = visitor.IbHost.NextOrderId;

            //
            var order = new Order()
            {
                OrderId = orderId,
                Action = "SELL",
                OrderType = "LMT",
                LmtPrice = price,
                TotalQuantity = visitor.InstrumentToTrade.Quantity,
                Transmit = true,
                OutsideRth = true,
            };

            return (order);
        }

        private static async Task DoSendOrder(IPositionsVisitor visitor, Contract contract, Order order)
        {
            var result = await visitor.IbHost.PlaceOrderAsync(contract, order, App.TIMEOUT);

            if (result.ErrorMessage != "")
            {
                visitor.TwsMessageCollection.Add($"ConId={contract.ConId} {contract.Symbol} error:{result.ErrorMessage}");

            }
            else if (result.OrderState != null)
            {
                visitor.TwsMessageCollection.Add($"ConId={contract.ConId} {contract.Symbol} order submitted.");
            }
            else throw new Exception("Unexpected. Both ErrorMessage nad OrderState are not set.");
        }
    }
}
