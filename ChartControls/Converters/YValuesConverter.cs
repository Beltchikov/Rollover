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
            var dataPointCollection = ((ObservableCollection<DataPoint>)values[1]).Select(c => c.YValue).Cast<double>().ToList();
            var height = (double)values[2];

            if (height == 0)
            {
                return 0.0;
            }

            var startOffset = (dataPointCollection.Max() - dataPointCollection.Min()) * 0.1;
            var endOffset = startOffset;
            return _mathUtility.GetDiagramCoordinate(
                height,
                dataPointCollection,
                dataPointCollection.IndexOf(yValue),
                startOffset,
                endOffset);

            
            throw new NotImplementedException();

            //if (values[0].GetType() != typeof(DateTime)) 
            //{
            //    throw new NotImplementedException($"Not implements for the type {values.GetType()}");
            //}

            //var time = (DateTime)values[0]; 
            //var timeCollection= ((ObservableCollection<DataPoint>)values[1]).Select(c => c.XValue).Cast<DateTime>().ToList();
            //var width = (double)values[2];

            //if(width == 0)
            //{
            //    return 0.0;
            //}

            //var valuesForDiagram = timeCollection.Select(t => (t - new DateTime(1970, 1, 1)).TotalMilliseconds)
            //                                                              .Cast<double>()
            //                                                              .ToList();
            //var endOffset = (valuesForDiagram.Max() - valuesForDiagram.Min()) * 0.2;
            //return _mathUtility.GetDiagramCoordinate(width, valuesForDiagram, timeCollection.IndexOf(time), 0, endOffset);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
