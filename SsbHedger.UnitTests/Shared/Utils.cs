namespace SsbHedger.UnitTests.Shared
{
    public static class Utils
    {

        public static Dictionary<DateTime, bool> BuildDateTimeDictionary(
            string lineTimesString,
            string displayFlagString)
        {
            Dictionary<DateTime, bool> lineTimesDictionary = new Dictionary<DateTime, bool>();

            string[] lineTimesStringArray = lineTimesString.Split(";");
            string[] displayFlagStringArray = displayFlagString.Split(";");

            for (int i = 0; i < lineTimesStringArray.Length; i++)
            {
                var time = DateTime.Parse(lineTimesStringArray[i]);
                var displayFlag = Convert.ToBoolean(Convert.ToInt32(displayFlagStringArray[i]));

                lineTimesDictionary[time] = displayFlag;
            }

            return lineTimesDictionary;
        }
    }
}