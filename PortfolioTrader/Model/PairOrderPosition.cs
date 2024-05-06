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

            BuyNetBms = Convert.ToInt32(splitted[1]);
            BuyConId = Convert.ToInt32(splitted[2]);

            if (Int32.TryParse(splitted[3], out int buyPriceInCents)) BuyPriceInCents = buyPriceInCents;  
            BuyPriceType = splitted[4];  
            if(Int32.TryParse(splitted[5], out int buyWeight)) BuyWeight = buyWeight;  
            if(Int32.TryParse(splitted[6], out int buyQuantity)) BuyQuantity = buyQuantity;  
            if(Int32.TryParse(splitted[7], out int buyMargin)) BuyMargin = buyMargin;  
            if(Int32.TryParse(splitted[8], out int buyMarketValue)) BuyMarketValue = buyMarketValue;  
         
            SellNetBms = Convert.ToInt32(splitted[9]);
            SellConId = Convert.ToInt32(splitted[10]);

            if (Int32.TryParse(splitted[11], out int sellPriceInCents)) SellPriceInCents = sellPriceInCents;
            SellPriceType = splitted[12];
            if (Int32.TryParse(splitted[13], out int sellWeight)) SellWeight = sellWeight;
            if (Int32.TryParse(splitted[14], out int sellQuantity)) SellQuantity = sellQuantity;
            if (Int32.TryParse(splitted[15], out int sellMargin)) SellMargin = sellMargin;
            if (Int32.TryParse(splitted[16], out int sellMarketValue)) SellMarketValue = sellMarketValue;

            if (Int32.TryParse(splitted[17], out int totalMargin)) TotalMargin = totalMargin;
            if (Int32.TryParse(splitted[18], out int delta)) Delta = delta;
        }

        public int BuyNetBms { get; set; }
        public int? BuyConId { get; set; }
        public int? BuyPriceInCents { get; set; }
        public string? BuyPriceType { get; set; }
        public int? BuyWeight { get; set; }
        public int? BuyQuantity { get; set; }
        public int? BuyMargin { get; set; }
        public int? BuyMarketValue { get; set; }

        public int SellNetBms { get; set; }
        public int? SellConId { get; set; }
        public int? SellPriceInCents { get; set; }
        public string? SellPriceType { get; set; }
        public int? SellWeight { get; set; }
        public int? SellQuantity { get; set; }
        public int? SellMargin { get; set; }
        public int? SellMarketValue { get; set; }

        public int? TotalMargin { get; set; }
        public double? Delta { get; set; }
    }
}
