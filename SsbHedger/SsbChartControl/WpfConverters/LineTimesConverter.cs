using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class LineTimesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resultList = new List<string>();

            var lineTimesDictionary = (Dictionary<DateTime, bool>)value;
            foreach (var dateTimeKey in lineTimesDictionary.Keys)
            {
                if (!lineTimesDictionary[dateTimeKey])
                {
                    resultList.Add("");
                }
                else
                {
                    resultList.Add(dateTimeKey.ToString("HH:mm"));
                }
            }

            return resultList.ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
