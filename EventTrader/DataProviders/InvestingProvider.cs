﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Dsmn.DataProviders
{
    public class InvestingProvider : IInvestingProvider
    {
        XmlNamespaceManager _xmlNamespaceManager;

        public InvestingProvider()
        {
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }

        public List<string> GetEarningsData(string htmlSource)
        {
            var result = new List<string>();

            htmlSource = htmlSource.Replace("/&nbsp;", "");
            htmlSource = htmlSource.Replace("&nbsp;", "");
            var xDocument = XDocument.Parse(htmlSource);
            var tableRows = xDocument.Descendants("tr").ToList();

            MessageBox.Show("GetEarningsData");

            return result;

        }
    }
}
