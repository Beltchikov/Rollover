using Ta.Indicators;

namespace SignalsTest
{
    public class MacdShould
    {
        [Theory]
        [InlineData(12, 26, 9, 235.318, 235.233, 0.125, 0.085, 0.01)]
        void CalculateFirstMacdCorrectly(
            int fastEmaPeriod,
            int slowEmaPeriod,
            int signalPeriod,
            double firstFastEma,
            double firstSlowEma,
            double firstSignal,
            double expectedMacdValue,
            double expectedPrecisionInPercent)
        {
            var macd = new  Macd(
                fastEmaPeriod,
                slowEmaPeriod,
                signalPeriod,
                firstFastEma,
                firstSlowEma,
                firstSignal);

            var difference = Math.Abs(expectedMacdValue - macd.MacdValue());
            var expetcedDifference = expectedMacdValue * expectedPrecisionInPercent;
            Assert.True(difference < expetcedDifference);
        }
    }
}
