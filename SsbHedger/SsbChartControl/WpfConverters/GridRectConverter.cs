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
        int DEFAULT_WIDTH = 20;
        int DEFAULT_HEIGHT = 20;
        
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if(values[0].GetType() != typeof(Dictionary<DateTime, bool>))
            {
                return new Rect(0, 0, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            }
            
            Dictionary<DateTime, bool> lineTimesDictionary = (Dictionary<DateTime, bool>)values[0];
            int barWidth = (int)values[1];
            int yAxisWidth = (int)values[2];
            double controlWidth = (double)values[3];

            double interval = GetInterval(lineTimesDictionary);
            double period = GetPeriod(lineTimesDictionary);
            double ratioIntervalPeriod = interval / period;

            double scaledWidth = (controlWidth - 2 * barWidth - yAxisWidth) * ratioIntervalPeriod;

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
