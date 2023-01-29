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
            List<BarUnderlying> bars = Utils.GenerateTestBars(
                DateTime.Now,
                5,
                rangeMin,
                rangeMax,
                2,
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

            Assert.IsType<List<PriceAndMargin>>(result);
            var resultTyped = (List<PriceAndMargin>)result; 
            Assert.Equal(
                ExpectedNumberOfLabels(axisHeight),
                resultTyped.Count());
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
