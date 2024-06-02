using IBApi;
using IbClient.IbHost;
using System.Collections.ObjectModel;

namespace IbClient.UnitTests
{
    public class IbHostShould
    {
        [Fact]
        public async Task PlaceOrderAndWaitExecutionAsync()
        {
            IIbHost sut = new IbHost.IbHost();
            sut.Consumer = CreateIbConsumer();

            Contract contract = CreateTestContract();
            Order order = CreateTestOrder();
            double avrFillPrice = await sut.PlaceOrderAndWaitExecution(contract, order);
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

        private Order CreateTestOrder()
        {
            var order = new Order()
            {
                OrderId = 111,
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