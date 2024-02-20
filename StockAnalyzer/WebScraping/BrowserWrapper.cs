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
            _browser = new Browser
            {
                //UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10"
            };
        }

        public bool Navigate(string url)
        {
            return _browser.Navigate(url);
        }

        public void SetHeader(string header)
        {
            _browser.SetHeader(header);
        }

        public string Text => _browser.Text;
        public string CurrentHtml => _browser.CurrentHtml;
        public XDocument XDocument => _browser.XDocument;

        public WebException LastWebException => _browser.LastWebException;
    }
}
