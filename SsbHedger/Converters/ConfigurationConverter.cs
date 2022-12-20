﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace SsbHedger.Converters
{
    public class ConfigurationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{values[0]};{values[1]};{values[2]}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
