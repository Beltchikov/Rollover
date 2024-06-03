﻿using IBApi;
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
            DateTime last10BarsEndTime = GetLast10BarsEndTime();
            string endDateTime = last10BarsEndTime.ToString(App.FORMAT_STRING_API);
            string durationString = $"{App.BAR_SIZE_IN_MINUTES * App.STD_DEV_PERIOD * 60} S";
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

            var midPrices = lastHistoricalDataMessages.Select(m => Math.Round((m.Close + m.Open) / 2, 2));
            var twoStdDev = midPrices.StandardDeviation() * 2;
            var qty = (int)Math.Round(App.RISK_IN_USD / twoStdDev, 0);
            if (qty <= 0)
            {
                MessageBox.Show($"The calculated quantity is {qty}. The execution of the command stops.");
                return;
            }

            int orderId = await GetNextOrderIdAsync(visitor);
            Order order = CreateOrder(orderId, "BUY", visitor.InstrumentToTrade.AskPrice, qty);
            double avrFillPrice = await visitor.IbHost.PlaceOrderAndWaitForExecution(contract, order);

            // Wait for order execution

            int orderIdTakeProfit = await GetNextOrderIdAsync(visitor);
            // 2 std. dev correspond to probability of 2,35%. Given RRR of 2 and some amount for commission, we use 5% for TP
            var tpDistance = App.RISK_IN_USD / qty * 0.05; 
            var tpPrice = Math.Round(avrFillPrice + tpDistance, 2);
            Order orderTakeProfit = CreateOrder(orderIdTakeProfit, "SELL", tpPrice, qty);

            var result = await visitor.IbHost.PlaceOrderAsync(contract, orderTakeProfit, App.TIMEOUT);

            if (result.ErrorMessage != "")
            {
                var msg = $"ConId={contract.ConId} {contract.Symbol} error:{result.ErrorMessage}";
                visitor.TwsMessageCollection.Add(msg);
                MessageBox.Show(msg);

            }
            else if (result.OrderState != null)
            {
                var msg = $"ConId={contract.ConId} {contract.Symbol} take profit order submitted.";
                visitor.TwsMessageCollection.Add(msg);
                MessageBox.Show(msg);
            }
            else throw new Exception("Unexpected. Both ErrorMessage nad OrderState are not set.");
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
    }
}
