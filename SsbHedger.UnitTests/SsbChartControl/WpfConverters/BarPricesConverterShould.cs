using SsbHedger.SsbChartControl;
using SsbHedger.SsbChartControl.WpfConverters;
using SsbHedger.UnitTests.Shared;
using System.Globalization;

namespace SsbHedger.UnitTests.SsbChartControl.WpfConverters
{
    public  class BarPricesConverterShould
    {
        [Theory]
        [InlineData(160, 190, 150)]
        public void ReturnListOfPriceAndMargin(
            double rangeMin,
            double rangeMax,
            double axisHeight)
        {
            var decimalPlaces = 2;
            List<BarUnderlying> bars = Utils.GenerateTestBars(
                DateTime.Now,
                5,
                rangeMin,
                rangeMax,
                decimalPlaces,
                10);
            object[] values = new object[]
            {
                bars,
                axisHeight
            };
                        
            var sut = new BarPricesConverter();
            var result = sut.Convert(
                values,
                typeof(List<PriceAndMargin>),
                new object(),
                CultureInfo.InvariantCulture);

            // Verify type and count
            Assert.IsType<List<PriceAndMargin>>(result);
            var resultTyped = (List<PriceAndMargin>)result; 
            Assert.Equal(
                ExpectedNumberOfLabels(axisHeight),
                resultTyped.Count());

            // Verify same step for prices
            var prices = resultTyped
                .Select(r => Convert.ToDouble(r.PriceAsString))
                .ToList();
            var priceSteps = prices.Zip(prices
                .Skip(1), (x,y) => Math.Round(x-y, decimalPlaces))
                .ToList();
            Assert.Equal(priceSteps.Max(), priceSteps.Min());

            // Verify same step for margin tops
            var marginTopValues = resultTyped
                .Select(r => r.Margin.Top)
                .ToList();
            var marginTopSteps = marginTopValues.Zip(marginTopValues
                .Skip(1), (x, y) => y - x)
                .ToList();
            Assert.Equal(marginTopSteps.Max(), marginTopSteps.Min());
        }

        private int ExpectedNumberOfLabels(double axisHeight)
        {
            double axisHeightNet = axisHeight *
                (1 - 2 * WpfConvertersConstants.CHART_BUFFER_UP_DOWN_IN_PERCENT / 100);
            int numberOfLabels = (int)Math.Round(
               axisHeightNet / WpfConvertersConstants.MIN_HEIGHT_FOR_PRICE_LABEL,
               0);
            return numberOfLabels;  
        }
    }
}
