using System;

namespace SsbHedger.SsbChartControl.Utilities
{
    public class IncrementCalculator : IIncrementCalculator
    {
        public int Calculate(DateTime sessionStart, DateTime sessionEnd)
        {
            var result = Math.Min(sessionStart.Minute, sessionEnd.Minute);
            return result;
        }
    }
}
