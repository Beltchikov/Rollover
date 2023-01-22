using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class GridRectConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            Dictionary<DateTime, bool> lineTimesDictionary = (Dictionary<DateTime, bool>)values[0];
            int barWidth = (int)values[1];
            double controlWidth = (double)values[2];

            double interval = GetInterval(lineTimesDictionary);
            double period = GetPeriod(lineTimesDictionary);
            double ratioIntervalPeriod = interval / period;

            double scaledWidth = (controlWidth - 2 * barWidth) * ratioIntervalPeriod;

            return new Rect(0, 0, scaledWidth, 20);
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private double GetInterval(Dictionary<DateTime, bool> lineTimesDictionary)
        {
            var tempList = lineTimesDictionary.Where(d => d.Value).ToList();
            return (tempList[1].Key - tempList[0].Key).TotalMinutes;
        }

        private double GetPeriod(Dictionary<DateTime, bool> lineTimesDictionary)
        {
            var count = lineTimesDictionary.Keys.Count;
            return (lineTimesDictionary.Keys.ElementAt(count -1) 
                - lineTimesDictionary.Keys.ElementAt(0)).TotalMinutes;
        }
    }
}
