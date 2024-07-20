using System.Collections;
using Ta.Indicators;
using static SignalsTest.HistoricalDataPoints;

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

            internal ExpectedValue this[int i]
            {
                get { return _expectedValues[i]; }
                set { _expectedValues[i] = value; }
            }

            internal ExpectedValue BarsAgo(int i)
            {
                return _expectedValues[_expectedValues.Count - 1 - i];
            }
        }

        readonly List<object[]> _data = [];

        public HistoricalDataPoints()
        {
            var dataPoints = new DataPoints
            {
                new DataPoint(DateTimeOffset.Parse("16.07.2024 10:00:00 +01:00"), 234.8),
                new DataPoint(DateTimeOffset.Parse("16.07.2024 10:05:00 +01:00"), 234.43),
                new DataPoint(DateTimeOffset.Parse("16.07.2024 10:10:00 +01:00"), 235)
            };

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
}
