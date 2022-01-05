using AutoFixture.Xunit2;
using IBApi;
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

        [Theory, AutoNSubstituteData]
        public void CallIbClientReqPositions(
            [Frozen] IIbClientWrapper ibClient,
            MessageCollector sut)
        {
            sut.reqPositions();
            ibClient.Received().reqPositions();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientReqContractDetails(
            [Frozen] IIbClientWrapper ibClient,
            Contract contract,
            MessageCollector sut)
        {
            sut.reqContractDetails(contract);
            ibClient.Received().reqContractDetails(Arg.Any<int>(), contract);
        }

        //[Theory, AutoNSubstituteData]
        //public void CallIbClientReqSecDefOptParams(
        //    [Frozen] IIbClientWrapper ibClient,
        //    MessageCollector sut)
        //{
        //    sut.reqSecDefOptParams(
        //        Arg.Any<string>(),
        //        Arg.Any<string>(),
        //        Arg.Any<string>(),
        //        Arg.Any<int>());
        //    ibClient.ReceivedWithAnyArgs().reqSecDefOptParams(
        //       default,
        //       default,
        //       default,
        //       default,
        //       default
        //       );
        //}
    }
}
