using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.UnitTests.SsbChartControl.WpfConverters
{
    public class GridRectConverterShould
    {
        [Theory]
        [InlineData(";;16:00;;;;;;;;18:00;;;;;;;;20:00;;;;;;;;22:00;",
            10,
            580)]
        void ConvertCorrectly(string lineTimesString, int barWidth, int controlWidth)
        {
            List<DateTime> lineTimes; // TODO
        }
    }
}
