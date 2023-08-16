using AutoFixture;
using SsbHedger.Utilities;
using System.Globalization;

namespace SsbHedger.UnitTests.Utilities
{
    public class LastTradeDateConverterShould
    {
        [Theory]
        [InlineData("02.02.2023", "230202")]
        [InlineData("02.11.2023", "231102")]
        [InlineData("12.10.1999", "991012")]
        public void ConvertInProperFormat(string dateTimeString, string expected)
        {
            var dateTime = DateTime.Parse(dateTimeString, new CultureInfo("DE-de"));
            
            var sut = (new Fixture()).Create<LastTradeDateConverter>();
            var lastTradeDateString = sut.FromDateTime(dateTime);

            Assert.IsType<string>(lastTradeDateString);
            Assert.Equal(expected, lastTradeDateString);
        }
    }
}
