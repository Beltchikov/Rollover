using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<DateTime> LineTimes(DateTime sessionStart, DateTime sessionEnd, int hoursInterval)
        {
            List<DateTime> displayableTimes = GetDisplayableTimes(sessionStart, sessionEnd, hoursInterval);
            int incrementInMinutes = _incrementCalculator.Calculate(sessionStart, sessionEnd);
            List<DateTime> allTimes = GetAllTimes(sessionStart, sessionEnd, incrementInMinutes);

            List<DateTime> resultTimes = new List<DateTime>();
            foreach (DateTime time in allTimes) 
            { 
                if(displayableTimes.Any(d => d.Hour == time.Hour 
                    && d.Minute == time.Minute))
                {
                    resultTimes.Add(time);
                }
                else
                {
                    resultTimes.Add(DateTime.MinValue);
                }
            }

            return resultTimes;
        }

        private List<DateTime> GetAllTimes(DateTime sessionStart, DateTime sessionEnd, int incrementInMinutes)
        {
            var result = new List<DateTime>();

            var running = new DateTime(
                        sessionStart.Year,
                        sessionStart.Month,
                        sessionStart.Day,
                        sessionStart.Hour,
                        sessionStart.Minute,
                        0);

            while (running <= sessionEnd)
            {
                var dateTimeToAdd = new DateTime(
                    sessionStart.Year,
                    sessionStart.Month,
                    sessionStart.Day,
                    running.Hour,
                    running.Minute,
                    0);
                result.Add(dateTimeToAdd);
               
                running = running.AddMinutes(incrementInMinutes);
            }

            return result;
        }

        private static List<DateTime> GetDisplayableTimes(
            DateTime sessionStart,
            DateTime sessionEnd,
            int hoursInterval)
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
                if (running.Hour % hoursInterval == 0)
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
