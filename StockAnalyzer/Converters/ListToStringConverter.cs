using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace StockAnalyzer.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resultList = value as ObservableCollection<string>;

            if (resultList == null)
            {
                return string.Empty;
            }
            else
            {
                if (!resultList.Any())
                {
                    return string.Empty;
                }
                else
                {
                    return resultList.Select(v=>v.Trim()).Aggregate((r, n) => r + "\r\n" + n);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var listAsString = value as string;
            if (listAsString == null)
            {
                return new ObservableCollection<string>();
            }

            var resultList = listAsString.Split("\r\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            return new ObservableCollection<string>(resultList);
        }
    }
}
