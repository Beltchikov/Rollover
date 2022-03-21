using System;
using Xunit;

namespace UsMoversOpening.Tests
{
    public class StocksBuyerShould
    {
        [Fact]
        public void NotReturnTriggeredFalseBeforeConfiguredTime()
        {
            var sut = new StocksBuyer();
            string timeToBuy = $"{DateTime.Now.AddHours(1).Hour}:{DateTime.Now.AddHours(1).Minute}";

            var result = sut.Triggered(timeToBuy);
            Assert.False (result);  
        }

        [Fact]
        public void ReturnTriggeredTrueAfterConfiguredTime()
        {
            var sut = new StocksBuyer();
            string timeToBuy = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";

            var result = sut.Triggered(timeToBuy);
            Assert.True(result);
        }
    }
}
