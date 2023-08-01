namespace EventTrader.WebScraping
{
    public interface IWebScraper
    {
        public double AudInterestRate(string url, string pattern);
        double UsdInterestRate(string url, string pattern);
    }
}