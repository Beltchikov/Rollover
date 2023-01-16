using System;
using System.Globalization;
using System.Windows.Data;

namespace SsbHedger.Converters
{
    public class IbDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DateTime.MinValue;
            }

            return DateTime.ParseExact((string)value, "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            DateTime valueTyped = (DateTime)value;
            return $"{valueTyped.Year}{EnsureTwoDigits(valueTyped.Month)}" +
                $"{EnsureTwoDigits(valueTyped.Day)}" +
                $" {EnsureTwoDigits(valueTyped.Hour)}:" +
                $"{EnsureTwoDigits(valueTyped.Minute)}:" +
                $"{EnsureTwoDigits(valueTyped.Second)}";
        }

        private string EnsureTwoDigits(int month)
        {
            string result = month.ToString();
            return result.Length == 2 ? result : "0" + result;
        }
    }
}
