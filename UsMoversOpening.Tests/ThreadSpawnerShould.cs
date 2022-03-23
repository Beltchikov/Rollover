using AutoFixture.Xunit2;
using NSubstitute;
using UsMoversOpening.Threading;
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
            umoAgent.Received().Run(sut, Arg.Any<IThreadWrapper>(), Arg.Any<IThreadWrapper>());
        }
    }
}
