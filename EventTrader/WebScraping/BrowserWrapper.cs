using System.Diagnostics.CodeAnalysis;
using SimpleBrowser;

namespace EventTrader.WebScraping
{
    [ExcludeFromCodeCoverage]
    public class BrowserWrapper : IBrowserWrapper
    {
        private Browser _browser;

        public BrowserWrapper()
        {
            _browser = new Browser();
        }

        public bool Navigate(string url)
        {
            return _browser.Navigate(url);
        }

        public string Text => _browser.Text;
    }
}
