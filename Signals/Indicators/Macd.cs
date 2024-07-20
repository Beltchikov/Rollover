using System.Collections;

namespace Ta.Indicators
{
    public class Macd
    {
        //private List<DataPoint> DataPoints;
        private DataPoints DataPoints;

        public int FastEmaPeriod { get; private set; }
        public int SlowEmaPeriod { get; private set; }
        public int SignalPeriod { get; private set; }
        public double FirstFastEma { get; private set; }
        public double FirstSlowEma { get; private set; }
        public double FirstSignal { get; private set; }


        public Macd(
            int fastEmaPeriod,
            int slowEmaPeriod,
            int signalPeriod,
            double firstFastEma,
            double firstSlowEma,
            double firstSignal)
        {
            DataPoints = new DataPoints();

            FastEmaPeriod = fastEmaPeriod;
            SlowEmaPeriod = slowEmaPeriod;
            SignalPeriod = signalPeriod;

            FirstFastEma = firstFastEma;
            FirstSlowEma = firstSlowEma;
            FirstSignal = firstSignal;
        }

        public double FastEmaBarsAgo(int barsAgo)
        {
            if (DataPoints.Count <= 1)
                return FirstFastEma;
            else
            {
                //= B7 * (2 / ($B$1 + 1))+C6 * (1 - (2 / ($B$1 + 1)))
                var closeBarsAgo = DataPoints.BarsAgo(barsAgo).Value; // B7
               

                throw new NotImplementedException();
            }
        }

        public double SlowEmaBarsAgo(int barsAgo)
        {
            if (DataPoints.Count <= 1)
                return FirstSlowEma;
            else
            { throw new NotImplementedException(); }
        }

        public double MacdValueBarsAgo(int barsAgo)
        {
            if (DataPoints.Count <= 1)
                return FirstFastEma - FirstSlowEma;
            else
                throw new NotImplementedException();
        }

        public double SignalBarsAgo(int barsAgo)
        {
            if (DataPoints.Count <= 1)
                return FirstSignal;
            else
            { throw new NotImplementedException(); }
        }

        public void AddDataPoints(DataPoints dataPoints)
        {
            DataPoints.AddRange(dataPoints);
        }


    }

    public record DataPoint(DateTimeOffset Time, double Value);

    public class DataPoints : IEnumerable<DataPoint>    
    {
        private List<DataPoint> _dataPoints;

        public DataPoints()
        {
                _dataPoints = new List<DataPoint>();    
        }

        public int Count => _dataPoints.Count;

        public void Add(DataPoint dataPoint) {
            _dataPoints.Add(dataPoint);
        }

        public DataPoint BarsAgo(int barsAgo)
        {
            return _dataPoints[_dataPoints.Count - 1 - barsAgo];
        }

        public void AddRange(DataPoints dataPoints)
        {
            _dataPoints.AddRange(dataPoints);
        }

        public IEnumerator<DataPoint> GetEnumerator()
        {
            return ((IEnumerable<DataPoint>)_dataPoints).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dataPoints).GetEnumerator();
        }
    }
}
