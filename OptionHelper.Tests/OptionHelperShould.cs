using NSubstitute;

namespace OptionHelper.Tests
{
    public class OptionHelperShould
    {
        [Theory]
        [InlineData(36.07, 0.01, 1, 2, 37)]
        [InlineData(36.07, 0.01, 0.5, 2, 36.5)]
        [InlineData(6170308.29, 0.01, 1, 2, 6170309)]
        public void ReturnCorrectNextHigherStrike(
            double underlying,
            double stepUnderlying,
            double stepStrike,
            int precision,
            double expectedResult)
        {
            var sut = new OptionHelper(null);
            var result = sut.NextHigherStrike(underlying, stepUnderlying, stepStrike, precision);
            Assert.Equal(expectedResult, result);

        }

        [Theory]
        [InlineData(36.07, 0.01, 1, 2, 36)]
        [InlineData(35.97, 0.01, 0.5, 2, 35.5)]
        [InlineData(6170308.29, 0.01, 1, 2, 6170308)]
        public void ReturnCorrectNextLowerStrike(
            double underlying,
            double stepUnderlying,
            double stepStrike,
            int precision,
            double expectedResult)
        {
            var sut = new OptionHelper(null);
            var result = sut.NextLowerStrike(underlying, stepUnderlying, stepStrike, precision);
            Assert.Equal(expectedResult, result);

        }

        [Theory]
        [InlineData(35.54, 0.01, 1, 0.16, 2, 41)]
        [InlineData(35.54, 0.01, 1, 0.5, 2, 36)]
        [InlineData(36, 0.01, 1, 0.5, 2, 36)]
        [InlineData(35.54, 0.01, 1, 0.55, 2, 35)]
        public void ReturnCorrectFirstCallStrikeByDelta(
            double underlying,
            double stepUnderlying,
            double stepStrike,
            double targetDelta,
            int precision,
            double expectedStrike)
        {
            double volatility = 0.4825;
            double interestRate = 0.01;
            double dividendYield = 0;
            double daysToExpiration = 26;

            IBlackScholes blackScholes = Substitute.For<IBlackScholes>();
            var strikeAndCallDeltaDict = new Dictionary<double, double>
            {
                {35,0.5749},
                {36,0.4881},
                {37,0.4041},
                {38,0.3264},
                {39,0.2574},

                {40,0.1982},
                {41,0.1492},
                {42,0.1099},
                {43,0.0793},
            };
            foreach (var strikeAndCallDelta in strikeAndCallDeltaDict)
            {
                blackScholes.CallDelta(underlying,
                                   strikeAndCallDelta.Key,
                                   volatility,
                                   interestRate,
                                   dividendYield,
                                   daysToExpiration)
                .Returns(strikeAndCallDelta.Value);
            }

            var sut = new OptionHelper(blackScholes);
            var strike = sut.FirstCallStrikeByDelta(
                targetDelta,
                underlying,
                stepUnderlying,
                stepStrike,
                volatility,
                interestRate,
                dividendYield,
                daysToExpiration,
                precision);

            Assert.Equal(expectedStrike, strike);
        }

        [Theory]
        [InlineData(41.5, 0.01, 1, -0.16, 2, 36)]
        [InlineData(41.5, 0.01, 1, -0.5, 2, 41)]
        [InlineData(42, 0.01, 1, -0.5, 2, 41)]
        [InlineData(41.5, 0.01, 1, -0.55, 2, 43)]
        public void ReturnCorrectFirstPutStrikeByDelta(
           double underlying,
           double stepUnderlying,
           double stepStrike,
           double targetDelta,
           int precision,
           double expectedStrike)
        {
            double volatility = 0.4825;
            double interestRate = 0.01;
            double dividendYield = 0;
            double daysToExpiration = 26;

            IBlackScholes blackScholes = Substitute.For<IBlackScholes>();
            var strikeAndPutDeltaDict = new Dictionary<double, double>
            {
                {35,-0.0819},
                {36,-0.1202},
                {37,-0.1682},
                {38,-0.2254},
                {39,-0.2903},

                {40,-0.3610},
                {41,-0.4348},
                {42,-0.5092},
                {43,-0.5815},
            };
            foreach (var strikeAndCallDelta in strikeAndPutDeltaDict)
            {
                blackScholes.PutDelta(underlying,
                                   strikeAndCallDelta.Key,
                                   volatility,
                                   interestRate,
                                   dividendYield,
                                   daysToExpiration)
                .Returns(strikeAndCallDelta.Value);
            }

            var sut = new OptionHelper(blackScholes);
            var strike = sut.FirstPutStrikeByDelta(
                targetDelta,
                underlying,
                stepUnderlying,
                stepStrike,
                volatility,
                interestRate,
                dividendYield,
                daysToExpiration,
                precision);

            Assert.Equal(expectedStrike, strike);
        }
    }
}
