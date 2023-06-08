using AutoFixture;
using SsbHedger.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.UnitTests.Utilities
{
    public class LastTradeDateConverterShould
    {
        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void ConvertInProperFormat(int dte)
        {
            var expirationDate = DateTime.Now.AddDays(dte);

            var sut = (new Fixture()).Create<LastTradeDateConverter>();
            var lastTradeDate = sut.FromDte(dte);
            // TODO Assert
        }
    }
}
