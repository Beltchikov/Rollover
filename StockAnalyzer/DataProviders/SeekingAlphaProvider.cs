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
using OpenQA.Selenium.Interactions;

namespace StockAnalyzer.DataProviders
{
    public class SeekingAlphaProvider : ProviderBase, ISeekingAlphaProvider
    {
        IWebDriver? driver = null;
        public SeekingAlphaProvider() : base() { }
        public async Task<IEnumerable<string>> PeersComparison(List<string> tickerList, int delay)
        {
            string urlTemplate = $"https://seekingalpha.com/symbol/TICKER";
            var result = new List<string>();
            var basePort = 9977;
            Dictionary<string, int> _tickerPortMap = new Dictionary<string, int>();
            Process[] _processes = new Process [tickerList.Count+1];
            IWebDriver[] _drivers = new WebDriver [tickerList.Count+1];

            int i = 1;
            foreach (var ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();
                var port = basePort+i;
                _tickerPortMap[tickerTrimmed] = port;
                string url = urlTemplate.Replace("TICKER", ticker.Trim());
                var urlWithoutScheme = UrlWithoutScheme(url);

                _processes[i] = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
                        Arguments = $"--remote-debugging-port={port} seekingalpha.com/symbol/MSFT"
                    }
                };
                _processes[i].Start();

                
                TriggerStatus($"Retrieving peers for {tickerTrimmed}");
                    ChromeOptions options = new()
                    {
                        DebuggerAddress = $"127.0.0.1:{port}" // "http://127.0.0.1:9977"
                    };
                    options.AddArgument("--enable-javascript");
                    _drivers[i] = new ChromeDriver(options);

                    // Button Accept All Cookies
                    var buttonAcceptAllCookiesOrError = WaitUntilElementExists(_drivers[i], By.XPath(
                        "//button[text() = 'Accept All Cookies']"), false);
                    buttonAcceptAllCookiesOrError.Value?.Click();

                    // EPS
                    WithError<IWebElement> epsElement = WaitUntilElementExists(_drivers[i], By.XPath(
                        "//div[text() = 'EPS (FWD)']/../following-sibling::*/div"));
                    if(epsElement.Error != null) result.Add(epsElement.Error);
                    if(epsElement.Value != null) result.Add(epsElement.Value.Text);

                    // //var t = epsValueElement?.GetAttribute("outerHTML");

                    _drivers[i].Quit();
                    _processes[i].Kill();

                // todo remove later
                break;
                
                
                // await Task.Run(() =>
                // {
                    // TriggerStatus($"Retrieving peers for {tickerTrimmed}");
                    // ChromeOptions options = new()
                    // {
                    //     DebuggerAddress = $"127.0.0.1:{port}" // "http://127.0.0.1:9977"
                    // };
                    // options.AddArgument("--enable-javascript");
                    // _drivers[i] = new ChromeDriver(options);

                    // // EPS
                    // WithError<IWebElement> epsElement = WaitUntilElementExists(_drivers[i], By.XPath(
                    //     "//div[text() = 'EPS (FWD)']/../following-sibling::*/div"));
                    // if(epsElement.Error != null) result.Add(epsElement.Error);
                    // if(epsElement.Value != null) result.Add(epsElement.Value.Text);

                    // // //var t = epsValueElement?.GetAttribute("outerHTML");

                    // _drivers[i].Quit();
                    
                    // // Peers
                    // var peersElementOrError = WaitUntilElementExists(By.XPath("//h2[text() = 'Peers']"));
                    // if (peersElementOrError.Error != null)
                    // {
                    //     result.Add(peersElementOrError.Error);
                    //     return;
                    // }
                    // else if(peersElementOrError.Value != null) result.Add(peersElementOrError.Value.GetAttribute("outerHTML"));
                    // else throw new Exception("Unexpected");

                    // // Scroll
                    // Actions actions = new(driver);
                    // actions.MoveToElement(peersElementOrError.Value);
                    // actions.Perform();

                    // Get peers
                    // //h2[text() = 'Peers']/../../../following-sibling::*/descendant::tr
                    // //h2[text() = 'Peers']/../../../following-sibling::*/descendant::tr/descendant::th
                    // var peersTrElementOrError = WaitUntilElementExists(By.XPath(
                    //     "//h2[text() = 'Peers']/../../../following-sibling::*/descendant::tr"));
                    // if (peersTrElementOrError.Value != null) result.Add(peersTrElementOrError.Value.GetAttribute("outerHTML"));
                    // else result.Add($"Unexpected! {peersTrElementOrError.Error}");

                    // Actions actions = new(driver);
                    // actions.MoveToElement(peersElementOrError.Value);
                    // actions.Perform();

                    // var peersElementParent1 = peersElementOrError.Value?.FindElement(By.XPath("parent::*"));
                    // var peersElementParent2 = peersElementParent1?.FindElement(By.XPath("parent::*"));
                    // var peersElementParent3 = peersElementParent2?.FindElement(By.XPath("parent::*"));
                    // var peersElementParent4 = peersElementParent3?.FindElement(By.XPath("parent::*"));
                    // var peersElementParent5 = peersElementParent4?.FindElement(By.XPath("parent::*"));

                    // if (peersElementParent5 != null) result.Add(peersElementParent5.GetAttribute("outerHTML"));
                    // else result.Add("Unexpected! peersElementParent5 is null");

                    // var peersSibling = peersElementParent3.FindElement(By.XPath("following-sibling::* "));

                    // // var firstTrElement = peersElementParent3.FindElement(By.XPath("//tr"));
                    // // result.Add(firstTrElement.GetAttribute("outerHTML"));

                    // //result.Add(peersSibling.GetAttribute("outerHTML"));
                    // result.Add(peersElementParent3.GetAttribute("outerHTML"));

                    
                    // // TODO Fehler
                    // var t = epsValueElement?.Text;
                    // if (t != null) result.Add(t);

                //});
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

// Usefull code lines

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
