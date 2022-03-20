namespace UsMoversOpening.Configuration
{
    public class Configuration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int ClientId { get; set; }
        public int Timeout { get; set; }
        public int RiskInUsd { get; set; }
        public int NumberOfStocksInScanner { get; set; }
        public int NumberOfStocksToBuy { get; set; }
        public string TimeToBuy { get; set; }
    }
}


