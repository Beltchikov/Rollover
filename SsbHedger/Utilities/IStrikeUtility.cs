using System.Collections.Generic;

namespace SsbHedger.Utilities
{
    public interface IStrikeUtility
    {
        AtmStrikes[] AtmStrikeCandidates(double underlyingPrice, double strikesStep);
        List<double> ReplaceInvalidStrike(
            List<double> strikeList,
            double strike,
            double underlyingPrice,
            double strikeStep);
        void SetAtmStrikesInViewModel(IIbHost ibHost, double underlyingPrice);
    }
}
