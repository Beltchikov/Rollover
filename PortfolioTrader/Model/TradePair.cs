using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrader.Model
{
    internal class TradePair
    {
        public TradePair(
            string symbolBuy,
            string symbolSell,
            int conIdBuy,
            int conIdSell,
            int quantityBuy,
            int quantitySell)
        {
            SymbolBuy = symbolBuy;
            SymbolSell = symbolSell;
            ConIdBuy = conIdBuy;
            ConIdSell = conIdSell;
            QuantityBuy = quantityBuy;
            QuantitySell = quantitySell;
        }

        public string SymbolBuy { get; set; }
        public string SymbolSell { get; set; }
        public int ConIdBuy { get; set; }
        public int ConIdSell { get; set; }
        public int QuantityBuy { get; set; }
        public int QuantitySell { get; set; }
    }
}
