using IBApi;
using MathNet.Numerics.Statistics;
using System.Windows;
using System.Windows.Controls;

namespace SignalAdvisor.Commands
{
    public class SendOrders2StdDevShort
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            if (visitor.InstrumentToTrade.BidPrice <= 0)
            {
                MessageBox.Show($"The bid price is {visitor.InstrumentToTrade.BidPrice}. The market is probably closed. The execution of the command stops.");
                return;
            }

            var contract = visitor.InstrumentToTrade.ToContract();
            DateTime last10BarsEndTime = GetLast10BarsEndTime();
            var lastHistoricalDataMessages = await visitor.IbHost.RequestHistoricalDataAsync(
                contract,
                last10BarsEndTime.ToString(App.FORMAT_STRING_API),
                 $"{App.BAR_SIZE_IN_MINUTES * App.STD_DEV_PERIOD * 60} S",
                $"{App.BAR_SIZE_IN_MINUTES} mins",
                "TRADES",
                0,
                1,
                [],
                App.TIMEOUT);

            var midPrices = lastHistoricalDataMessages.Select(m => Math.Round((m.Close + m.Open) / 2, 2));
            var twoStdDev = midPrices.StandardDeviation() * 2;
            var qty = (int)Math.Round(App.RISK_IN_USD / twoStdDev, 0);
            if (qty <= 0)
            {
                MessageBox.Show($"The calculated quantity is {qty}. The execution of the command stops.");
                return;
            }

            int orderId = await GetNextOrderIdAsync(visitor);
            Order order = CreateOrder(orderId, "SELL", "LMT", visitor.InstrumentToTrade.BidPrice, qty, true);
            LogOrder(visitor, contract, order);
            double avrFillPrice = await visitor.IbHost.PlaceOrderAndWaitForExecution(contract, order);

            // Wait for order execution
            if (avrFillPrice <= 0)
            {
                MessageBox.Show($"The avrFillPrice is invalid: {avrFillPrice}. Something went wrong. The execution of the command stops.");
                return;
            }

            // TP order
            string ocaGroup = $"ocaGroup-{orderId}";
            int orderIdTakeProfit = orderId + 1;
            var tpDistance = (App.LIVE_COST_PROFIT / App.RISK_IN_USD) * twoStdDev; 
            var tpPrice = Math.Round(avrFillPrice - tpDistance, 2);
            Order orderTakeProfit = CreateOrder(orderIdTakeProfit, "BUY", "LMT", tpPrice, qty, true, ocaGroup);

            // SL order
            int orderIdStopLoss = orderIdTakeProfit + 2;
            var slPrice = Math.Round(avrFillPrice + twoStdDev, 2);
            Order orderStopLoss= CreateOrder(orderIdStopLoss, "BUY", "MIDPRICE", slPrice, qty, false, ocaGroup);

            PriceCondition priceConditionStop = (PriceCondition)OrderCondition.Create(OrderConditionType.Price);
            priceConditionStop.ConId = visitor.InstrumentToTrade.ConId;
            priceConditionStop.Exchange = visitor.InstrumentToTrade.Exchange;
            priceConditionStop.IsMore = true;
            priceConditionStop.Price = slPrice;
            orderStopLoss.Conditions.Add(priceConditionStop);

            // Place Orders
            await PlaceOrderAndHandleResultAsync(visitor, contract, orderTakeProfit, App.TIMEOUT);
            await PlaceOrderAndHandleResultAsync(visitor, contract, orderStopLoss, App.TIMEOUT);

            await GetNextOrderIdAsync(visitor);
        }

        private static async Task PlaceOrderAndHandleResultAsync(IPositionsVisitor visitor, Contract contract, Order order, int timeout)
        {
            var result = await visitor.IbHost.PlaceOrderAsync(contract, order, App.TIMEOUT);

            if (result.ErrorMessage != "")
            {
                var msg = $"ConId={contract.ConId} {contract.Symbol} error:{result.ErrorMessage}";
                visitor.TwsMessageCollection.Add(msg);
                visitor.OrderLog = string.IsNullOrWhiteSpace(visitor.OrderLog)
                    ? msg
                    : visitor.OrderLog + Environment.NewLine + msg;

            }
            else if (result.OrderState != null)
            {
                var msg = $"ConId={contract.ConId} {contract.Symbol} take profit order submitted.";
                visitor.TwsMessageCollection.Add(msg);
            }
            else throw new Exception("Unexpected. Both ErrorMessage nad OrderState are not set.");
        }

        private static void LogOrder(IPositionsVisitor visitor, Contract contract, Order order)
        {
            var msg = $"Sending order {order.OrderId} {contract.Symbol} {order.Action} {order.OrderType} at {order.LmtPrice} qty:{order.TotalQuantity}";
            visitor.OrderLog = string.IsNullOrWhiteSpace(visitor.OrderLog)
                ? msg
                : visitor.OrderLog + Environment.NewLine + msg;
        }

        private static DateTime GetLast10BarsEndTime()
        {
            TimeOnly nowTimeOnly = TimeOnly.FromDateTime(DateTime.Now);
            DateOnly nowDateOnly = nowTimeOnly < App.SESSION_START
                ? DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
                : DateOnly.FromDateTime(DateTime.Now);
            
            if (nowDateOnly.DayOfWeek == DayOfWeek.Sunday)
            {
                nowDateOnly = nowDateOnly.AddDays(-2);
            }
            else if (nowDateOnly.DayOfWeek == DayOfWeek.Saturday)
            {
                nowDateOnly = nowDateOnly.AddDays(-1);
            }

            var sessionStart = nowDateOnly.ToDateTime(App.SESSION_START);
            var utcOffset = GetUtcOffset();
            sessionStart = sessionStart - utcOffset;
            var result = sessionStart.AddMinutes(App.BAR_SIZE_IN_MINUTES * App.STD_DEV_PERIOD);
            return result;
        }

        private static TimeSpan GetUtcOffset()
        {
            DateTime utc = DateTime.UtcNow;
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local);
            var utcOffset = localDateTime - utc;
            return utcOffset;
        }

        private static async Task<int> GetNextOrderIdAsync(IPositionsVisitor visitor)
        {
            if (visitor.OrdersSent > 0)
                await visitor.IbHost.ReqIdsAsync(-1);
            var orderId = visitor.IbHost.NextOrderId;
            return orderId;
        }

        private static Order CreateOrder(
            int orderId,
            string action,
            string orderType,
            double lmtPrice,
            int totalQuantity,
            bool outsideRth = false,
            string ocaGroup = "")
        {
            var order = new Order()
            {
                OrderId = orderId,
                Action = action,
                OrderType = orderType,
                LmtPrice = lmtPrice,
                TotalQuantity = totalQuantity,
                Transmit = true,
                OutsideRth = outsideRth,
            };

            if(!string.IsNullOrEmpty(ocaGroup))
            {
                order.OcaGroup = ocaGroup;
                order.OcaType = 1;
            }

            return order;
        }
    }
}
