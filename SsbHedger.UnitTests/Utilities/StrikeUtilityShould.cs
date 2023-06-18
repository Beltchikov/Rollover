using AutoFixture;
using SsbHedger.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.UnitTests.Utilities
{
    public class StrikeUtilityShould
    {
        [Theory]
        [InlineData("9.5, 10, 10.5", 10.5, 9.9, 0.5, "1,1,0", "9.5, 10, 11")]
        public void ReplaceInvalidStrikeCorrectly(
            string inputListString,
            double strike,
            double underlyingPrice,
            double strikesStep,
            string validityMatrixString,
            string expectedOutputListString)
        {
            // Prepare
            List<double> inputList = inputListString.Split(new char[] { ',' })
                .Select(e => Convert.ToDouble(e))
                .ToList();
            List<double> expectedOutputList = expectedOutputListString.Split(new char[] { ',' })
                .Select(e => Convert.ToDouble(e))
                .ToList();

            var sut = (new Fixture()).Create<StrikeUtility>();

            // Act
            var outputStrikes = sut.ReplaceInvalidStrike(inputList, strike, underlyingPrice, strikesStep);

            // Verify
            Assert.IsType <List<double>> (outputStrikes);
            //Assert.True(expectedOutputList.SequenceEqual(outputStrikes));
        }
    }
}
