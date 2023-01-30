using SsbHedger.SsbChartControl.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.UnitTests.SsbChartControl.Utilities
{
    public class PriceLabelsUtilitiesShould
    {
        [Theory]
        [InlineData(4, 160, 190, "165;172;179;186")]
        public void GetCorrectPrices(
            int numberOfLabels,
            double rangeMin,
            double rangeMax,
            string expectedListString)
        {
            var expectedList = expectedListString.Split(";").ToList();

            var sut = new PriceLabelsUtility();
            var resultList = sut.GetPrices(
                numberOfLabels,
                rangeMin,
                rangeMax);

            throw new NotImplementedException();
        }

        [Fact]
        public void GetCorrectCanvasTops()
        {
            throw new NotImplementedException();
        }
    }
}
