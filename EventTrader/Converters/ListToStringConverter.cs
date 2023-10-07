using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Eomn.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resultListYahooEps = value as ObservableCollection<string>;

            if (resultListYahooEps == null)
            {
                return string.Empty;
            }
            else
            {
                if (!resultListYahooEps.Any())
                {
                    return string.Empty;
                }
                else
                {
                    return resultListYahooEps.Aggregate((r, n) => r + "\r\n" + n);
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

            var resultList = listAsString.Split("\r\n", StringSplitOptions.TrimEntries).ToList();
            return new ObservableCollection<string>(resultList);
        }
    }
}
