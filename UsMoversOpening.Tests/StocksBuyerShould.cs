using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        //[Theory, AutoNSubstituteData]
        //public void NotCallSendOrdersMultipleTimes(
        //  [Frozen] IConfigurationManager configurationManager,
        //  [Frozen] IStocksBuyer stocksBuyer,
        //  UmoAgent sut)
        //{
        //    throw new NotImplementedException();
        //}

        //[Theory, AutoNSubstituteData]
        //public void CallSendOrdersAtConfiguredTime(
        //   [Frozen] IConfigurationManager configurationManager,
        //   [Frozen] IStocksBuyer stocksBuyer,
        //   UmoAgent sut)
        //{
        //    var configuration = new Configuration.Configuration
        //    {
        //        TimeToBuy = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}"
        //    };
        //    configurationManager.GetConfiguration().Returns(configuration);

        //    sut.Run();
        //    stocksBuyer.Received().Buy();
        //}
    }
}
