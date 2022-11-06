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
            ErrorInfo errorInfo,
            [Frozen] IDispatcherAbstraction dispatcher,
            ResponseProcessor sut)
        {
            sut.Process(errorInfo);
            dispatcher.Received().Invoke(Arg.Any<Action>());
        }

    }
}
