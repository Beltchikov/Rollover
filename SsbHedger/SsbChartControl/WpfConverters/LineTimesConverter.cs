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

            foreach (var dateTimeKeyValue in (Dictionary<DateTime, bool>)value) 
            { 
                if(!dateTimeKeyValue.Value)
                {
                    resultList.Add("");
                }
                else
                {
                    resultList.Add(dateTimeKeyValue.Key.ToString("HH:mm"));
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
