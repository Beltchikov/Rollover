namespace Ta.Indicators
{
    public class Macd
    {
        private record DataPoint(DateTimeOffset Time, double Value);

        private List<DataPoint> DataPoints; 
        
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
            DataPoints = new List<DataPoint>();

            FastEmaPeriod = fastEmaPeriod;
            SlowEmaPeriod = slowEmaPeriod;
            SignalPeriod = signalPeriod;

            FirstFastEma = firstFastEma;
            FirstSlowEma = firstSlowEma;
            FirstSignal = firstSignal;
        }

        public double FastEma(int barsAgo)
        {
            if (DataPoints.Count <= 1)
                return FirstFastEma;
            else
                {throw new NotImplementedException();}
        }

        public double SlowEma(int barsAgo)
        {
            if (DataPoints.Count <= 1)
                return FirstSlowEma;
            else
            { throw new NotImplementedException(); }
        }

        public double MacdValue(int barsAgo)
        {
            if(DataPoints.Count <=1)
                return FirstFastEma - FirstSlowEma;   
            else
                throw new NotImplementedException();
        }

        public double Signal(int barsAgo)
        {
            if (DataPoints.Count <= 1)
                return FirstSignal;
            else
            { throw new NotImplementedException(); }
        }

        public void AddDataPoint(DateTimeOffset time, double value)
        {
            DataPoints.Add(new DataPoint(time, value)); 
        }

       
    }
}
