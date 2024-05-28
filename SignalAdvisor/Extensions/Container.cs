using System.Collections.ObjectModel;
using System.Globalization;
using Ta;

namespace SignalAdvisor.Extensions
{
    public static class Container
    {
        public static List<Bar> For(this ObservableCollection<KeyValuePair<string, List<Bar>>> bars, string symbol)
        {
            return bars.Any(kvp => kvp.Key == symbol)
                ? bars.First(kvp => kvp.Key == symbol).Value
                : new List<Bar>();
        }

        public static List<Dictionary<string, int>> For(this ObservableCollection<KeyValuePair<string, List<Dictionary<string, int>>>> signals, string symbol)
        {
            return signals.Any(kvp => kvp.Key == symbol)
                ? signals.First(kvp => kvp.Key == symbol).Value
                : new List<Dictionary<string, int>>();
        }

        public static DateTimeOffset DateTimeOffsetFromString(this string dateTimeOffsetString)
        {
            var dtoStringSplitted = dateTimeOffsetString
                          .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => s.Trim())
                          .ToArray();
            var timeZoneString = dtoStringSplitted[2];
            var dateTimeString = dtoStringSplitted
                .Take(2)
                .Aggregate((r, n) => r + " " + n);

            var dateTime = DateTime.ParseExact(dateTimeString, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
            var result = new DateTimeOffset(dateTime, timeZoneInfo.BaseUtcOffset);

            return result;
        }
    }
}
