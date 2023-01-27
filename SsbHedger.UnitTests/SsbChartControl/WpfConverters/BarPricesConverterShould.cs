using SsbHedger.SsbChartControl;
using System.Collections;

namespace SsbHedger.UnitTests.SsbChartControl.WpfConverters
{
    public  class BarPricesConverterShould
    {
        [Theory]
        [ClassData(typeof(Height150TestData))]
        public void ConvertCorrectlyHeight150(
            double width,
            int bufferInPercent,
            List<BarUnderlying> bars,
            List<double> expectedLabels)
        {
            
            
            throw new NotImplementedException();
        }
    }

    internal class Height150TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var testBars = new List<BarUnderlying>
            {
                new BarUnderlying(DateTime.Now, 186,190, 184, 188),
                new BarUnderlying(DateTime.Now, 170,175, 165, 166),
                new BarUnderlying(DateTime.Now, 165,168, 160, 162)
            };
            var expectedLabels = new List<double> { 165, 185 };

            yield return new object[]
            {
                150,
                10,
                testBars,
                expectedLabels
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
