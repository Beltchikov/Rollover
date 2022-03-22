using AutoFixture.Xunit2;
using NSubstitute;
using System;
using UsMoversOpening.Configuration;
using UsMoversOpening.Helper;
using UsMoversOpening.Threading;
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

        [Theory, AutoNSubstituteData]
        public void NotCallSendOrdersMultipleTimes(
            IThreadSpawner threadSpawner,
            IThreadWrapper inputThread,
            [Frozen] IStocksBuyer stocksBuyer,
            UmoAgent sut)
        {
            stocksBuyer.Triggered(Arg.Any<string>()).Returns(true, true);
            threadSpawner.ExitFlagInputThread.Returns(false, true);

            sut.Run(threadSpawner, inputThread);

            stocksBuyer.Received().SendOrders();
        }
    }
}
