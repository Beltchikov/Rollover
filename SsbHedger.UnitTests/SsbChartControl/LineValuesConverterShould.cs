using AutoFixture;
using SsbHedger.SsbChartControl;

namespace SsbHedger.UnitTests.SsbChartControl
{
    public class LineValuesConverterShould
    {
        [Theory]
        [InlineData("15:30", "22:15", "16:00;18:00;20:00;22:00")]
        public void ReturnLineTimesCorrectly(
            string sessionStartString,
            string sessionEndString,
            string expectedLineTimesListString)
        {
            var sessionStart = DateTime.Parse(sessionStartString);
            var sessionEnd = DateTime.Parse(sessionEndString);
            List<DateTime> expectedLineTimesList = new List<DateTime>();
            expectedLineTimesListString
                .Split(";")
                .ToList()
                .ForEach(ts => expectedLineTimesList.Add(DateTime.Parse(ts)));


            var sut = (new Fixture()).Create<LineValuesConverter>();
            var lineTimes = sut.LineTimes(sessionStart, sessionEnd);

            Assert.IsType<List<DateTime>>(lineTimes);
            Assert.Equal(expectedLineTimesList.Count, lineTimes.Count);

            for(int i = 0; i < expectedLineTimesList.Count; i++)
            {
                var expectedLineTime = expectedLineTimesList[i];    
                var lineTime = lineTimes[i];    
                Assert.Equal(expectedLineTime.Hour, lineTime.Hour);
                Assert.Equal(expectedLineTime.Minute, lineTime.Minute);
                Assert.Equal(0, lineTime.Minute);
                Assert.Equal(0, lineTime.Second);
            }
        }
    }
}
