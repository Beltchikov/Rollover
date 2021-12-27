using IBApi;
using Rollover.Ib;
using Xunit;

namespace Rollover.UnitTests
{
    public class UnderlyingConverterShould
    {
        [Fact]
        public void ReturnUnderlyingForMnq()
        {
            Contract derivative = new Contract
            {
                SecType = "FOP",
                Symbol = "MNQ",
                Currency = "USD",
                Exchange = null
            };

            IUnderlyingConverter sut = new UnderlyingConverter();
            var underlying = sut.GetUnderlying(derivative);

            Assert.Equal("IND", underlying.SecType);
            Assert.Equal("GLOBEX", underlying.Exchange);
        }
    }
}
