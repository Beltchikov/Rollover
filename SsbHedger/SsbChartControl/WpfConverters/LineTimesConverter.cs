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
            var resultList = new List<XAxisValue>();

            var lineTimesDictionary = (Dictionary<DateTime, bool>)value;
            int position = 0;
            foreach (var dateTimeKey in lineTimesDictionary.Keys)
            {
                if (!lineTimesDictionary[dateTimeKey])
                {
                    resultList.Add(new XAxisValue(DateTime.MinValue, position));
                }
                else
                {
                    resultList.Add(new XAxisValue(dateTimeKey, position));
                }
                position++;
            }

            return resultList.ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class XAxisValue
    {
        public XAxisValue(DateTime dateTime, int position)
        {
            Time = dateTime;
            Position = position;
        }

        public DateTime Time { get; set; }
        public string TimeAsString 
        { 
            get 
            { 
                return Time == DateTime.MinValue 
                    ? "" 
                    : Time.ToString("HH:mm"); 
            } 
        }
        public int Position { get; set; }
    }
}
