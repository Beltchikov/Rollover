namespace SsbHedger.Utilities
{
    public interface IAtmStrikeUtility
    {
        double[] AtmStrikeCandidates(double underlyingPrice, double strikesStep);
    }
}
