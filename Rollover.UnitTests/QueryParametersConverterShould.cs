using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class QueryParametersConverterShould
    {
        [Fact]
        public void ReturnCorrectParametersForMnq()
        {
            ITrackedSymbol derivative = new TrackedSymbol
            {
                SecType = "FOP",
                Symbol = "MNQ",
                Currency = "USD",
                Exchange = null
            };

            IQueryParametersConverter sut = new QueryParametersConverter();
            var underlying = sut.TrackedSymbolForReqSecDefOptParams(derivative);

            Assert.Equal("IND", underlying.SecType);
            Assert.Equal("GLOBEX", underlying.Exchange);
        }
    }
}
