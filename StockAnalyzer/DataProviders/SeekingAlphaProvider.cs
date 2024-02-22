using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.IO;
using System.Diagnostics;
using StockAnalyzer.DataProviders.Types;

namespace StockAnalyzer.DataProviders
{
    public class SeekingAlphaProvider : ProviderBase, ISeekingAlphaProvider
    {
        IWebDriver? driver = null;
        public SeekingAlphaProvider() : base() { }
        public async Task<IEnumerable<string>> PeersComparison(string ticker, int delay)
        {
            string urlTemplate = $"https://seekingalpha.com/symbol/TICKER";
            var tickerTrimmed = ticker.Trim();
            string url = urlTemplate.Replace("TICKER", ticker.Trim());
            var urlWithoutScheme = UrlWithoutScheme(url);

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
                    Arguments = "--remote-debugging-port=9977 seekingalpha.com/symbol/MSFT"
                }
            };
            proc.Start();

            var result = new List<string>();
            await Task.Run(() =>
            {
                TriggerStatus($"Retrieving peers for {tickerTrimmed}");
                ChromeOptions options = new()
                {
                    DebuggerAddress = "127.0.0.1:9977" // "http://127.0.0.1:9977"
                };
                options.AddArgument("--enable-javascript");
                driver = new ChromeDriver(options);

                // Button Accept All Cookies
                var buttonAcceptAllCookiesOrError = WaitUntilElementExists(By.XPath("//button[text() = 'Accept All Cookies']"));
                buttonAcceptAllCookiesOrError.Value?.Click();
                
                // Get peers
                var peersElementOrError = WaitUntilElementExists(By.XPath("//h2[text() = 'Peers']"));
                if(peersElementOrError.Error != null) result.Add(peersElementOrError.Error);
                else if(peersElementOrError.Value != null) result.Add(peersElementOrError.Value.GetAttribute("outerHTML"));
                else throw new Exception("Unexpected");

                // IWebElement peersElement = WaitUntilElementExists(By.XPath("//h2[text() = 'Peers']"));
                // result.Add(peersElement.GetAttribute("outerHTML"));

                // var peersElementParent1 = peersElement.FindElement(By.XPath("parent::*"));
                // var peersElementParent2 = peersElementParent1.FindElement(By.XPath("parent::*"));
                // var peersElementParent3 = peersElementParent2.FindElement(By.XPath("parent::*"));
                // var peersSibling = peersElementParent3.FindElement(By.XPath("following-sibling::* "));

                // // var firstTrElement = peersElementParent3.FindElement(By.XPath("//tr"));
                // // result.Add(firstTrElement.GetAttribute("outerHTML"));

                // //result.Add(peersSibling.GetAttribute("outerHTML"));
                // result.Add(peersElementParent3.GetAttribute("outerHTML"));

                // IWebElement epsElement = WaitUntilElementExists(By.XPath("//div[text() = 'EPS (FWD)']"));
                // var epsElementParent = epsElement.FindElement(By.XPath("parent::*"));
                // var epsSibling = epsElementParent.FindElement(By.XPath("following-sibling::* "));
                // var epsValueElement = epsSibling.FindElement(By.XPath("descendant::*"));

                // //var t = epsValueElement?.GetAttribute("outerHTML");
                // // TODO Fehler
                // var t = epsValueElement?.Text;
                // if (t != null) result.Add(t);

            });

            return result;
        }

        private static void ClickButtonIfExists(IWebDriver driver, string xPath)
        {
            var buttons = driver.FindElements(By.XPath(xPath));
            if (buttons.Count > 0 && buttons[0].Enabled && buttons[0].Displayed)
            {
                buttons[0].Click();
            }
        }

        private static string UrlWithoutScheme(string url)
        {
            Uri uri = new(url);
            return uri.Host + uri.PathAndQuery + uri.Fragment;
        }

        public WithError<IWebElement> WaitUntilElementExists(By elementLocator, bool throwException = true, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                return new WithError<IWebElement>(wait.Until(ExpectedConditions.ElementExists(elementLocator)));
            }
            catch (NoSuchElementException e)
            {
                if(throwException) throw;
                return new WithError<IWebElement>(e.ToString());
            }
        }


    }
}

// Usefull code lines

//var userAgent = $"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0";

//call "C:\Program Files\Google\Chrome\Application\chrome.exe" --remote-debugging-port=9977 seekingalpha.com/symbol/MSFT
//string cmdCommand = "call \"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe \" --remote-debugging-port=9977 seekingalpha.com/symbol/MSFT";

//ChromeOptions options = new();
//options.AddArgument("--headless=new");
//options.AddArgument($"--user-agent={userAgent}");
// options.AddArgument($"--referer={url}");
//options.DebuggerAddress= "127.0.0.1:9977";

//driver.Manage().Cookies.DeleteAllCookies();
//driver.Navigate().GoToUrl("http://127.0.0.1:9977");
