using SsbHedger.SsbChartControl.WpfConverters;
using SsbHedger.UnitTests.Shared;
using System.Globalization;
using System.Windows;

namespace SsbHedger.UnitTests.SsbChartControl.WpfConverters
{
    public class GridRectConverterShould
    {
        [Theory]
        [InlineData("15:30;15:45;" +
            "16:00;16:15;16:30;16:45;17:00;17:15;17:30;17:45;" +
            "18:00;18:15;18:30;18:45;19:00;19:15;19:30;19:45;" +
            "20:00;20:15;20:30;20:45;21:00;21:15;21:30;21:45;" +
            "22:00;22:15",
            "0;0;" +
            "1;0;0;0;0;0;0;0;" +
            "1;0;0;0;0;0;0;0;" +
            "1;0;0;0;0;0;0;0;" +
            "1;0",
            10,
            580,
            160)]
        void ConvertCorrectly(
            string lineTimesString,
            string displayFlagString,
            int barWidth,
            double controlWidth,
            int expectedScaledWidth)
        {
            Dictionary<DateTime, bool> lineTimesDictionary = Utils.BuildDateTimeDictionary(
                lineTimesString,
                displayFlagString);
            
            var sut = new GridRectConverter();
            object[] values =
            {
                lineTimesDictionary,
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
    }
}
