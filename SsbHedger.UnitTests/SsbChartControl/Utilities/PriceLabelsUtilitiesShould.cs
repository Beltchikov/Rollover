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
            var expectedList = expectedListString
                .Split(";")
                .Select(m => Convert.ToDouble(m))
                .ToList();

            var sut = new PriceLabelsUtility();
            var resultList = sut.GetPrices(
                numberOfLabels,
                rangeMin,
                rangeMax);

            Assert.Equal(expectedList.Count, resultList.Count);
            
            Assert.Equal(expectedList[0], resultList[0]);
            Assert.Equal(expectedList[1], resultList[1]);
            Assert.Equal(expectedList[1], resultList[1]);
            Assert.Equal(expectedList[1], resultList[1]);
        }

        //[Fact]
        //public void GetCorrectCanvasTops()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
