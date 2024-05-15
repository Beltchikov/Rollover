using System.Collections.ObjectModel;
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

        public static List<int> For(this ObservableCollection<KeyValuePair<string, List<int>>> signals, string symbol)
        {
            return signals.Any(kvp => kvp.Key == symbol)
                ? signals.First(kvp => kvp.Key == symbol).Value
                : new List<int>();
        }
    }
}
