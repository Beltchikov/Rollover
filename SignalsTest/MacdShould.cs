namespace SignalsTest
{
    public class MacdShould
    {
        double EXPECTED_PRECISION_IN_PERCENT = 0.01;

        [Theory]
        [InlineData(0.085)]
        void CalculateFirstMacd(
            double expectedMacdValue)
        {
            var macd = Factory.Create("MACD");

            // Without adding the first data point
            var difference = Math.Abs(expectedMacdValue - macd.MacdValue(0));
            var expetcedDifference = expectedMacdValue * EXPECTED_PRECISION_IN_PERCENT;
            Assert.True(difference < expetcedDifference);

            // With the first data point
            macd.AddDataPoint(DateTimeOffset.Parse("16.07.2024 10:00:00 +01:00"), 234.8);
            difference = Math.Abs(expectedMacdValue - macd.MacdValue(0));
            expetcedDifference = expectedMacdValue * EXPECTED_PRECISION_IN_PERCENT;
            Assert.True(difference < expetcedDifference);
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

            // Slow EMA
            
            // MACD
            //var macdValue1 = macd.MacdValue(1);
            //var difference = Math.Abs(expectedMacdValue1 - macdValue1);
            //var expectedDifference = expectedMacdValue1 * EXPECTED_PRECISION_IN_PERCENT;
            //Assert.True(difference < expectedDifference);

            // Signal
        }
    }
}
