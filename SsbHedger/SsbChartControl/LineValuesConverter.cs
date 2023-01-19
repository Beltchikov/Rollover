using System;
using System.Collections.Generic;
using SsbHedger.SsbChartControl.Utilities;

namespace SsbHedger.SsbChartControl
{
    public class LineValuesConverter : ILineValuesConverter
    {
        private IIncrementCalculator _incrementCalculator;

        public LineValuesConverter()
        {
           _incrementCalculator = new IncrementCalculator();
        }

        public List<DateTime> LineTimes(DateTime sessionStart, DateTime sessionEnd)
        {
            List<DateTime> displayableTimes = GetDisplayableTimes(sessionStart, sessionEnd);

            //int incrementInMinutes = Math.Min(sessionStart.Minute, sessionEnd.Minute);  
            int incrementInMinutes = _incrementCalculator.Calculate(sessionStart, sessionEnd);

            return displayableTimes;
        }

        private static List<DateTime> GetDisplayableTimes(DateTime sessionStart, DateTime sessionEnd)
        {
            var result = new List<DateTime>();

            var running = new DateTime(
                        sessionStart.Year,
                        sessionStart.Month,
                        sessionStart.Day,
                        sessionStart.Hour,
                        0,
                        0);

            while (running < sessionEnd)
            {
                if (running.Hour % 2 == 0)
                {
                    var dateTimeToAdd = new DateTime(
                        sessionStart.Year,
                        sessionStart.Month,
                        sessionStart.Day,
                        running.Hour,
                        0,
                        0);
                    result.Add(dateTimeToAdd);
                }

                running = running.AddHours(1);
            }

            return result;
        }
    }
}
