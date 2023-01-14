using System;
using System.Globalization;
using System.Windows.Data;

namespace SsbHedger.Converters
{
    public class IbDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.ParseExact("20230111 10:15:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime valueTyped = (DateTime)value;
            return $"{valueTyped.Year}";
        }
    }
}
