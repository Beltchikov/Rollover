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
            int decimalPlaces,
            int count)
        {
            List <BarUnderlying> resultList = new List<BarUnderlying>();
            
            int i = 0;
            var time = startTime;
            while(i < count)
            {
                time = startTime.AddMinutes(i*intervalInMinutes);
                double open = RandomPrice(rangeMin, rangeMax, decimalPlaces);
                double high = RandomPrice(open, rangeMax, decimalPlaces);
                double low = RandomPrice(rangeMin, open, decimalPlaces);
                double close = RandomPrice(high, low, decimalPlaces);

                var newBar = new BarUnderlying(time, open, high, low, close);
                resultList.Add(newBar);
                i++;
            }

            // Ensure max high = rangeMax
            var barWithMaxHigh = resultList.MaxBy(b => b.High);
            if (barWithMaxHigh != null)
            {
                var barMaxHighCopy = CloneBarAndSetHigh(barWithMaxHigh, rangeMax);
                var idx = resultList.FindIndex(b => b.Time == barWithMaxHigh.Time);
                resultList[idx] = barMaxHighCopy;   
            }

            // Ensure min low = rangeMin
            var barWithMinLow = resultList.MinBy(b => b.Low);
            if (barWithMinLow != null)
            {
                var barMinLowCopy = CloneBarAndSetLow(barWithMinLow, rangeMin);
                var idx = resultList.FindIndex(b => b.Time == barWithMinLow.Time);
                resultList[idx] = barMinLowCopy;
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
        private static BarUnderlying CloneBarAndSetHigh(
            BarUnderlying barWithMaxHigh,
            double newHigh)
        {
            return new BarUnderlying(
                barWithMaxHigh.Time,
                barWithMaxHigh.Open,
                newHigh,
                barWithMaxHigh.Low,
                barWithMaxHigh.Close);
        }

        private static BarUnderlying CloneBarAndSetLow(
            BarUnderlying barWithMinLow,
            double newLow)
        {
            return new BarUnderlying(
                barWithMinLow.Time,
                barWithMinLow.Open,
                barWithMinLow.High,
                newLow,
                barWithMinLow.Close);
        }
    }
}