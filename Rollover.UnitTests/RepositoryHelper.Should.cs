using AutoFixture;
using Rollover.Ib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class RepositoryHelperShould
    {
        [Theory]
        [InlineData("14.02.2022 11:02", true)]
        public void CheckTradingHours(string dateString, bool expectedResult)
        {
            var tradingHoursString = "20220214:0900-20220214:1730;" +
                "20220215:0900-20220215:1730;20220216:0900-20220216:1730;" +
                "20220217:0900-20220217:1730;20220218:0900-20220218:1730";

            RepositoryHelper sut = new Fixture().Create<RepositoryHelper>();

            var dateTime = DateTime.Parse(dateString, new CultureInfo("DE-de"));
            var result = sut.IsInTradingHours(tradingHoursString, dateTime);
            Assert.Equal(expectedResult, result);

        }
    }
}
