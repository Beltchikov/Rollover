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
        public void CallInputThreadStart(
            IThreadSpawner threadSpawner,
            IThreadWrapper inputThread,
            UmoAgent sut)
        {
            sut.Run(threadSpawner, inputThread);
            inputThread.Received().Start();
        }

        [Theory, AutoNSubstituteData]
        public void CallCofigurationManager(
            IThreadSpawner threadSpawner, 
            IThreadWrapper inputThread,
            [Frozen] IConfigurationManager configurationManager,
            UmoAgent sut)
        {
            var configuration = new Configuration.Configuration
            {
                TimeToBuy = $"14:35"
            };
            configurationManager.GetConfiguration().Returns(configuration);

            sut.Run(threadSpawner, inputThread);
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
