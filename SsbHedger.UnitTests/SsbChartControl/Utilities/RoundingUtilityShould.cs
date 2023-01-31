using SsbHedger.SsbChartControl.Utilities;

namespace SsbHedger.UnitTests.SsbChartControl.Utilities
{
    public class RoundingUtilityShould
    {
        [Theory]
        [InlineData(164.88, "50;25;20;10;5;2", 165)]
        //[InlineData(171.63, "50;25;20;10;5;2", 172)]
        //[InlineData(178.38, "50;25;20;10;5;2", 179)]
        //[InlineData(185.13, "50;25;20;10;5;2", 186)]
        public void RoundCorrectly(
            double inputPrice,
            string lastTwoDigitsArrayString,
            double expectedPrice)
        {
            var sut = new RoundingUtility();
            var roundedPrice = sut.RoundUsingTwoLastDigitsArray(
                inputPrice,
                lastTwoDigitsArrayString);

            Assert.Equal(expectedPrice, roundedPrice);
        }

        // TODO test "25;20;10;5;2"

        // TODO test "20;10;5;2"

        // TODO test "10;5;2"

        // TODO test "5;2"

        // TODO test "2"

    }
}
