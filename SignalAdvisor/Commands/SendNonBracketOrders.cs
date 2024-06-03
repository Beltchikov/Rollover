using IBApi;
using System.Windows;

namespace SignalAdvisor.Commands
{
    public class SendNonBracketOrders
    {
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

            double avrFillPrice = await visitor.IbHost.PlaceOrderAndWaitForExecution(contract, order);

            // TP order
            var commission = visitor.InstrumentToTrade.CalculateCommision();
            var tpPriceInTenthOfCent = Math.Ceiling(avrFillPrice * 1000
                + visitor.InstrumentToTrade.TakeProfitInCents * 10
                + commission * 2 * 1000);
            var tpPrice = Math.Round(tpPriceInTenthOfCent / 1000 + 0.01, 2);
            Order tpOrder = await CreateTpOrderAsync(visitor, tpPrice) ?? throw new Exception();
            await DoSendOrder(visitor, contract, tpOrder);
        }

        private static async Task<Order?> CreateOrderAsync(IPositionsVisitor visitor)
        {
            // TODO refactor in separate method GetAskPrice
            var askPrice = visitor.InstrumentToTrade.AskPrice;
            if (askPrice <= 0)
            {
                MessageBox.Show($"The ask price is {askPrice}. The market is probably closed. The execution of the command stops.");
                return (null);
            }

            var orderId = visitor.IbHost.NextOrderId;
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
            var orderId = visitor.IbHost.NextOrderId;

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
