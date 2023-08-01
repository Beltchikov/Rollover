namespace EventTrader.WebScraping
{
    public interface IBrowserWrapper
    {
        public bool Navigate(string url);
        public string Text { get; }
    }
}