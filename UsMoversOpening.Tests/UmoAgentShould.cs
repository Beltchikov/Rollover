using AutoFixture.Xunit2;
using NSubstitute;
using System;
using UsMoversOpening.Configuration;
using UsMoversOpening.Helper;
using Xunit;

namespace UsMoversOpening.Tests
{
    public class UmoAgentShould
    {
        [Theory, AutoNSubstituteData]
        public void CallCofigurationManager(
            [Frozen] IConfigurationManager configurationManager,
            [Frozen] IConsoleWrapper consoleWrapper,
            UmoAgent sut)
        {
            var configuration = new Configuration.Configuration
            {
                TimeToBuy = $"14:35"
            };
            configurationManager.GetConfiguration().Returns(configuration);

            consoleWrapper.ReadLine().Returns("Some input", "q");

            sut.Run();
            configurationManager.Received().GetConfiguration();
        }

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
