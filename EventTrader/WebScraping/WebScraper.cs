using Microsoft.CodeAnalysis;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Globalization;
using System;

namespace EventTrader.WebScraping
{
    public class WebScraper : IWebScraper
    {
        private ISimpleBrowser _browser;

        public WebScraper(ISimpleBrowser browser)
        {
            _browser = browser;
        }

        public double AudInterestRate(string url, string pattern)
        {
            //throw new NotImplementedException();

            // TODO
            return 33.44;
        }

        public double UsdInterestRate(string url, string pattern)
        {
            //throw new System.NotImplementedException();
            // TODO
            //return 5.25;

            //////////////////////////////////
            ///
            url = @$"https://www.forexfactory.com/calendar";
            pattern = "";
            var result = _browser.OneValueResult(url, pattern);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new ApplicationException("Empty string returned.");
            }

            double doubleResult = Convert.ToDouble(result, new CultureInfo("EN-us"));
            return doubleResult;
        }
    }
}