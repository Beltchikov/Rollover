using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using StockAnalyzer.WebScraping;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace StockAnalyzer.DataProviders
{
    public class SeekingAlphaProvider : BrowserProviderBase, ISeekingAlphaProvider
    {
        public SeekingAlphaProvider(IBrowserWrapper browserWrapper) : base(browserWrapper) { }
        public async Task<IEnumerable<string>> PeersComparison(string ticker, int delay)
        {
            string urlTemplate = $"https://seekingalpha.com/symbol/TICKER";
            string url = "";

            var result = new List<string>();

            var tickerTrimmed = ticker.Trim();
            await Task.Run(() =>
            {
                TriggerStatus($"Retrieving peers for {tickerTrimmed}");
                url = urlTemplate.Replace("TICKER", tickerTrimmed);
                var headerUserAgent = $"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0";

                IWebDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl(url);  

                IWebElement ele = driver.FindElement(By.CssSelector("li:nth-child(1)"));  

            });



            return result;
        }




    }
}