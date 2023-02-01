using SsbHedger.SsbChartControl.Utilities;

namespace SsbHedger.UnitTests.SsbChartControl.Utilities
{
    public class RoundingUtilityShould
    {
        [Theory]
        [InlineData(164.88, "00", 165)]
        [InlineData(171.63, "00", 172)]
        [InlineData(178.38, "00", 179)]
        [InlineData(185.13, "00", 186)]
        public void RoundCorrectly00(
            double inputPrice,
            string lastDigitsString,
            double expectedPrice)
        {
            var sut = new RoundingUtility();
            var roundedPrice = sut.RoundUsingTwoLastDigits(
                inputPrice,
                lastDigitsString);

            Assert.Equal(expectedPrice, roundedPrice);
        }

        [Theory]
        [InlineData(177.44, "50", 177.5)]
        [InlineData(180.82, "50", 181)]
        [InlineData(184.2, "50", 184.5)]
        [InlineData(187.58, "50", 188)]
        public void RoundCorrectly50(
            double inputPrice,
            string lastDigitsString,
            double expectedPrice)
        {
            var sut = new RoundingUtility();
            var roundedPrice = sut.RoundUsingTwoLastDigits(
                inputPrice,
                lastDigitsString);

            Assert.Equal(expectedPrice, roundedPrice);
        }

        [Theory]
        [InlineData(183.34, "25", 183.5)]
        [InlineData(185.03, "25", 185.25)]
        [InlineData(186.72, "25", 186.75)]
        [InlineData(188.41, "25", 188.5)]
        public void RoundCorrectly25(
            double inputPrice,
            string lastDigitsString,
            double expectedPrice)
        {
            var sut = new RoundingUtility();
            var roundedPrice = sut.RoundUsingTwoLastDigits(
                inputPrice,
                lastDigitsString);

            Assert.Equal(expectedPrice, roundedPrice);
        }

        // TODO test "20;10;5;2"

        // TODO test "10;5;2"

        // TODO test "5;2"

        // TODO test "2"

    }
}
