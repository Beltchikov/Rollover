using AutoFixture.Xunit2;
using NSubstitute;
using System;
using UsMoversOpening.Configuration;
using UsMoversOpening.Helper;
using UsMoversOpening.IBApi;
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
            IThreadWrapper ibThread,
            UmoAgent sut)
        {
            sut.Run(threadSpawner, inputThread, ibThread);
            inputThread.Received().Start();
        }

        [Theory, AutoNSubstituteData]
        public void CallCofigurationManager(
            IThreadSpawner threadSpawner,
            IThreadWrapper inputThread,
            IThreadWrapper ibThread,
            [Frozen] IConfigurationManager configurationManager,
            UmoAgent sut)
        {
            var configuration = new Configuration.Configuration
            {
                TimeToBuy = $"14:35"
            };
            configurationManager.GetConfiguration().Returns(configuration);

            sut.Run(threadSpawner, inputThread, ibThread);
            configurationManager.Received().GetConfiguration();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientEConnect(
            IThreadSpawner threadSpawner,
            IThreadWrapper inputThread,
            IThreadWrapper ibThread,
            [Frozen] IConfigurationManager configurationManager,
            [Frozen] IIbClientWrapper iIbClientWrapper,
            UmoAgent sut)
        {
            var configuration = new Configuration.Configuration{};
            configurationManager.GetConfiguration().Returns(configuration);

            sut.Run(threadSpawner, inputThread, ibThread);
            iIbClientWrapper.Received().eConnect(
                configuration.Host,
                configuration.Port,
                configuration.ClientId);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbThreadStart(
            IThreadSpawner threadSpawner,
            IThreadWrapper inputThread,
            IThreadWrapper ibThread,
            UmoAgent sut)
        {
            sut.Run(threadSpawner, inputThread, ibThread);
            ibThread.Received().Start();
        }

        [Theory, AutoNSubstituteData]
        public void NotCallSendOrdersMultipleTimes(
            IThreadSpawner threadSpawner,
            IThreadWrapper inputThread,
            IThreadWrapper ibThread,
            [Frozen] IStocksBuyer stocksBuyer,
            UmoAgent sut)
        {
            stocksBuyer.Triggered(Arg.Any<string>()).Returns(true, true);
            threadSpawner.ExitFlagInputThread.Returns(false, true);

            sut.Run(threadSpawner, inputThread, ibThread);

            stocksBuyer.Received().SendOrders();
        }
    }
}
