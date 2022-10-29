using AutoFixture.Xunit2;
using IbClient;
using NSubstitute;

namespace SsbHedger.UnitTests
{
    public class LogicShould
    {
        [Theory, AutoNSubstituteData]
        public void CallIbClientConnectAndStartReaderThread(
            [Frozen] IIBClient ibClient,
            Logic sut)
        {
            sut.Execute();
            ibClient.Received().ConnectAndStartReaderThread(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>());
        }

    }
}