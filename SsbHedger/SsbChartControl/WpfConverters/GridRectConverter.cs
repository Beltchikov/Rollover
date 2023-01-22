using System;
using System.Collections.Generic;
using System.Globalization;
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
            List<DateTime> lineTimes = (List<DateTime>)values[0];
            DateTime sessionStart = (DateTime)values[1];
            DateTime sessionEnd= (DateTime)values[2];
            int barWidth = (int)values[3];
            double controlWidth = (double)values[4];

            int interval = GetInterval(lineTimes);
            int period = GetPeriod(sessionStart, sessionEnd);
            double ratioIntervalPeriod = (double)interval / (double)period;   

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

        private int GetInterval(List<DateTime> lineTimes)
        {
            List<DateTime> tempList = new List<DateTime>();
            foreach (DateTime dateTime in lineTimes)
            {
                if (!dateTime.Equals(DateTime.MinValue))
                {
                    tempList.Add(dateTime);
                }
            }
            return (int)(tempList[1] - tempList[0]).TotalMinutes;
        }

        private int GetPeriod(
            DateTime sessionStart,
            DateTime sessionEnd)
        {
            return (int)(sessionEnd - sessionStart).TotalMinutes;
        }
    }
}
