using AutoFixture.Xunit2;
using NSubstitute;
using System;
using UsMoversOpening.Configuration;
using Xunit;

namespace UsMoversOpening.Tests
{
    public class UmoAgentShould
    {
        [Theory, AutoNSubstituteData]
        public void CallCofigurationManager(
            [Frozen] IConfigurationManager configurationManager,
            UmoAgent sut)
        {
            var configuration = new Configuration.Configuration
            {
                TimeToBuy = $"14:35"
            };
            configurationManager.GetConfiguration().Returns(configuration);

            sut.Run();
            configurationManager.Received().GetConfiguration();
        }

        [Theory, AutoNSubstituteData]
        public void NotCallSendOrdersBeforeConfiguredTime(
          [Frozen] IConfigurationManager configurationManager,
          [Frozen] IStocksBuyer stocksBuyer,
          UmoAgent sut)
        {
            var configuration = new Configuration.Configuration
            {
                TimeToBuy = $"{DateTime.Now.AddHours(-1).Hour}:{DateTime.Now.AddHours(-1).Minute}"
            };
            configurationManager.GetConfiguration().Returns(configuration);

            sut.Run();
            stocksBuyer.DidNotReceive().SendOrders();
        }

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



        //[Theory, AutoNSubstituteData]
        //public void NotCallSendOrdersMultipleTimes(
        //  [Frozen] IConfigurationManager configurationManager,
        //  [Frozen] IStocksBuyer stocksBuyer,
        //  UmoAgent sut)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
