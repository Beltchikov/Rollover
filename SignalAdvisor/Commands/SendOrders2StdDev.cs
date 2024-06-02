using IBApi;
using MathNet.Numerics.Statistics;
using System.Windows;

namespace SignalAdvisor.Commands
{
    public class SendOrders2StdDev
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            if (visitor.InstrumentToTrade.AskPrice <= 0)
            {
                MessageBox.Show($"The ask price is {visitor.InstrumentToTrade.AskPrice}. The market is probably closed. The execution of the command stops.");
                return;
            }

            var contract = visitor.InstrumentToTrade.ToContract();
            string endDateTime = DateTime.Now.ToString(App.FORMAT_STRING_API);
            string durationString = $"{App.BAR_SIZE_IN_MINUTES * App.STD_DEV_PERIOD * 60} S";
            //string barSizeSetting = "5 mins";
            string barSizeSetting = $"{App.BAR_SIZE_IN_MINUTES} mins";
            string whatToShow = "TRADES";
            int useRTH = 0;

            var lastHistoricalDataMessages = await visitor.IbHost.RequestHistoricalDataAsync(
                contract,
                endDateTime,
                durationString,
                barSizeSetting,
                whatToShow,
                useRTH,
                1,
                [],
                App.TIMEOUT);

            var midPrices = lastHistoricalDataMessages.Select(m => Math.Round((m.Close - m.Open) / 2, 2));
            var stdDev = midPrices.StandardDeviation();

            int orderId = await GetNextOrderIdAsync(visitor);
            

            //Order order = await CreateOrderAsync(visitor) ?? throw new Exception();
            //Contract contract = new Contract()
            //{
            //    ConId = visitor.InstrumentToTrade.ConId,
            //    Symbol = visitor.InstrumentToTrade.Symbol,
            //    SecType = App.SEC_TYPE_STK,
            //    Currency = visitor.InstrumentToTrade.Currency,
            //    Exchange = visitor.InstrumentToTrade.Exchange
            //};

            //double avrFillPrice = await visitor.IbHost.PlaceOrderAndWaitForExecution(contract, order);

            //// TP order
            //var commission = visitor.InstrumentToTrade.CalculateCommision();
            //var tpPriceInTenthOfCent = Math.Ceiling(avrFillPrice * 1000
            //    + visitor.InstrumentToTrade.TakeProfitInCents * 10
            //    + commission * 2 * 1000);
            //var tpPrice = Math.Round(tpPriceInTenthOfCent / 1000 + 0.01, 2);
            //Order tpOrder = await CreateTpOrderAsync(visitor, tpPrice) ?? throw new Exception();
            //await DoSendOrder(visitor, contract, tpOrder);
        }

        private static async Task<int> GetNextOrderIdAsync(IPositionsVisitor visitor)
        {
            if (visitor.OrdersSent > 0)
                await visitor.IbHost.ReqIdsAsync(-1);
            var orderId = visitor.IbHost.NextOrderId;
            return orderId; 
        }

        private static Order CreateOrder(int orderId, string action, double lmtPrice, int totalQuantity)
        {
            var order = new Order()
            {
                OrderId = orderId,
                Action = action,
                OrderType = "LMT",
                LmtPrice = lmtPrice,
                TotalQuantity = totalQuantity,
                Transmit = true,
                OutsideRth = true,
            };

            return order;
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
