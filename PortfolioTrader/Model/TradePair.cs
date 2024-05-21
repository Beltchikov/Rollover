namespace PortfolioTrader.Model
{
    internal class TradePair
    {
        public TradePair(
            string symbolBuy,
            string symbolSell,
            int conIdBuy,
            int conIdSell,
            int priceInCentsBuy,
            int priceInCentsSell,
            int quantityBuy,
            int quantitySell,
            int barInCentsBuy,
            int barInCentsSell)
        {
            SymbolBuy = symbolBuy;
            SymbolSell = symbolSell;
            ConIdBuy = conIdBuy;
            ConIdSell = conIdSell;
            PriceInCentsBuy = priceInCentsBuy;
            PriceInCentsSell = priceInCentsSell;
            QuantityBuy = quantityBuy;
            QuantitySell = quantitySell;
            BarInCentsBuy = barInCentsBuy;
            BarInCentsSell = barInCentsSell;
        }

        public string SymbolBuy { get; set; }
        public string SymbolSell { get; set; }
        public int ConIdBuy { get; set; }
        public int ConIdSell { get; set; }
        public int PriceInCentsBuy { get; set; }
        public int PriceInCentsSell { get; set; }
        public int QuantityBuy { get; set; }
        public int QuantitySell { get; set; }
        public int BarInCentsBuy { get; set; }
        public int BarInCentsSell { get; set; }


    }
}
