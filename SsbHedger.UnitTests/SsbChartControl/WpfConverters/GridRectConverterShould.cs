using SsbHedger.SsbChartControl.WpfConverters;
using System.Globalization;
using System.Windows;

namespace SsbHedger.UnitTests.SsbChartControl.WpfConverters
{
    public class GridRectConverterShould
    {
        [Theory]
        [InlineData(";;16:00;;;;;;;;18:00;;;;;;;;20:00;;;;;;;;22:00;",
            10,
            580,
            160)]
        void ConvertCorrectly(
            string lineTimesString,
            int barWidth,
            double controlWidth,
            int expectedScaledWidth)
        {
            List<DateTime> lineTimes = BuildDateTimeArrray(lineTimesString);
            
            var sut = new GridRectConverter();
            object[] values =
            {
                lineTimes,
                barWidth,
                controlWidth
            };
            Rect rect = (Rect)sut.Convert(
                values,
                typeof(Rect),
                new object(),
                CultureInfo.InvariantCulture);

            Assert.Equal(expectedScaledWidth, rect.Width);
        }

        private static List<DateTime> BuildDateTimeArrray(string lineTimesString)
        {
            List<DateTime> lineTimes = new List<DateTime>();
            string[] lineTimesStringArray = lineTimesString.Split(";");
            foreach (var timeString in lineTimesStringArray)
            {
                if (string.IsNullOrWhiteSpace(timeString))
                {
                    lineTimes.Add(DateTime.MinValue);
                }
                else
                {
                    lineTimes.Add(DateTime.Parse(timeString));
                }
            }

            return lineTimes;
        }
    }
}
