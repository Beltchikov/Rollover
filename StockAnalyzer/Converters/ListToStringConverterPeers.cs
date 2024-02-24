using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace StockAnalyzer.Converters
{
    public class ListToStringConverterPeers : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ObservableCollection<string> resultList)
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
                    return resultList.Aggregate((r, n) => r + "\t" + n);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string listAsString)
            {
                return new ObservableCollection<string>();
            }

            var resultList = listAsString.Split("\t", StringSplitOptions.TrimEntries).ToList();
            return new ObservableCollection<string>(resultList);
        }
    }
}
