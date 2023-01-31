namespace SsbHedger.SsbChartControl.Utilities
{
    public interface IRoundingUtility
    {
        double RoundUsingTwoLastDigitsArray(double price, string twoLastDigits);
    }
}