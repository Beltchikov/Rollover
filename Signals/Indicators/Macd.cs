using System.Linq;

namespace Ta.Indicators
{
    public class Macd
    {
        //private List<DataPoint> DataPoints;
        private DataPoints DataPoints;
        private readonly List<double> _fastEmaValues;
        private readonly List<double> _slowEmaValues;
        private readonly List<double> _macdValues;

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
            DataPoints = [];
            _fastEmaValues = [];
            _slowEmaValues = [];
            _macdValues = [];

            FastEmaPeriod = fastEmaPeriod;
            SlowEmaPeriod = slowEmaPeriod;
            SignalPeriod = signalPeriod;

            FirstFastEma = firstFastEma;
            FirstSlowEma = firstSlowEma;
            FirstSignal = firstSignal;
        }

        public double FastEmaBarsAgo(int barsAgo)
        {
            return DataPoints.Count == 0
                ? FirstFastEma
                : _fastEmaValues[DataPoints.Count - 1 - barsAgo];   
        }

        public double SlowEmaBarsAgo(int barsAgo)
        {
            return DataPoints.Count == 0
                ? FirstSlowEma
                : _slowEmaValues[DataPoints.Count - 1 - barsAgo];
        }

        public double MacdValueBarsAgo(int barsAgo)
        {
            if (DataPoints.Count == 0)
                return FirstFastEma - FirstSlowEma;
            else
                return _macdValues[DataPoints.Count - 1 - barsAgo];
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

            // foreach(var dataPoint in DataPoints)
            //      FastEmaValues.Add(...)
            //      SlowEmaValues.Add(...)
            //      MacdValues.Add(...)
            //      SignalValues.Add(...)

            for(int i = 0; i < DataPoints.Count; i++)
            {
                DataPoint dataPoint = DataPoints.BarsAgo(DataPoints.Count -1 - i);

                double fastEma;
                if (i == 0)
                {
                    fastEma = FirstFastEma;
                }
                else
                {
                    //= B7 * (2 / ($B$1 + 1))+C6 * (1 - (2 / ($B$1 + 1)))
                    var b7 = dataPoint.Value;
                    var b7Weighted = b7 * (2d / (FastEmaPeriod + 1d));
                    var c6 = _fastEmaValues[i-1];
                    var c6Weighted = c6 * (1 - (2d / (FastEmaPeriod + 1d)));
                    fastEma = b7Weighted + c6Weighted;
                }
                _fastEmaValues.Add(fastEma);

                double slowEma;
                if (i == 0)
                {
                    slowEma = FirstSlowEma;
                }
                else
                {
                    //=B7*(2/($E$1+1))+D6*(1-(2/($E$1+1)))
                    var b7 = dataPoint.Value;
                    var b7Weighted = b7 * (2d / (SlowEmaPeriod + 1d));
                    var c6 = _slowEmaValues[i - 1];
                    var c6Weighted = c6 * (1 - (2d / (SlowEmaPeriod + 1d)));
                    slowEma = b7Weighted + c6Weighted;
                }
                _slowEmaValues.Add(slowEma);

                double macdValue = fastEma - slowEma;
                _macdValues.Add(macdValue);

                double signal;

            }
        }


    }
}
