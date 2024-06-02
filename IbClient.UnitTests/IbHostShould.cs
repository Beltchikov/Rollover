using IBApi;
using IbClient.IbHost;
using IBSampleApp.messages;
using System.Collections.ObjectModel;

namespace IbClient.UnitTests
{
    public class IbHostShould
    {
        [Fact]
        public async Task PlaceOrderAndWaitExecutionAsync()
        {
            const int DELAY = 10000;
            const string FILLED = "FILLED";
            int orderId = 111;
            double avrFillPriceExpected = 85.56;
            
            IIbHost sut = new IbHost.IbHost();
            sut.Consumer = CreateIbConsumer();
            sut.OrderResponseMapper = new RequestResponseMapper();

            OrderStatusMessage filledOrderStatusMessage = CreateOrderStatusMessage(orderId, FILLED, avrFillPriceExpected);

            // Starts new async operation.
            // Main execution does not wait for it.
            // The operation writes messages in the RequestResponseMapper after a delay.

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => {
                Thread.Sleep(DELAY);

                var noiseResponses = Enumerable.Range(1, 10)
                .Select(x => CreateOrderStatusMessage(x, x.ToString(), (double)x  + 0.99))
                .ToList();
                noiseResponses.ForEach(nr => sut.OrderResponseMapper.AddResponse(nr.OrderId, nr));

                sut.OrderResponseMapper.AddResponse(orderId, filledOrderStatusMessage);

            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // Continues the main execution path.
            Contract contract = CreateTestContract();
            Order order = CreateTestOrder(orderId);
            double avrFillPrice = await sut.PlaceOrderAndWaitForExecution(contract, order);

            // 
            Assert.Equal(avrFillPriceExpected, avrFillPrice);   
        }

        private OrderStatusMessage CreateOrderStatusMessage(int orderId, string status, double avrFillPrice)
        {
            var orderStatusMessage = new OrderStatusMessage(orderId, status, 0, 0, avrFillPrice, 0, 0, 0, 0, "", 0);
           return orderStatusMessage;
        }

        class IbConsumerFake : IIbConsumer
        {
            bool _connectedToTws = false;
            ObservableCollection<string> _twsMessageCollection = null!;
            bool IIbConsumer.ConnectedToTws { get => _connectedToTws; set => _connectedToTws= value; }
            ObservableCollection<string> IIbConsumer.TwsMessageCollection { get => _twsMessageCollection; set => _twsMessageCollection = value; }
        }

        private IIbConsumer CreateIbConsumer()
        {
            return new IbConsumerFake();
        }

        private Order CreateTestOrder(int id)
        {
            var order = new Order()
            {
                OrderId = id,
                Action = "BUY",
                OrderType = "LMT",
                LmtPrice = 100,
                TotalQuantity = 99,
                Transmit = true,
                OutsideRth = true,
            };
            return order;
        }

        private Contract CreateTestContract()
        {
            Contract contract = new Contract()
            {
                ConId = 265598,
                Symbol = "AAPL",
                SecType = "STK",
                Currency = "USD",
                Exchange = "SMART"
            };

            return contract;    
        }
    }
}