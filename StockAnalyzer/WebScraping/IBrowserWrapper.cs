using System.Net;
using System.Xml.Linq;

namespace StockAnalyzer.WebScraping
{
    public interface IBrowserWrapper
    {
        public bool Navigate(string url);
        public string Text { get; }
        public string CurrentHtml { get; }
        public XDocument XDocument { get; }
        WebException LastWebException { get; }
        void SetHeader(string header);
        void RemoveHeader(string header);
    }
}