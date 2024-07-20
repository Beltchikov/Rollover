namespace Ta.Indicators
{
    public class Macd
    {
        private readonly DataPoints DataPoints;
        private readonly List<double> _fastEmaValues;
        private readonly List<double> _slowEmaValues;
        private readonly List<double> _macdValues;
        private readonly List<double> _signalValues;

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
            _signalValues = [];

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
            return DataPoints.Count == 0
                ? FirstFastEma - FirstSlowEma
                : _macdValues[DataPoints.Count - 1 - barsAgo];
        }

        public double SignalBarsAgo(int barsAgo)
        {
            return DataPoints.Count == 0
                ? FirstSignal
                : _signalValues[DataPoints.Count - 1 - barsAgo];
        }

        public void AddDataPoints(DataPoints dataPoints)
        {
            DataPoints.AddRange(dataPoints);

            for (int i = 0; i < DataPoints.Count; i++)
            {
                DataPoint dataPoint = DataPoints.BarsAgo(DataPoints.Count - 1 - i);

                double fastEma;
                if (i == 0)
                {
                    fastEma = FirstFastEma;
                }
                else
                {
                    fastEma = dataPoint.Value * (2d / (FastEmaPeriod + 1d)) 
                        + _fastEmaValues[i - 1] * (1 - (2d / (FastEmaPeriod + 1d)));
                }
                _fastEmaValues.Add(fastEma);

                double slowEma;
                if (i == 0)
                {
                    slowEma = FirstSlowEma;
                }
                else
                {
                    slowEma = dataPoint.Value * (2d / (SlowEmaPeriod + 1d)) 
                        + _slowEmaValues[i - 1] * (1 - (2d / (SlowEmaPeriod + 1d)));
                }
                _slowEmaValues.Add(slowEma);

                double macdValue = fastEma - slowEma;
                _macdValues.Add(macdValue);

                double signal;
                if (i == 0)
                {
                    signal = FirstSignal;
                }
                else
                {
                    signal = macdValue * (2d / (SignalPeriod + 1d)) 
                        + _signalValues[i - 1] * (1 - (2d / (SignalPeriod + 1d)));
                }
                _signalValues.Add(signal);
            }
        }
    }
}
