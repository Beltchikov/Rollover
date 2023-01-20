using SsbHedger.SsbChartControl.Utilities;

namespace SsbHedger.UnitTests.SsbChartControl.Utilities
{
    public class IncrementCalculatorShould
    {
        [Theory]
        [InlineData("15:30", "22:15", 15)]
        public void CalculateCorrectly(
            string sessionStartString,
            string sessionEndString,
            int expectedIncrement)
        {
            DateTime sessionStart = DateTime.Parse(sessionStartString); 
            DateTime sessionEnd= DateTime.Parse(sessionEndString); 

            var sut = new IncrementCalculator();
            int increment = sut.Calculate(sessionStart, sessionEnd);

            Assert.Equal(expectedIncrement, increment);
        }
    }
}
