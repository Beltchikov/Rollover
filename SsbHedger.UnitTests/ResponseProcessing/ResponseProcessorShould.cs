using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing;

namespace SsbHedger.UnitTests.ResponseProcessing
{
    public class ResponseProcessorShould
    {
        [Theory, AutoNSubstituteData]
        public void CallDispatcherInvokeForErrorInfoMessage(
            int reqId,
            ErrorInfo errorInfo1,
            ErrorInfo errorInfo2,
            [Frozen] IDispatcherAbstraction dispatcher,
            ResponseProcessor sut)
        {
            var reqIdAndResponses = new ReqIdAndResponses(
                reqId, 
                new List<object> { errorInfo1, errorInfo2});

            sut.Process(reqIdAndResponses);
            dispatcher.Received().Invoke(Arg.Any<Action>());
        }

    }
}
