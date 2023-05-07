namespace SsbHedger.Utilities
{
    public interface IAtmStrikeUtility
    {
        AtmStrikes[] AtmStrikeCandidates(double underlyingPrice, double strikesStep);
    }
}
