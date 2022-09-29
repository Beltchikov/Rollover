using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace OptionHelper
{
    public class BlackScholes : IBlackScholes
    {
        static readonly Normal _normalDistribution = new Normal();

        public double CallPrice(
            double underlying,
            double strike,
            double volatility,
            double interestRate,
            double dividendYield,
            double daysToExpiration)
        {
            var timeToExpiration = daysToExpiration / 365;
            var denominator = volatility * Math.Sqrt(timeToExpiration);

            var d1 = D1(
                underlying,
                strike,
                volatility,
                interestRate,
                dividendYield,
                timeToExpiration,
                denominator);

            var d2 = d1 - denominator;

            var nd1 = _normalDistribution.CumulativeDistribution(d1);
            var nd1Minus = _normalDistribution.CumulativeDistribution(-d1);
            var nd2 = _normalDistribution.CumulativeDistribution(d2);
            var nd2Minus = _normalDistribution.CumulativeDistribution(-d2);

            var kert = strike * Math.Exp(-interestRate * timeToExpiration);
            var seqt = underlying * Math.Exp(-dividendYield * timeToExpiration);

            var result = Math.Round(seqt * nd1 - kert * nd2, 3);
            return result;
        }

        public double PutPrice(
            double underlying,
            double strike,
            double volatility,
            double interestRate,
            double dividendYield,
            double daysToExpiration)
        {
            var timeToExpiration = daysToExpiration / 365;
            var denominator = volatility * Math.Sqrt(timeToExpiration);

            var d1 = D1(
                underlying,
                strike,
                volatility,
                interestRate,
                dividendYield,
                timeToExpiration,
                denominator);

            var d2 = d1 - denominator;

            var nd1 = _normalDistribution.CumulativeDistribution(d1);
            var nd1Minus = _normalDistribution.CumulativeDistribution(-d1);
            var nd2 = _normalDistribution.CumulativeDistribution(d2);
            var nd2Minus = _normalDistribution.CumulativeDistribution(-d2);

            var kert = strike * Math.Exp(-interestRate * timeToExpiration);
            var seqt = underlying * Math.Exp(-dividendYield * timeToExpiration);

            var result = Math.Round(kert * nd2Minus - seqt * nd1Minus, 3);
            return result;
        }

        public double NextStrikeCall(double currentPrice, double strikeStep)
        {
            double result = Math.Round(currentPrice + 0.01, 2);
            while (result % strikeStep > 0)
            {
                result = Math.Round(result + 0.01, 2);
            }

            return result;
        }

        public double NextStrikePut(double currentPrice, double strikeStep)
        {
            double result = Math.Round(currentPrice - 0.01, 2);
            while (result % strikeStep > 0)
            {
                result = Math.Round(result - 0.01, 2);
            }

            return result;
        }

        public double CallDelta(
           double underlying,
           double strike,
           double volatility,
           double interestRate,
           double dividendYield,
           double daysToExpiration)
        {
            var timeToExpiration = daysToExpiration / 365;
            var denominator = volatility * Math.Sqrt(timeToExpiration);

            var d1 = D1(
                underlying,
                strike,
                volatility,
                interestRate,
                dividendYield,
                timeToExpiration,
                denominator);

            var d2 = d1 - denominator;

            var nd1 = _normalDistribution.CumulativeDistribution(d1);
            var ert = Math.Exp(-interestRate * timeToExpiration);

            return nd1 * ert;
        }

        public double PutDelta(
           double underlying,
           double strike,
           double volatility,
           double interestRate,
           double dividendYield,
           double daysToExpiration)
        {
            var timeToExpiration = daysToExpiration / 365;
            var denominator = volatility * Math.Sqrt(timeToExpiration);

            var d1 = D1(
                underlying,
                strike,
                volatility,
                interestRate,
                dividendYield,
                timeToExpiration,
                denominator);

            var d2 = d1 - denominator;

            var nd1 = _normalDistribution.CumulativeDistribution(d1);
            var ert = Math.Exp(-interestRate * timeToExpiration);

            return (nd1 - 1) * ert;
        }

        private double D1(
           double underlying,
           double strike,
           double volatility,
           double interestRate,
           double dividendYield,
           double timeToExpiration,
           double denominator)
        {
            var numeratorTerm1 = Math.Log(underlying / strike);
            var numeratorTerm2 = timeToExpiration *
                (interestRate - dividendYield + Math.Pow(volatility, 2) / 2);

            return (numeratorTerm1 + numeratorTerm2)
                        / denominator;
        }
    }
}
