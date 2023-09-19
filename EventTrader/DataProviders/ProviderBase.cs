using Dsmn.WebScraping;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Dsmn.DataProviders
{
    public class ProviderBase
    {
        protected readonly IBrowserWrapper _browserWrapper;
        protected readonly XmlNamespaceManager _xmlNamespaceManager;

        public event Action<string> Status = null!;

        protected ProviderBase(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }

        protected void TriggerStatus(string message)
        {
            Status.Invoke(message);
        }

        protected string RegexMatch(string text, string regexPattern, int index)
        {
            var rx = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = rx.Matches(text).ToList();
            return matches.Count <= 0
                ? string.Empty
                : matches[index].ToString();
        }
    }
}
