using SimpleBrowser;
using System.Linq;
using System.Text.RegularExpressions;

namespace EventTrader.WebScraping
{
    public class SimpleBrowser : ISimpleBrowser
    {
        private IBrowserWrapper _browserWrapper;

        public SimpleBrowser(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
        }

        public string OneValueResult(string url, string regExPattern1, string regExPattern2)
        {
            // https://github.com/SimpleBrowserDotNet/SimpleBrowser
            _browserWrapper.Navigate(url);
            var responseText = _browserWrapper.Text;

            var rx = new Regex(regExPattern1, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var matchCollection1 = rx.Matches(responseText).ToList();
            if (!matchCollection1.Any())
            {
                return string.Empty;
            }
            var regExResult1 = matchCollection1.First().Value;

            if (string.IsNullOrWhiteSpace(regExPattern2))
            {
                return regExResult1;
            }

            rx = new Regex(regExPattern2, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchCollection2 = rx.Matches(regExResult1).ToList();
            if (!matchCollection2.Any())
            {
                return string.Empty;
            }

            return matchCollection2.First().Value;
        }
    }
}
