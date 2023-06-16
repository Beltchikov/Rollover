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
        [InlineData("9.5, 10, 10.5", 10.5, 9.9, 0.5, "9.5, 10, 11")]
        public void ReplaceInvalidStrikeCorrectly(
            string inputListString,
            double strike,
            double underlyingPrice,
            double strikesStep,
            string expectedOutputListString)
        {
            throw new NotImplementedException();
        }
    }
}
