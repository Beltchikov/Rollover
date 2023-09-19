using Dsmn.WebScraping;
using System.Xml;

namespace Dsmn.DataProviders
{
    public class ProviderBase
    {
        protected readonly IBrowserWrapper _browserWrapper;
        protected readonly XmlNamespaceManager _xmlNamespaceManager;

        public ProviderBase(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }
    }
}
