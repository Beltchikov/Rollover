namespace SsbHedger.SsbChartControl.Utilities
{
    public interface IRoundingUtility
    {
        double RoundUsingTwoLastDigits(double price, string twoLastDigits);
    }
}