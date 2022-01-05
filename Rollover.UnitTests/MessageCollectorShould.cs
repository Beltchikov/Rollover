using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Ib;
using Xunit;

namespace Rollover.UnitTests
{
    public class MessageCollectorShould
    {
        [Theory, AutoNSubstituteData]
        public void CallIbClientEConnect(
            [Frozen] IIbClientWrapper ibClient,
            MessageCollector sut)
        {
            sut.eConnect(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>());
            ibClient.Received().eConnect(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>());
        }
    }
}
