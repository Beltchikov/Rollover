using AutoFixture;
using AutoFixture.AutoNSubstitute;
using SsbHedger.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.UnitTests.Utilities
{
    public class StrikeUtilityShould
    {
        [Theory]
        [InlineData("9.5, 10, 10.5", 10.5, 9.9, 0.5, "9.5, 10, 11")]
        public void ReplaceInvalidStrikeCorrectly(
            string inputListString,
            double invalidStrike,
            double underlyingPrice,
            double strikesStep,
            string expectedOutputListString)
        {
            // Prepare
            List<double> inputList = inputListString.Split(new char[] { ',' })
                .Select(e => Convert.ToDouble(e, CultureInfo.InvariantCulture))
                .ToList();
            List<double> expectedOutputList = expectedOutputListString.Split(new char[] { ',' })
                .Select(e => Convert.ToDouble(e, CultureInfo.InvariantCulture))
                .ToList();
            
            var sut = (new Fixture()).Create<StrikeUtility>();

            // Act
            var outputStrikes = sut.ReplaceInvalidStrike(inputList, invalidStrike, underlyingPrice, strikesStep);

            // Verify
            Assert.IsType <List<double>> (outputStrikes);
            Assert.True(expectedOutputList.Count == outputStrikes.Count);
            Assert.True(expectedOutputList.SequenceEqual(outputStrikes));
        }
    }
}
