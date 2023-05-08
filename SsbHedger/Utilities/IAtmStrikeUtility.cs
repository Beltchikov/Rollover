namespace SsbHedger.Utilities
{
    public interface IAtmStrikeUtility
    {
        AtmStrikes[] AtmStrikeCandidates(double underlyingPrice, double strikesStep);
        void SetAtmStrikesInViewModel(IIbHost ibHost, double underlyingPrice);
    }
}
