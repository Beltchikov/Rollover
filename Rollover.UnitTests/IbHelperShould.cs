using Rollover.Helper;
using Xunit;

namespace Rollover.UnitTests
{
    public class IbHelperShould
    {
        [Theory]
        [InlineData(2022, 1, 3, "202203")]
        [InlineData(2022, 2, 3, "202203")]
        [InlineData(2022, 3, 3, "202203")]
        [InlineData(2022, 4, 3, "202206")]
        [InlineData(2022, 5, 3, "202206")]
        [InlineData(2022, 6, 3, "202206")]
        [InlineData(2022, 7, 3, "202209")]
        [InlineData(2022, 8, 3, "202209")]
        [InlineData(2022, 9, 3, "202209")]
        [InlineData(2022, 10, 3, "202212")]
        [InlineData(2022, 11, 3, "202212")]
        [InlineData(2022, 12, 3, "202212")]
        public void ReturnNextContractYearAndMonth(int year, int month, int period, string result)
        {
            Assert.Equal(result, IbHelper.NextContractYearAndMonth(year, month, period));
        }
    }
}
