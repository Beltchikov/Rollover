using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using SimpleBrowser;

namespace Eomn.WebScraping
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
        public string CurrentHtml => _browser.CurrentHtml;
        public XDocument XDocument => _browser.XDocument;
    }
}
