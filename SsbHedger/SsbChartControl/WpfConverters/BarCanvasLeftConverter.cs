using SsbHedger.SsbChartControl.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class BarCanvasLeftConverter : IMultiValueConverter
    {
        IGridLinesUtility _gridLinesUtility;

        public BarCanvasLeftConverter()
        {
            _gridLinesUtility = new GridLinesUtility();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1].GetType() != typeof(Dictionary<DateTime, bool>))
            {
                return new Rect(
                    0,
                    0,
                    WpfConvertersConstants.DEFAULT_GRID_WIDTH,
                    WpfConvertersConstants.DEFAULT_GRID_HEIGHT);
            }

            BarUnderlying bar = (BarUnderlying)values[0];
            
            // Ensure date of today for the test bars
            if (bar.Time.Date != DateTime.Now.Date)
            {
                bar = new BarUnderlying(
                    new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    bar.Time.Hour,
                    bar.Time.Minute,
                    0),
                     bar.Open,
                bar.High,
                bar.Low,
                bar.Close);
            }

            Dictionary<DateTime, bool> lineTimesDictionary = (Dictionary<DateTime, bool>)values[1];
            int barWidth = (int)values[2];
            int yAxisWidth = (int)values[3];
            double controlWidth = (double)values[4];

            double scaledWidth = _gridLinesUtility.GetScaledWidth(
                lineTimesDictionary,
                barWidth,
                yAxisWidth,
                controlWidth);


            DateTime sessionStart = lineTimesDictionary.Keys.ElementAt(0);
            double diffBarSessionStart = (bar.Time - sessionStart).TotalHours;
            
            return diffBarSessionStart * scaledWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
