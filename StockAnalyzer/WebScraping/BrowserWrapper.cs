using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Xml.Linq;
using SimpleBrowser;

namespace StockAnalyzer.WebScraping
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

        public void SetHeader(string header)
        {
            _browser.SetHeader(header);
        }

        public void RemoveHeader(string header)
        {
            _browser.RemoveHeader(header);
        }

        public string Text => _browser.Text;
        public string CurrentHtml => _browser.CurrentHtml;
        public XDocument XDocument => _browser.XDocument;

        public WebException LastWebException => _browser.LastWebException;
    }
}
