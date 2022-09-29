using NSubstitute;

namespace OptionHelper.Tests
{
    public class BlackScholesShould
    {
        [Fact]
        public void ReturnDoubleForCall()
        {
            var sut = new BlackScholes();
            var result = sut.CallPrice(
                Arg.Any<double>(),
                0,
                Arg.Any<double>(),
                Arg.Any<double>(),
                Arg.Any<double>(),
                Arg.Any<double>());

            Assert.IsType<double>(result);
        }


        [Theory]
        [InlineData(36.07, 35, 0.4825, 0.01, 0, 26, 2.423)]
        [InlineData(394.4, 394, 0.28, 0.01, 0, 3, 4.212)]
        public void ReturnCorrectCallPrice(
            double underlying,
            double strike,
            double volatility,
            double interestRate,
            double dividendYield,
            double daysToExpiration,
            double expectedResult)
        {
            var sut = new BlackScholes();
            var result = sut.CallPrice(
                underlying,
                strike,
                volatility,
                interestRate,
                dividendYield,
                daysToExpiration);

            Assert.Equal(expectedResult, result);

        }

        [Theory]
        [InlineData(36.07, 35, 0.4825, 0.01, 0, 26, 1.328)]
        [InlineData(394.4, 394, 0.28, 0.01, 0, 3, 3.779)]
        public void ReturnCorrectPutPrice(
            double underlying,
            double strike,
            double volatility,
            double interestRate,
            double dividendYield,
            double daysToExpiration,
            double expectedResult)
        {
            var sut = new BlackScholes();
            var result = sut.PutPrice(
                underlying,
                strike,
                volatility,
                interestRate,
                dividendYield,
                daysToExpiration);

            Assert.Equal(expectedResult, result);

        }

        [Theory]
        [InlineData(392.33, 1, 393)]
        [InlineData(11, 5, 15)]
        [InlineData(25.5, 0.5, 26)]
        public void ReturnCorrectNextStrikeCall(
            double currentPrice,
            double strikeStep,
            double expectedResult)
        {
            var sut = new BlackScholes();
            var result = sut.NextStrikeCall(
                currentPrice,
                strikeStep);

            Assert.Equal(expectedResult, result);

        }

        [Theory]
        [InlineData(392.33, 1, 392)]
        [InlineData(11, 5, 10)]
        [InlineData(25.5, 0.5, 25)]
        public void ReturnCorrectNextStrikePut(
           double currentPrice,
           double strikeStep,
           double expectedResult)
        {
            var sut = new BlackScholes();
            var result = sut.NextStrikePut(
                currentPrice,
                strikeStep);

            Assert.Equal(expectedResult, result);

        }

        [Theory]
        [InlineData(36.07, 35, 0.4825, 0.01, 0, 26, 0.62)]
        [InlineData(394.4, 394, 0.28, 0.01, 0, 3, 0.52)]
        public void ReturnCorrectCallDelta(
           double underlying,
           double strike,
           double volatility,
           double interestRate,
           double dividendYield,
           double daysToExpiration,
           double expectedResult)
        {
            var sut = new BlackScholes();
            var result = sut.CallDelta(
                underlying,
                strike,
                volatility,
                interestRate,
                dividendYield,
                daysToExpiration);

            Assert.Equal(expectedResult, Math.Round(result, 2));

        }

        [Theory]
        [InlineData(36.07, 35, 0.4825, 0.01, 0, 26, -0.38)]
        [InlineData(394.4, 394, 0.28, 0.01, 0, 3, -0.48)]
        public void ReturnCorrectPutDelta(
           double underlying,
           double strike,
           double volatility,
           double interestRate,
           double dividendYield,
           double daysToExpiration,
           double expectedResult)
        {
            var sut = new BlackScholes();
            var result = sut.PutDelta(
                underlying,
                strike,
                volatility,
                interestRate,
                dividendYield,
                daysToExpiration);

            Assert.Equal(expectedResult, Math.Round(result, 2));

        }
    }
}
