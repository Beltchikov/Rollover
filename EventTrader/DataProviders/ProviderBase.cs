using Dsmn.WebScraping;
using System;
using System.Xml;

namespace Dsmn.DataProviders
{
    public class ProviderBase
    {
        protected readonly IBrowserWrapper _browserWrapper;
        protected readonly XmlNamespaceManager _xmlNamespaceManager;

        public event Action<string> Status = null!;

        public ProviderBase(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }

        public void TriggerStatus(string message)
        {
            Status.Invoke(message);
        }
    }
}
