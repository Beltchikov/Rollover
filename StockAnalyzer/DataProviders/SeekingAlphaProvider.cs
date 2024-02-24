using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics;
using StockAnalyzer.DataProviders.Types;

namespace StockAnalyzer.DataProviders
{
    public class SeekingAlphaProvider : ProviderBase, ISeekingAlphaProvider
    {
        IWebDriver? driver = null;
        public SeekingAlphaProvider() : base() { }
        public IEnumerable<string> PeersComparison(List<string> tickerList, int delay)
        {
            string urlTemplate = $"https://seekingalpha.com/symbol/TICKER";
            var result = new List<string>();
            var basePort = 9977;

            int i = 0;
            foreach (var ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();
                var port = basePort + i;
                string url = urlTemplate.Replace("TICKER", ticker.Trim());
                var urlWithoutScheme = UrlWithoutScheme(url);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
                        Arguments = $"--remote-debugging-port={port} seekingalpha.com/symbol/{tickerTrimmed}"
                    }
                };
                process.Start();

                TriggerStatus($"Retrieving peers for {tickerTrimmed}");
                ChromeOptions options = new()
                {
                    DebuggerAddress = $"127.0.0.1:{port}" // "http://127.0.0.1:9977"
                };
                options.AddArgument("--enable-javascript");
                driver = new ChromeDriver(options);
                //driver.SwitchTo().Window()
                //var windowHandle = driver.CurrentWindowHandle;
                

                // Button Accept All Cookies
                var buttonAcceptAllCookiesOrError = WaitUntilElementExists(driver, By.XPath(
                    "//button[text() = 'Accept All Cookies']"), false);
                buttonAcceptAllCookiesOrError.Value?.Click();

                // EPS
                WithError<IWebElement> epsElement = WaitUntilElementExists(driver, By.XPath(
                    "//div[text() = 'EPS (FWD)']/../following-sibling::*/div"));
                if (epsElement.Error != null) result.Add(epsElement.Error);
                if (epsElement.Value != null) result.Add(epsElement.Value.Text);
                
                // ROE
                //div[text() = 'Return on Equity']/../following-sibling::*/div
                WithError<IWebElement> roeElement = WaitUntilElementExists(driver, By.XPath(
                    "//div[text() = 'Return on Equity']/../following-sibling::*/div"));
                if (roeElement.Error != null) result.Add(roeElement.Error);
                if (roeElement.Value != null) result.Add(roeElement.Value.Text);

                // DIV
                // //div[text() = 'Latest Announced Dividend']/../following-sibling::*/div
                WithError<IWebElement> divElement = WaitUntilElementExists(driver, By.XPath(
                    "//div[text() = 'Latest Announced Dividend']/../following-sibling::*/div"));
                if (divElement.Error != null) result.Add(divElement.Error);
                if (divElement.Value != null) result.Add(divElement.Value.Text);

                // Beta
                // //div[text() = '24M Beta']/../following-sibling::*/div
                WithError<IWebElement> betaElement = WaitUntilElementExists(driver, By.XPath(
                    "//div[text() = '24M Beta']/../following-sibling::*/div"));
                if (betaElement.Error != null) result.Add(betaElement.Error);
                if (betaElement.Value != null) result.Add(betaElement.Value.Text);

                driver.Quit();
                process.Kill();

                i++;
            }

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

        public WithError<IWebElement> WaitUntilElementExists(
            IWebDriver driver,
            By elementLocator,
            bool throwException = true,
            int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                var element = wait.Until(ExpectedConditions.ElementExists(elementLocator));
                return new WithError<IWebElement>(element);
            }
            catch (NoSuchElementException e)
            {
                if (throwException) throw;
                return new WithError<IWebElement>(e.ToString());
            }
            catch (WebDriverTimeoutException e)
            {
                if (throwException) throw;
                return new WithError<IWebElement>(e.ToString());
            }
        }


    }
}

// Know How

//var userAgent = $"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0";

//call "C:\Program Files\Google\Chrome\Application\chrome.exe" --remote-debugging-port=9977 seekingalpha.com/symbol/MSFT
//string cmdCommand = "call \"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe \" --remote-debugging-port=9977 seekingalpha.com/symbol/MSFT";

//ChromeOptions options = new();
//options.AddArgument("--headless=new");
//options.AddArgument($"--user-agent={userAgent}");
//options.AddArgument($"--referer={url}");
//options.DebuggerAddress= "127.0.0.1:9977";

//driver.Manage().Cookies.DeleteAllCookies();
//driver.Navigate().GoToUrl("http://127.0.0.1:9977");

// // Scroll
// Actions actions = new(driver);
// actions.MoveToElement(peersElementOrError.Value);
// actions.Perform();

// Switch to a tab
// List<String> tabs = new List<String> (driver.WindowHandles);
// driver.SwitchTo().Window(tabs[2]);
// driver.Close();
// driver.SwitchTo().Window(tabs[3]);