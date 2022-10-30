using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.ResponseProcessing;

namespace SsbHedger.UnitTests
{
    public class ResponseHandlerShould
    {

        [Theory, AutoNSubstituteData]
        public void CallEnqueueOnError(
            int reqId, int code, string message, Exception exception,
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.OnError(reqId, code, message, exception);
            queue.Received().Enqueue(Arg.Is<object>(a => 
                a.GetType() == typeof(ErrorMessage)
                && ((ErrorMessage)a).ReqId == reqId
                && ((ErrorMessage)a).Code == code
                && ((ErrorMessage)a).Message == message
            ));
        }
    }
}
