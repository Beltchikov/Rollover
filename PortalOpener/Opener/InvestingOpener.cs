using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.Design.AxImporter;

namespace PortalOpener.Opener
{
    internal class InvestingOpener : IOpener
    {
        private readonly int TIMEOUT_WAIT = 5000;

        private readonly string URL_INVESTING = "https://www.investing.com/search/?q=";
        private readonly string[] TA_LABELS = ["Strong Buy", "Buy", "Neutral", "Sell", "Strong Sell"];

        public string Execute(string[] symbols)
        {
            var _webDriver = new ChromeDriver();

            int i = 0;
            var symbolsCopy = symbols
                .Where(m=> !string.IsNullOrWhiteSpace(m))
                .Select(s => s.Trim())
                .ToArray();
            foreach (var symbol in symbolsCopy)
            {
                _webDriver.Navigate().GoToUrl(URL_INVESTING + symbol);

                var wait = new WebDriverWait(_webDriver, TimeSpan.FromMilliseconds(TIMEOUT_WAIT));
                var searchSectionMain = wait.Until(x => x.FindElement(By.CssSelector("div.searchSectionMain")));
                var linkWithEquitiesHref = searchSectionMain.FindElement(By.CssSelector("a[href^=\"/equities/\"]"));

                var secondUrl = linkWithEquitiesHref.GetAttribute("href");
                _webDriver.Navigate().GoToUrl(secondUrl);

                //try
                //{
                //    var divTechnical = wait.Until(x =>
                //                x.FindElement(By.XPath($"//div[text()='{TA_LABELS[0]}']")
                //            ));
                //}
                //catch (NoSuchElementException)
                //{

                //    var divTechnical = wait.Until(x =>
                //                x.FindElement(By.XPath($"//div[text()='{TA_LABELS[4]}']")
                //            ));
                //}

                //IWebElement divTechnical;
                //string taLabel = TA_LABELS[0];
                //while (!(divTechnical = WaitForSomeText(wait, taLabel)).Displayed)
                //{

                //}

                var divTechnical = WaitForSomeText(wait, TA_LABELS);


                i++;
                if(i < symbolsCopy.Length) _webDriver.SwitchTo().NewWindow(WindowType.Tab);
            }

            return "";
        }

        private IWebElement WaitForSomeText(WebDriverWait wait, string[] allTexts, int index = 0)
        {
            for(int i = index; i < allTexts.Length; i++)
            {
                var text = allTexts[i];
                try
                {
                    return wait.Until(x => x.FindElement(By.XPath($"//div[text()='{text}']")));
                }
                catch (NoSuchElementException)
                {
                    if (i == allTexts.Length - 1) throw;
                    return WaitForSomeText(wait, allTexts, i);
                }
            }
            
           throw new NoSuchElementException();
        }
    }
}
