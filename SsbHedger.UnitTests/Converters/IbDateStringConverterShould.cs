using SsbHedger.Converters;
using System.Globalization;

namespace SsbHedger.UnitTests.Converters
{
    public class IbDateStringConverterShould
    {
        [Theory]
        [InlineData("20230111 10:15:00", "2023.01.11 10:15:00")]
        [InlineData("20230102 01:04:09", "2023.01.02 01:04:09")]
        public void ConvertCorrectly(string ibDateString, string expectedDateString)
        {
            DateTime expectedDate = DateTime.Parse(expectedDateString, new CultureInfo("DE-de"));

            var sut = new IbDateStringConverter();
            var result = sut.Convert(ibDateString, typeof(DateTime), new object(), CultureInfo.InvariantCulture);
            Assert.Equal(expectedDate, result);
        }
    }
}
