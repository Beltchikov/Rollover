namespace SignalsTest
{
    public class MacdShould
    {
        // Expected precision in percent
        double EXPECTED_PRECISION = 0.01;

        [Theory]
        [InlineData(0.085)]
        void CalculateFirstMacd(
            double expectedMacdValue)
        {
            var macd = Factory.Create("MACD");

            // Without adding the first data point
            Assert.True(EqualWithPrecision(expectedMacdValue, macd.MacdValue(0), EXPECTED_PRECISION));

            // With the first data point
            macd.AddDataPoint(DateTimeOffset.Parse("16.07.2024 10:00:00 +01:00"), 234.8);
            Assert.True(EqualWithPrecision(expectedMacdValue, macd.MacdValue(0), EXPECTED_PRECISION));

        }

        [Theory]
        [InlineData("16.07.2024 10:00:00 +01:00", 234.8,
            "16.07.2024 10:05:00 +01:00",234.43,235.181384, 235.1735185, 0.007866097, 0.101573219)]
        void CalculateSubsequentValues(
            string timeString0,
            double dataPointValue0,
            string timeString1,
            double dataPointValue1,
            double expectedFastEma1,
            double expectedSlowEma1,
            double expectedMacdValue1,
            double expectedSignal1)
        {
            var macd = Factory.Create("MACD");

            macd.AddDataPoint(DateTimeOffset.Parse(timeString0), dataPointValue0);
            macd.AddDataPoint(DateTimeOffset.Parse(timeString1), dataPointValue1);

            // Fast EMA
            var fastEma1 = macd.FastEma(1);
            

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
