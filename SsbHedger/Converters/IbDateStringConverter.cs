using System;
using System.Globalization;
using System.Windows.Data;

namespace SsbHedger.Converters
{
    public class IbDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime valueTyped = (DateTime)value;
            return $"{valueTyped.Year}{EnsureTwoDigits(valueTyped.Month)}" +
                $"{EnsureTwoDigits(valueTyped.Day)}" +
                $" {EnsureTwoDigits(valueTyped.Hour)}:" +
                $"{EnsureTwoDigits(valueTyped.Minute)}:" +
                $"{EnsureTwoDigits(valueTyped.Second)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.ParseExact((string)value, "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture);
        }

        private string EnsureTwoDigits(int month)
        {
            string result = month.ToString();
            return result.Length == 2 ? result : "0" + result;
        }
    }
}
