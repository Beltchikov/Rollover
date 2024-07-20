using Ta.Indicators;

namespace SignalsTest
{

    public class MacdShould
    {
        // Expected precision in percent
        double EXPECTED_PRECISION = 0.01;

        [Theory]
        [ClassData(typeof(HistoricalDataPoints))]
        void CalculateFirstValues(DataPoints dataPoints, HistoricalDataPoints.ExpectedValues expectedValues)
        {
            //var macd = Factory.Create("MACD");

            //// Without adding the first data point
            //Assert.True(EqualWithPrecision(macd.FirstFastEma, macd.FastEma(0), EXPECTED_PRECISION));
            //Assert.True(EqualWithPrecision(macd.FirstSlowEma, macd.SlowEma(0), EXPECTED_PRECISION));
            //Assert.True(EqualWithPrecision(expectedMacdValue, macd.MacdValue(0), EXPECTED_PRECISION));
            //Assert.True(EqualWithPrecision(macd.FirstSignal, macd.Signal(0), EXPECTED_PRECISION));

            //// With the first data point
            //macd.AddDataPoint(DateTimeOffset.Parse("16.07.2024 10:00:00 +01:00"), 234.8);

            //Assert.True(EqualWithPrecision(macd.FirstFastEma, macd.FastEma(0), EXPECTED_PRECISION));
            //Assert.True(EqualWithPrecision(macd.FirstSlowEma, macd.SlowEma(0), EXPECTED_PRECISION));
            //Assert.True(EqualWithPrecision(expectedMacdValue, macd.MacdValue(0), EXPECTED_PRECISION));
            //Assert.True(EqualWithPrecision(macd.FirstSignal, macd.Signal(0), EXPECTED_PRECISION));

            var macd = Factory.Create("MACD");

            // Without adding the historical data points
            Assert.True(EqualWithPrecision(macd.FirstFastEma, macd.FastEmaBarsAgo(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(macd.FirstSlowEma, macd.SlowEmaBarsAgo(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(expectedValues[0].MacdValue, macd.MacdValueBarsAgo(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(macd.FirstSignal, macd.SignalBarsAgo(0), EXPECTED_PRECISION));

            // With historical data points
            macd.AddDataPoints(dataPoints);

            Assert.True(EqualWithPrecision(expectedValues.BarsAgo(0).FastEma, macd.FastEmaBarsAgo(0), EXPECTED_PRECISION));


        }

        /// <summary>
        /// EqualWithPrecision
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="precision">Precision in percent</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool EqualWithPrecision(double expected, double actual, double precision)
        {
            var difference = Math.Abs(expected - actual);
            var expectedDifference = expected * precision;
            return difference < expectedDifference;
        }
    }
}
