using IbClient.messages;
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
            Assert.Equal(0, sut.Count);
        }

        [Theory, AutoNSubstituteData]
        public void NotGroupResponsesWithDifferentRefId(
            IBApi.Contract contract,
            IBApi.Order order,
            IBApi.OrderState orderState,
            ResponseMapper sut)
        {
            var message1 = new OpenOrderMessage(1, contract, order, orderState);
            sut.AddResponse(message1);
            var message2 = new OpenOrderMessage(2, contract, order, orderState);
            sut.AddResponse(message2);
            
            List<ReqIdAndResponses> responses = sut.GetGrouppedResponses();

            Assert.Equal(2, responses.Count);
            Assert.Equal(0, sut.Count);
        }
    }
}
