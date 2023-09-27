using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Eomn.Converters
{
    public class YahooEpsConverter : IValueConverter
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
            throw new NotImplementedException();
        }
    }
}
