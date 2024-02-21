using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace StockAnalyzer.DataProviders
{
    public class SeekingAlphaProvider : ProviderBase, ISeekingAlphaProvider
    {
        IWebDriver? driver = null;
        public SeekingAlphaProvider() : base() { }
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
                var userAgent = $"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0";

                ChromeOptions options = new();
                //options.AddArgument("--headless=new");
                //options.AddArgument($"--user-agent={userAgent}");
                // options.AddArgument($"--referer={url}");
                options.AddArgument("--enable-javascript");
                driver = new ChromeDriver(options);
                driver.Manage().Cookies.DeleteAllCookies();
                driver.Navigate().GoToUrl(url);

                IWebElement epsElement = WaitUntilElementExists(By.XPath("//div[text() = 'EPS (FWD)']"));
                var epsElementParent = epsElement.FindElement(By.XPath("parent::*"));
                var silbing = epsElementParent.FindElement(By.XPath("following-sibling::* "));
                var epsValueElement = silbing.FindElement(By.XPath("descendant::*"));

                //result.Add(epsValueElement.Text);
                var t = epsValueElement?.GetAttribute("outerHTML");
                if(t != null) result.Add(t);

            });

            return result;
        }

        public IWebElement WaitUntilElementExists(By elementLocator, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                return wait.Until(ExpectedConditions.ElementExists(elementLocator));
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Element with locator: '" + elementLocator + "' was not found.");
                throw;
            }
        }


    }
}