using System.Collections;
using Ta.Indicators;

namespace SignalsTest
{
    internal class HistoricalDataPoints : IEnumerable<object[]>
    {
        internal record ExpectedValue(double FastEma, double SlowEma, double MacdValue, double Signal);

        internal class ExpectedValues
        {
            private List<ExpectedValue> _expectedValues = [];
            
            internal void Add(ExpectedValue expectedValue)
            {
                _expectedValues.Add(expectedValue);
            }
        }

        readonly List<object[]> _data = [];

        public HistoricalDataPoints()
        {
            //_data.Add([]);
            //_data.Add([]);
            //_data.Add([]);

            var dataPoints = new DataPoints();
            dataPoints.Add(new DataPoint(DateTimeOffset.Parse("16.07.2024 10:00:00 +01:00"), 234.8));
            dataPoints.Add(new DataPoint(DateTimeOffset.Parse("16.07.2024 10:05:00 +01:00"), 234.43));
            dataPoints.Add(new DataPoint(DateTimeOffset.Parse("16.07.2024 10:10:00 +01:00"), 235));

            var expectedValues = new ExpectedValues();
            expectedValues.Add(new ExpectedValue(235.318, 235.233, 0.085, 0.125));   
            expectedValues.Add(new ExpectedValue(235.1813846, 235.1735185, 0.007866097, 0.101573219));   
            expectedValues.Add(new ExpectedValue(235.1606653, 235.233, -0.007186005, 0.079821375));
            
            object[] testRow = [dataPoints, expectedValues];
            _data.Add(testRow); 

        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return ((IEnumerable<object[]>)_data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_data).GetEnumerator();
        }
    }

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
            //macd.AddDataPoints()

        }

        //[Theory]
        //[InlineData(new string[] { "16.07.2024 10:00:00 +01:00", "16.07.2024 10:05:00 +01:00" },
        //    new double[] { 234.8, 234.43 },
        //    235.181384, 235.1735185, 0.007866097, 0.101573219)]
        //void CalculateSubsequentValues(
        //    string[] timeStrings,
        //    double[] dataPointValues,
        //    double expectedFastEma,
        //    double expectedSlowEma,
        //    double expectedMacdValue,
        //    double expectedSignal)
        //{
        //    var macd = Factory.Create("MACD");

        //    macd.AddDataPoint(DateTimeOffset.Parse(timeStrings[0]), dataPointValues[0]);
        //    macd.AddDataPoint(DateTimeOffset.Parse(timeStrings[1]), dataPointValues[1]);

        //    // Fast EMA
        //    var fastEma = macd.FastEma(0);
        //    Assert.True(EqualWithPrecision(expectedFastEma, fastEma, EXPECTED_PRECISION));

        //    // Slow EMA

        //    // MACD
        //    //var macdValue1 = macd.MacdValue(1);
        //    //var difference = Math.Abs(expectedMacdValue1 - macdValue1);
        //    //var expectedDifference = expectedMacdValue1 * EXPECTED_PRECISION_IN_PERCENT;
        //    //Assert.True(difference < expectedDifference);

        //    // Signal
        //}

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
