using AutoFixture;
using SsbHedger.SsbChartControl;

namespace SsbHedger.UnitTests.SsbChartControl
{
    public class LineValuesConverterShould
    {
        [Theory]
        [InlineData("15:30", 
            "22:15", 
            "15:30;15:45;" +
            "16:00;16:15;16:30;16:45;17:00;17:15;17:30;17:45;" +
            "18:00;18:15;18:30;18:30;18:45;19:00;19:15;19:30;" +
            "20:00;20:15;20:30;20:45;21:00;21:15;21:30;21:45;" +
            "22:00;22:15",
            "0;0;" +
            "1;0;0;0;0;0;0;0;" +
            "1;0;0;0;0;0;0;0;" +
            "1;0;0;0;0;0;0;0;" +
            "1;0")]
        public void ReturnLineTimesCorrectly(
            string sessionStartString,
            string sessionEndString,
            string expectedLineTimesListString,
            string expectedDisplayableFlagString)
        {
            var sessionStart = DateTime.Parse(sessionStartString);
            var sessionEnd = DateTime.Parse(sessionEndString);
            
            List<DateTime> expectedLineTimesList = new List<DateTime>();
            expectedLineTimesListString
                .Split(";")
                .ToList()
                .ForEach(ts => expectedLineTimesList.Add(ParseTimeString(ts)));

            List<bool> expectedDisplayableFlagList= new List<bool>();
            expectedDisplayableFlagString
                .Split(";")
                .ToList()
                .ForEach(df => expectedDisplayableFlagList.Add(
                    Convert.ToBoolean(Convert.ToInt32(df))));

            var sut = (new Fixture()).Create<LineValuesConverter>();
            var lineTimesDictionary = sut.LineTimes(sessionStart, sessionEnd, 2);

            Assert.IsType<Dictionary<DateTime, bool>>(lineTimesDictionary);
            Assert.Equal(expectedLineTimesList.Count, lineTimesDictionary.Count);

            for (int i = 0; i < expectedLineTimesList.Count; i++)
            {
                var expectedLineTime = expectedLineTimesList[i];
                var expectedDisplayFlag= expectedDisplayableFlagList[i];
                var lineTime = lineTimesDictionary.Keys.First(k => k == expectedLineTime);
                var displayFlag = lineTimesDictionary[lineTime];

                Assert.Equal(expectedLineTime.Hour, lineTime.Hour);
                Assert.Equal(expectedLineTime.Minute, lineTime.Minute);
                Assert.Equal(0, lineTime.Second);
                Assert.Equal(expectedDisplayFlag, displayFlag);
            }
        }

        private DateTime ParseTimeString(string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
            {
                return DateTime.MinValue;
            }

            return DateTime.Parse(timeString);
        }
    }
}
