using AutoFixture.Xunit2;
using NSubstitute;
using UsMoversOpening.Configuration;
using Xunit;

namespace UsMoversOpening.Tests
{
    public class ThreadSpawnerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallUmoAgentStart(
            [Frozen] IUmoAgent umoAgent,
            ThreadSpawner sut)
        {
            sut.Run();
            umoAgent.Received().Run(sut, Arg.Any<IThreadWrapper>());
        }

        [Theory, AutoNSubstituteData]
        public void CallCofigurationManager(
            [Frozen] IConfigurationManager configurationManager,
            ThreadSpawner sut)
        {
            sut.Run();
            configurationManager.Received().GetConfiguration();
        }
    }
}
