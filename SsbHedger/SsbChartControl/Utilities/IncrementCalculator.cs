using System;

namespace SsbHedger.SsbChartControl.Utilities
{
    public class IncrementCalculator : IIncrementCalculator
    {
        public int Calculate(DateTime sessionStart, DateTime sessionEnd)
        {
            var startMinute = sessionStart.Minute;
            var endMinute = sessionEnd.Minute;

            if(startMinute == 0 || startMinute > 30)
            {
                startMinute = 60;
            }
            if (endMinute == 0 || endMinute > 30)
            {
                endMinute = 60;
            }


            var result = Math.Min(startMinute, endMinute);
            

            while(60 % result != 0)
            {
                result++;   
            }

            return result;
        }
    }
}
