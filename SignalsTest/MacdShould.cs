using Ta.Indicators;

namespace SignalsTest
{
    public class MacdShould
    {
        [Theory]
        [InlineData(0.085, 0.01)]
        void CalculateFirstMacdCorrectly(
            double expectedMacdValue,
            double expectedPrecisionInPercent)
        {
            var macd = Factory.Create("MACD");

            // Without adding the first data point
            var difference = Math.Abs(expectedMacdValue - macd.MacdValue(0));
            var expetcedDifference = expectedMacdValue * expectedPrecisionInPercent;
            Assert.True(difference < expetcedDifference);

            // With the first data point
            macd.AddDataPoint(DateTimeOffset.Parse("16.07.2024 10:00:00 +01:00"), 234.8);
            difference = Math.Abs(expectedMacdValue - macd.MacdValue(0));
            expetcedDifference = expectedMacdValue * expectedPrecisionInPercent;
            Assert.True(difference < expetcedDifference);
        }
    }

    public class Factory
    {
        public static Macd Create(string name)
        {
            return name.ToUpper() switch
            {
                "MACD" => new Macd(12, 26, 9, 235.318, 235.233, 0.125),
                _ => throw new NotImplementedException($"Not implemented for{name}"),
            };
        }
    }
}
