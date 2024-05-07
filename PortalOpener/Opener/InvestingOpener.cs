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
        private readonly object STRONG_BUY = "Strong Buy";
        private readonly object BUY = "Buy";
        private readonly object NEUTRAL = "Neutral";
        private readonly object SELL = "Sell";
        private readonly object STRONG_SELL = "Strong Sell";

        public string Execute(string[] symbols)
        {
            var _webDriver = new ChromeDriver();
            _webDriver.Navigate().GoToUrl("https://www.investing.com/search/?q=tsla");
           
            var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
            var searchSectionMain = wait.Until(x => x.FindElement(By.CssSelector("div.searchSectionMain")));

            //var linkWithEquitiesHref = wait.Until(x => x.FindElement(By.CssSelector("div.searchSectionMain")));
            var linkWithEquitiesHref = searchSectionMain.FindElement(By.CssSelector("a[href^=\"/equities/\"]"));
            //var linkWithEquitiesHref = searchSectionMain.FindElement(By.CssSelector("a[@href~=\"equities\"]"));

            var secondUrl = linkWithEquitiesHref.GetAttribute("href");

            _webDriver.Navigate().GoToUrl(secondUrl);

            //driver.FindElements(By.XPath("//h1[text()='Table Pagination Demo']"));
            //WebDriverWait(driver, 15).until_not(lambda driver: driver.find_element(By.XPATH, layovers[0]) and driver.find_element(By.XPATH, layovers[1]))

            var divTechnical = wait.Until(x =>
            x.FindElement(By.XPath($"//div[text()='{STRONG_BUY}']")
            ));

            //IWebElement divTechnical = wait.Until(x => {
            //    if (x.FindElement(By.XPath($"//div[text()='{STRONG_BUY}']")).Displayed)
            //    {
            //        return x.FindElement(By.XPath($"//div[text()='{STRONG_BUY}']"));
            //    }
            //    else throw new NoSuchElementException();  
            //}); 

            // TODO
            return "";
        }
    }
}
