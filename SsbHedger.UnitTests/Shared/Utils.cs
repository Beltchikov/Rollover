using SsbHedger.SsbChartControl;
using System.Collections.Generic;

namespace SsbHedger.UnitTests.Shared
{
    public static class Utils
    {

        public static Dictionary<DateTime, bool> BuildDateTimeDictionary(
            string lineTimesString,
            string displayFlagString)
        {
            Dictionary<DateTime, bool> lineTimesDictionary = new Dictionary<DateTime, bool>();

            string[] lineTimesStringArray = lineTimesString.Split(";");
            string[] displayFlagStringArray = displayFlagString.Split(";");

            for (int i = 0; i < lineTimesStringArray.Length; i++)
            {
                var time = DateTime.Parse(lineTimesStringArray[i]);
                var displayFlag = Convert.ToBoolean(Convert.ToInt32(displayFlagStringArray[i]));

                lineTimesDictionary[time] = displayFlag;
            }

            return lineTimesDictionary;
        }

        public static List<BarUnderlying> GenerateTestBars(
            
            DateTime startTime,
            int intervalInMinutes,
            double rangeMin,
            double rangeMax,
            int roundDigits,
            int count)
        {
            List <BarUnderlying> resultList = new List<BarUnderlying>();
            
            int i = 0;
            var time = startTime;
            while(i < count)
            {
                time = startTime.AddMinutes(i*intervalInMinutes);
                double open = RandomPrice(rangeMin, rangeMax, roundDigits);
                double high = RandomPrice(open, rangeMax, roundDigits);
                double low = RandomPrice(rangeMin, open, roundDigits);
                double close = RandomPrice(high, low, roundDigits);

                var newBar = new BarUnderlying(time, open, high, low, close);
                resultList.Add(newBar);
                i++;
            }

            return resultList;
        }

        private static double RandomPrice(
            double rangeMin,
            double rangeMax,
            int roundDigits)
        {
            Random random = new Random();
            double unroundedValue = random.NextDouble() * (rangeMax - rangeMin) + rangeMin;
            return Math.Round(unroundedValue, roundDigits);
        }
    }
}