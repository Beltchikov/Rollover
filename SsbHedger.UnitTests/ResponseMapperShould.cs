using SsbHedger.ResponseProcessing;

namespace SsbHedger.UnitTests
{
    public class ResponseMapperShould
    {
        [Theory, AutoNSubstituteData]
        public void ThrowExceptionIfUnknownMessageType(
            ResponseMapper sut)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => sut.AddResponse(new ObsoleteAttribute()));
            Assert.StartsWith("Unknown message type", exception.Message);
        }

        [Theory, AutoNSubstituteData]
        public void NotGroupResponsesWithRefIdMinusOne(
            int code,
            string message, 
            ResponseMapper sut)
        {
            int count = 3;
            
            for (int i = 1; i <= count; i++)
            {
                ErrorInfo errorInfo = new ErrorInfo(-1, code, message, null);
                sut.AddResponse(errorInfo); 
            }
            List<ReqIdAndResponses> responses = sut.GetGrouppedResponses();

            Assert.Equal(count, responses.Count);
        }
    }
}
