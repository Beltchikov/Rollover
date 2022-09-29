using System;

namespace OptionHelper
{
    public class OptionHelper : IOptionHelper
    {
        private readonly IBlackScholes _blackScholes;

        public OptionHelper(IBlackScholes blackScholes)
        {
            _blackScholes = blackScholes;
        }

        public double NextHigherStrike(
            double underlying,
            double stepUnderlying,
            double stepStrike,
            int precision)
        {
            return NextStrike(
                underlying,
                stepUnderlying,
                stepStrike,
                precision,
                (strike, step) => strike + step);
        }

        public double NextLowerStrike(
            double underlying,
            double stepUnderlying,
            double stepStrike,
            int precision)
        {
            return NextStrike(
                underlying,
                stepUnderlying,
                stepStrike,
                precision,
                (strike, step) => strike - step);
        }

        public double FirstCallStrikeByDelta(
            double targetDelta,
            double underlying,
            double stepUnderlying,
            double stepStike,
            double volatility,
            double interestRate,
            double dividendYield,
            double daysToExpiration,
            int precision)
        {
            double nextStrike = underlying;
            double currentDelta = 1;
            int loopCount = 0;

            if (Math.Abs(targetDelta) <= 0.5)
            {
                while (Math.Abs(currentDelta) >= Math.Abs(targetDelta))
                {
                    nextStrike = NextHigherStrike(
                        loopCount == 0 ? nextStrike : nextStrike + stepUnderlying,
                        stepUnderlying,
                        stepStike,
                        precision);
                    currentDelta = _blackScholes.CallDelta(underlying,
                                                               nextStrike,
                                                               volatility,
                                                               interestRate,
                                                               dividendYield,
                                                               daysToExpiration);
                    loopCount++;

                }
                return nextStrike;
            }

            currentDelta = 0;
            while (Math.Abs(currentDelta) <= Math.Abs(targetDelta))
            {
                nextStrike = NextLowerStrike(
                    loopCount == 0 ? nextStrike : nextStrike - stepUnderlying,
                    stepUnderlying,
                    stepStike,
                    precision);
                currentDelta = _blackScholes.CallDelta(underlying,
                                                           nextStrike,
                                                           volatility,
                                                           interestRate,
                                                           dividendYield,
                                                           daysToExpiration);
                loopCount++;
            }
            return nextStrike;

        }

        public double FirstPutStrikeByDelta(
            double targetDelta,
            double underlying,
            double stepUnderlying,
            double stepStike,
            double volatility,
            double interestRate,
            double dividendYield,
            double daysToExpiration,
            int precision)
        {
            double nextStrike = underlying;
            double currentDelta = 1;
            int loopCount = 0;

            if (Math.Abs(targetDelta) <= 0.5)
            {
                while (Math.Abs(currentDelta) >= Math.Abs(targetDelta))
                {
                    nextStrike = NextLowerStrike(
                        loopCount == 0 ? nextStrike : nextStrike - stepUnderlying,
                        stepUnderlying,
                        stepStike,
                        precision);
                    currentDelta = _blackScholes.PutDelta(underlying,
                                                               nextStrike,
                                                               volatility,
                                                               interestRate,
                                                               dividendYield,
                                                               daysToExpiration);

                    loopCount++;
                }
                return nextStrike;
            }

            currentDelta = 0;
            while (Math.Abs(currentDelta) <= Math.Abs(targetDelta))
            {
                nextStrike = NextHigherStrike(
                    nextStrike == underlying ? nextStrike : nextStrike + stepUnderlying,
                    stepUnderlying,
                    stepStike,
                    precision);
                currentDelta = _blackScholes.PutDelta(underlying,
                                                           nextStrike,
                                                           volatility,
                                                           interestRate,
                                                           dividendYield,
                                                           daysToExpiration);
                loopCount++;

            }
            return nextStrike;
        }

        private double NextStrike(
            double underlying,
            double stepUnderlying,
            double stepStrike,
            int precision,
            Func<double, double, double> strikeSearcFunc)
        {
            var strikeCandidate = underlying;
            while (strikeCandidate % stepStrike != 0)
            {
                strikeCandidate = strikeSearcFunc(strikeCandidate, stepUnderlying);
                strikeCandidate = Math.Round(strikeCandidate, precision);
            }
            return strikeCandidate;
        }
    }
}
