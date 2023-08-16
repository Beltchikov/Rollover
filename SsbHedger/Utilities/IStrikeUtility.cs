using System.Collections.Generic;

namespace SsbHedger.Utilities
{
    public interface IStrikeUtility
    {
        AtmStrikes[] AtmStrikeCandidates(double underlyingPrice, double strikesStep);
        void SetAtmStrikesInViewModel(IIbHost ibHost, double underlyingPrice);
    }
}
