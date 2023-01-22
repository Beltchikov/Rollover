using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class GridRectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<DateTime> lineTimes = (List<DateTime>)values[0];
            int barWidth = (int)values[1];
            int controlWidth = (int)values[2];

            int interval = GetInterval(lineTimes);
            int period = GetPeriod(lineTimes);
            double ratioIntervalPeriod = (double)interval / (double)period;   

            double scaledWidth = (controlWidth - 2 * barWidth) * ratioIntervalPeriod;

            return new Rect(0, 0, scaledWidth, 20);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
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

        private int GetPeriod(List<DateTime> lineTimes)
        {
            List<DateTime> tempList = new List<DateTime>();
            foreach (DateTime dateTime in lineTimes)
            {
                if (!dateTime.Equals(DateTime.MinValue))
                {
                    tempList.Add(dateTime);
                }
            }
            return (int)(tempList.Last() - tempList.First()).TotalMinutes;
        }
    }
}
