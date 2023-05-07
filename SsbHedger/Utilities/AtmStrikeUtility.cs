using System;

namespace SsbHedger.Utilities
{
    public class AtmStrikeUtility : IAtmStrikeUtility
    {
        public double[] AtmStrikeCandidates(double underlyingPrice, double strikesStep)
        {
            int decimalPlaces = DecimalPlaces(strikesStep);
            var firstAtmCandidate = Math.Round(underlyingPrice / strikesStep, 0) / (decimalPlaces + 1);

            if (firstAtmCandidate > underlyingPrice)
            {
                return new double[]
                {
                    firstAtmCandidate,
                    firstAtmCandidate - strikesStep,
                    firstAtmCandidate - 2 * strikesStep
                };
            }
            else if (firstAtmCandidate < underlyingPrice)
            {
                return new double[]
                {
                    firstAtmCandidate,
                    firstAtmCandidate + strikesStep,
                    firstAtmCandidate + 2 * strikesStep
                };
            }

            return new double[] { firstAtmCandidate };
        }

        private static int DecimalPlaces(double value)
        {
            return value.ToString().Length - ((int)value).ToString().Length == 0
                     ? value.ToString().Length - ((int)value).ToString().Length
                     : value.ToString().Length - ((int)value).ToString().Length - 1;
        }
    }
}
