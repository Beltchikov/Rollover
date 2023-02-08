using System;
using System.Collections.Generic;
using System.Linq;

namespace SsbHedger.SsbChartControl.Utilities
{
    public class GridLinesUtility : IGridLinesUtility
    {
        public double GetScaledOffsetX(
            Dictionary<DateTime, bool> lineTimesDictionary,
            int barWidth,
            int yAxisWidth,
            double controlWidth)
        {
            double offsetX = GetOffsetX(lineTimesDictionary);
            double period = GetPeriod(lineTimesDictionary);
            double ratioOffsetPeriod = offsetX / period;
            double scaledOffsetX= (controlWidth - 2 * barWidth - yAxisWidth) * ratioOffsetPeriod;

            return scaledOffsetX;
        }

        public double GetScaledWidth(
            Dictionary<DateTime, bool> lineTimesDictionary,
            int barWidth,
            int yAxisWidth,
            double controlWidth)
        {
            double interval = GetInterval(lineTimesDictionary);
            double period = GetPeriod(lineTimesDictionary);
            double ratioIntervalPeriod = interval / period;
            double scaledWidth = (controlWidth - 2 * barWidth - yAxisWidth)
                * ratioIntervalPeriod;

            return scaledWidth;

        }
        private double GetOffsetX(Dictionary<DateTime, bool> lineTimesDictionary)
        {
            int i = 0;
            while (!lineTimesDictionary[lineTimesDictionary.Keys.ElementAt(i)])
            {
                i++;
            }

            var firstDisplayableTime = lineTimesDictionary.Keys.ElementAt(i);
            var firstTime = lineTimesDictionary.Keys.ElementAt(0);
            return (firstDisplayableTime - firstTime).TotalMinutes;
        }

        private double GetInterval(Dictionary<DateTime, bool> lineTimesDictionary)
        {
            var tempList = lineTimesDictionary.Where(d => d.Value).ToList();
            return (tempList[1].Key - tempList[0].Key).TotalMinutes;
        }

        private double GetPeriod(Dictionary<DateTime, bool> lineTimesDictionary)
        {
            var count = lineTimesDictionary.Keys.Count;
            return (lineTimesDictionary.Keys.ElementAt(count - 1)
                - lineTimesDictionary.Keys.ElementAt(0)).TotalMinutes;
        }
    }
}
