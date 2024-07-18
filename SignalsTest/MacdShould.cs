namespace SignalsTest
{
    public class MacdShould
    {
        // Expected precision in percent
        double EXPECTED_PRECISION = 0.01;

        [Theory]
        [InlineData(0.085)]
        void CalculateFirstValues(
            double expectedMacdValue)
        {
            var macd = Factory.Create("MACD");

            // Without adding the first data point
            Assert.True(EqualWithPrecision(macd.FirstFastEma, macd.FastEma(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(macd.FirstSlowEma, macd.SlowEma(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(expectedMacdValue, macd.MacdValue(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(macd.FirstSignal, macd.Signal(0), EXPECTED_PRECISION));

            // With the first data point
            macd.AddDataPoint(DateTimeOffset.Parse("16.07.2024 10:00:00 +01:00"), 234.8);

            Assert.True(EqualWithPrecision(macd.FirstFastEma, macd.FastEma(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(macd.FirstSlowEma, macd.SlowEma(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(expectedMacdValue, macd.MacdValue(0), EXPECTED_PRECISION));
            Assert.True(EqualWithPrecision(macd.FirstSignal, macd.Signal(0), EXPECTED_PRECISION));
        }

        [Theory]
        [InlineData(new string[] { "16.07.2024 10:00:00 +01:00", "16.07.2024 10:05:00 +01:00" },
            new double[] { 234.8, 234.43 },
            235.181384, 235.1735185, 0.007866097, 0.101573219)]
        void CalculateSubsequentValues(
            string[] timeStrings,
            double[] dataPointValues,
            double expectedFastEma,
            double expectedSlowEma,
            double expectedMacdValue,
            double expectedSignal)
        {
            var macd = Factory.Create("MACD");

            macd.AddDataPoint(DateTimeOffset.Parse(timeStrings[0]), dataPointValues[0]);
            macd.AddDataPoint(DateTimeOffset.Parse(timeStrings[1]), dataPointValues[1]);

            // Fast EMA
            var fastEma = macd.FastEma(0);
            Assert.True(EqualWithPrecision(expectedFastEma, fastEma, EXPECTED_PRECISION));

            // Slow EMA

            // MACD
            //var macdValue1 = macd.MacdValue(1);
            //var difference = Math.Abs(expectedMacdValue1 - macdValue1);
            //var expectedDifference = expectedMacdValue1 * EXPECTED_PRECISION_IN_PERCENT;
            //Assert.True(difference < expectedDifference);

            // Signal
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
