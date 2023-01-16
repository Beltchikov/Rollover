using SsbHedger.Converters;
using System.Globalization;

namespace SsbHedger.UnitTests.Converters
{
    public class TimeToDateTimeConverterShould
    {
        [Theory]
        [InlineData("15:30", "15:30")]
        [InlineData("22:15", "22:15")]
        public void ConvertCorrectly(string sessionTimeString, string expectedDateString)
        {
            DateTime expectedDate = DateTime.Parse(expectedDateString, new CultureInfo("DE-de"));

            var sut = new TimeToDateTimeConverter();
            var result = sut.Convert(sessionTimeString, typeof(DateTime), new object(), CultureInfo.InvariantCulture);
            Assert.Equal(expectedDate, result);
        }
    }
}
