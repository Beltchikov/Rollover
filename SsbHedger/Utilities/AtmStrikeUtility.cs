using System;

namespace SsbHedger.Utilities
{
    public class AtmStrikeUtility : IAtmStrikeUtility
    {
        public AtmStrikes[] AtmStrikeCandidates(double underlyingPrice, double strikesStep)
        {
            int decimalPlaces = DecimalPlaces(strikesStep);
            var firstAtmCandidate = Math.Round(underlyingPrice / strikesStep, 0) / (decimalPlaces + 1);

            if (firstAtmCandidate > underlyingPrice)
            {
                return new AtmStrikes[]
                {
                    new AtmStrikes(firstAtmCandidate, firstAtmCandidate),

                    new AtmStrikes(firstAtmCandidate - strikesStep, firstAtmCandidate + strikesStep),
                    new AtmStrikes(firstAtmCandidate - strikesStep, firstAtmCandidate + 2*strikesStep),

                    new AtmStrikes(firstAtmCandidate - 2*strikesStep, firstAtmCandidate + strikesStep),
                    new AtmStrikes(firstAtmCandidate - 2*strikesStep, firstAtmCandidate + 2*strikesStep)
                };
            }
            else if (firstAtmCandidate < underlyingPrice)
            {
                return new AtmStrikes[]
               {
                    new AtmStrikes(firstAtmCandidate, firstAtmCandidate),

                    new AtmStrikes(firstAtmCandidate + strikesStep, firstAtmCandidate - strikesStep),
                    new AtmStrikes(firstAtmCandidate + strikesStep, firstAtmCandidate - 2*strikesStep),

                    new AtmStrikes(firstAtmCandidate + 2*strikesStep, firstAtmCandidate - strikesStep),
                    new AtmStrikes(firstAtmCandidate + 2*strikesStep, firstAtmCandidate - 2*strikesStep)
               };
            }

            return new AtmStrikes[] {new AtmStrikes(firstAtmCandidate, firstAtmCandidate)};
        }

        private static int DecimalPlaces(double value)
        {
            return value.ToString().Length - ((int)value).ToString().Length == 0
                     ? value.ToString().Length - ((int)value).ToString().Length
                     : value.ToString().Length - ((int)value).ToString().Length - 1;
        }
    }

    public record AtmStrikes(double NextAtmStrike, double SecondAtmStrike);
}
