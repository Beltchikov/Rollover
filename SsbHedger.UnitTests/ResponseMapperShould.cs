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
    }
}
