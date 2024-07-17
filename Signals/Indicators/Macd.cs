namespace Ta.Indicators
{
    public class Macd
    {
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
            FastEmaPeriod = fastEmaPeriod;
            SlowEmaPeriod = slowEmaPeriod;
            SignalPeriod = signalPeriod;

            FirstFastEma = firstFastEma;
            FirstSlowEma = firstSlowEma;
            FirstSignal = firstSignal;
        }
    }
}
