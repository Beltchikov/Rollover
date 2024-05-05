using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrader.Model
{
    internal class PairOrderPosition
    {
        public PairOrderPosition()
        {
        }

        public PairOrderPosition(List<string> splitted)
        {
            if (splitted == null) throw new Exception("Unexpected. splitted may not be null");

            BuySymbol = splitted[1];
            BuyNetBms = Convert.ToInt32(splitted[2]);
            BuyConId = splitted.Count > 3 ? Convert.ToInt32(splitted[3]) : null;
            BuyWeight = splitted.Count > 4 ? Convert.ToInt32(splitted[4]) : null;
            BuyPriceInCents = splitted.Count > 5 ? Convert.ToInt32(splitted[5]) : null;
            BuyPriceType = splitted.Count > 6 ? splitted[6] : null;
            BuyQuantity = splitted.Count > 7 ? Convert.ToInt32(splitted[7]) : null;
            BuyMargin = splitted.Count > 8 ? Convert.ToInt32(splitted[8]) : null;
            BuyMarketValue = splitted.Count > 9 ? Convert.ToInt32(splitted[9]) : null;

            SellSymbol = splitted[10];
            SellNetBms = Convert.ToInt32(splitted[11]);
            SellConId = splitted.Count > 12 ? Convert.ToInt32(splitted[12]) : null;
            SellWeight = splitted.Count > 13 ? Convert.ToInt32(splitted[13]) : null;
            SellPriceInCents = splitted.Count > 14 ? Convert.ToInt32(splitted[14]) : null;
            SellPriceType = splitted.Count > 15 ? splitted[15] : null;
            SellQuantity = splitted.Count > 16 ? Convert.ToInt32(splitted[16]) : null;
            SellMargin = splitted.Count > 17 ? Convert.ToInt32(splitted[17]) : null;
            SellMarketValue = splitted.Count > 18 ? Convert.ToInt32(splitted[18]) : null;

            TotalMargin = splitted.Count > 19 ? Convert.ToInt32(splitted[19]) : null;
            Delta = splitted.Count > 20 ? Convert.ToDouble(splitted[20]) : null;
        }

        public string BuySymbol { get; set; }
        public int BuyNetBms { get; set; }
        public int? BuyConId { get; set; }
        public int? BuyWeight { get; set; }
        public int? BuyPriceInCents { get; set; }
        public string? BuyPriceType { get; set; }
        public int? BuyQuantity { get; set; }
        public int? BuyMargin { get; set; }
        public int? BuyMarketValue { get; set; }

        public string SellSymbol { get; set; }
        public int SellNetBms { get; set; }
        public int? SellConId { get; set; }
        public int? SellWeight { get; set; }
        public int? SellPriceInCents { get; set; }
        public string? SellPriceType { get; set; }
        public int? SellQuantity { get; set; }
        public int? SellMargin { get; set; }
        public int? SellMarketValue { get; set; }

        public int? TotalMargin { get; set; }
        public double? Delta { get; set; }
    }
}
