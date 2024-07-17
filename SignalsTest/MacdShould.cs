using Ta.Indicators;


namespace SignalsTest
{
    public class MacdShould
    {
        [Fact]
        void CalculateFirstMacdCorrectly()
        {
            var macd = new  Macd(12, 26, 9, 235.318, 235.233, 0.125);
        }
    }
}
