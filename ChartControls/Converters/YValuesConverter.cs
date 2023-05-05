using ChartControls.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ChartControls.Converters
{
    public class YValuesConverter : IMultiValueConverter
    {
        private IMathUtility _mathUtility;

        public YValuesConverter()
        {
            _mathUtility = new MathUtility();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0].GetType() != typeof(double))
            {
                throw new NotImplementedException($"Not implements for the type {values.GetType()}");
            }

            var yValue = (double)values[0];
            var dataRow = ((ObservableCollection<DataPoint>)values[1]).Select(c => c.YValue).Cast<double>().ToList();
            var dataRowsCollection = ((ObservableCollection<ObservableCollection<DataPoint>>)values[2]);
            var height = (double)values[3];

            if (height == 0)
            {
                return 0.0;
            }

            var startOffset = (dataRow.Max() - dataRow.Min()) * 0.1;
            var endOffset = startOffset;
            return _mathUtility.GetDiagramY(
                height,
                dataRow,
                dataRow.IndexOf(yValue),
                startOffset,
                endOffset);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
