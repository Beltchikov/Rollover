using SsbHedger.SsbChartControl.WpfConverters;
using System.Globalization;

namespace SsbHedger.UnitTests.SsbChartControl.WpfConverters
{
    public class LineTimesConverterShould
    {
        [Theory]
        [InlineData("16:00;18:00;20:00;22:00", "16:00;18:00;20:00;22:00")]
        public void ConvertCorrectly(string timeListString, string expectedTimeListString)
        {
            var timeList = timeListString
                .Split(";")
                .Select(x => DateTime.Parse(x))
                .ToList();
            var expectedTimeList = expectedTimeListString
                .Split(";")
                .ToList();

            var sut = new LineTimesConverter();
            List<string> convertedTimeList = (List<string>)sut.Convert(
                timeList,
                typeof(List<string>),
                new object(),
                CultureInfo.InvariantCulture);

            Assert.Equal(expectedTimeList.Count(), convertedTimeList.Count());
        }
    }
}
