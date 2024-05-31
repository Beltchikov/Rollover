using IBApi;
using System.Windows;

namespace SignalAdvisor.Commands
{
    public class SendNonBracketOrders
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {

            Order? order = await CreateOrderAsync(visitor);
            if (order == null)
            {
                return;
            }

            Contract contract = new Contract()
            {
                ConId = visitor.InstrumentToTrade.ConId,
                Symbol = visitor.InstrumentToTrade.Symbol,
                SecType = App.SEC_TYPE_STK,
                Currency = visitor.InstrumentToTrade.Currency,
                Exchange = visitor.InstrumentToTrade.Exchange
            };

            await DoSendOrder(visitor, contract, order);
            visitor.OrdersSent++;
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
                Transmit = true
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
