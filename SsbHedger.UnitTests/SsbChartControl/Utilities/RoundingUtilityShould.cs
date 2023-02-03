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

        [Theory]
        [InlineData(184.68, "20", 184.8)]
        [InlineData(186.03, "20", 186.20)]
        [InlineData(187.38, "20", 187.4)]
        [InlineData(188.73, "20", 188.8)]
        public void RoundCorrectly20(
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
        [InlineData(187.34, "10", 187.4)]
        [InlineData(188.01, "10", 188.1)]
        [InlineData(188.69, "10", 188.7)]
        [InlineData(189.36, "10", 189.4)]
        public void RoundCorrectly10(
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
        [InlineData(188.67, "05", 188.7)]
        [InlineData(189.01, "05", 189.05)]
        [InlineData(189.34, "05", 189.35)]
        [InlineData(189.68, "05", 189.7)]
        public void RoundCorrectly05(
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
        [InlineData(189.47, "02", 189.48)]
        [InlineData(189.6,  "02", 189.6)]
        [InlineData(189.74, "02", 189.74)]
        [InlineData(189.87, "02", 189.88)]
        public void RoundCorrectly02(
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
        [InlineData(391.35, "10", 391.4)]
      
        public void RoundCorrectly01(
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

    }
}
