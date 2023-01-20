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

        [Theory]
        [InlineData("15:30", "22:14", 15)]
        [InlineData("15:29", "22:00", 30)]
        public void CalculateCorrectlyIfSixtyIsNotMultipleOfIncrement(
            string sessionStartString,
            string sessionEndString,
            int expectedIncrement)
        {
            DateTime sessionStart = DateTime.Parse(sessionStartString);
            DateTime sessionEnd = DateTime.Parse(sessionEndString);

            var sut = new IncrementCalculator();
            int increment = sut.Calculate(sessionStart, sessionEnd);

            Assert.Equal(expectedIncrement, increment);
        }

        [Theory]
        [InlineData("15:00", "22:00", 60)]
        public void CalculateCorrectlyIfBothMinutesZero(
           string sessionStartString,
           string sessionEndString,
           int expectedIncrement)
        {
            DateTime sessionStart = DateTime.Parse(sessionStartString);
            DateTime sessionEnd = DateTime.Parse(sessionEndString);

            var sut = new IncrementCalculator();
            int increment = sut.Calculate(sessionStart, sessionEnd);

            Assert.Equal(expectedIncrement, increment);
        }

        [Theory]
        [InlineData("15:45", "22:55", 60)]
        public void CalculateCorrectlyIfMinutesAbove30(
           string sessionStartString,
           string sessionEndString,
           int expectedIncrement)
        {
            DateTime sessionStart = DateTime.Parse(sessionStartString);
            DateTime sessionEnd = DateTime.Parse(sessionEndString);

            var sut = new IncrementCalculator();
            int increment = sut.Calculate(sessionStart, sessionEnd);

            Assert.Equal(expectedIncrement, increment);
        }
    }
}
