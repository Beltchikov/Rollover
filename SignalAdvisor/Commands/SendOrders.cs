﻿using IBApi;
using System.Windows;

namespace SignalAdvisor.Commands
{
    public class SendOrders
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {

            (Order? orderBuyParent, Order? orderBuyStop, Order? orderBuyProfit) = await CreateOrdersAsync(visitor);
            if (orderBuyParent == null || orderBuyStop == null || orderBuyProfit == null)
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

            await DoSendOrder(visitor, contract, orderBuyParent);
            await DoSendOrder(visitor, contract, orderBuyStop);
            await DoSendOrder(visitor, contract, orderBuyProfit);
        }

        private static async Task<(Order?, Order?, Order?)> CreateOrdersAsync(IPositionsVisitor visitor)
        {
            var askPrice = visitor.InstrumentToTrade.AskPrice;
            if (askPrice <= 0)
            {
                MessageBox.Show($"The ask price is {askPrice}. The market is probably closed. The execution of the command stops.");
                return (null, null, null);
            }

            var orderBuyId = visitor.IbHost.NextOrderId;
            var orderParent = new Order()
            {
                OrderId = orderBuyId,
                Action = "BUY",
                OrderType = "LMT",
                LmtPrice = askPrice,
                TotalQuantity = visitor.InstrumentToTrade.Quantity,
                Transmit = false
            };

            Order orderStop = new()
            {
                OrderId = orderParent.OrderId + 1,
                Action = "SELL",
                OrderType = "MIDPRICE",
                LmtPrice = visitor.InstrumentToTrade.CalculateStopLossPrice(askPrice),  // Only for the visual feedback on a chart. Should be removed after positioning.
                TotalQuantity = visitor.InstrumentToTrade.Quantity,
                ParentId = orderParent.OrderId,
                Transmit = false
            };

            PriceCondition priceConditionStop = (PriceCondition)OrderCondition.Create(OrderConditionType.Price);
            priceConditionStop.ConId = visitor.InstrumentToTrade.ConId;
            priceConditionStop.Exchange = visitor.InstrumentToTrade.Exchange;
            priceConditionStop.IsMore = false;
            priceConditionStop.Price = visitor.InstrumentToTrade.CalculateStopLossPrice(askPrice);
            orderStop.Conditions.Add(priceConditionStop);

            // 
            Order orderProfit = new()
            {
                OrderId = orderParent.OrderId + 2,
                Action = "SELL",
                OrderType = "LMT",
                LmtPrice = visitor.InstrumentToTrade.CalculateTakeProfitPrice(askPrice),
                TotalQuantity = visitor.InstrumentToTrade.Quantity,
                ParentId = orderParent.OrderId,
                Transmit = true
            };

            return (orderParent, orderStop, orderProfit);
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
