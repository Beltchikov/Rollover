namespace PortfolioTrader.Model
{
    internal class Position
    {
        public Position()
        {
        }

        public Position(List<string> splitted)
        {
            if (splitted == null) throw new Exception("Unexpected. splitted may not be null");

            NetBms = Convert.ToInt32(splitted[1]);
            ConId = splitted.Count > 2 ? Convert.ToInt32(splitted[2]) : null;
            PriceInCents = splitted.Count > 3 ? Convert.ToInt32(splitted[3]) : null;
            PriceType = splitted.Count > 4 ? splitted[4] : null;
            Weight = splitted.Count > 5 ? Convert.ToInt32(splitted[5]) : null;
            Quantity = splitted.Count > 6 ? Convert.ToInt32(splitted[6]) : null;
            Margin = splitted.Count > 7 ? Convert.ToInt32(splitted[7]) : null;
            BarInCents = splitted.Count > 8 ? Convert.ToInt32(splitted[8]) : null;
        }

        public int NetBms { get; set; }
        public int? ConId { get; set; }
        public int? Weight { get; set; }
        public int? PriceInCents { get; set; }
        public string? PriceType{ get; set; }
        public int? Quantity { get; set; }
        public int? Margin { get; set; }
        public int? BarInCents { get; internal set; }

        public static string CalculateQuantity(string stocksAsString, int investmentAmount)
        {
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var resultDictionary = new Dictionary<string, Position>();

            foreach (var kvp in stocksDictionary)
            {
                if (kvp.Value.ConId == null) throw new Exception("Unexpected. Contract ID is null");
                int weightNotNullable = kvp.Value.Weight ?? throw new Exception("Unexpected. Weight is null");
                int priceInCentsNotNullable = kvp.Value.PriceInCents ?? throw new Exception("Unexpected. PriceInCents is null");

                int quantity = 0;
                if (priceInCentsNotNullable > 0)
                {
                    quantity = CalculateQuantity(investmentAmount, priceInCentsNotNullable, weightNotNullable);
                }
                resultDictionary[kvp.Key] = kvp.Value;
                resultDictionary[kvp.Key].Quantity = quantity;
            }

            return SymbolsAndScore.PositionDictionaryToString(resultDictionary);
        }

        private static int CalculateQuantity(int investmentAmount, int priceInCents, int weight)
        {
            var priceInDollars = (double)priceInCents / 100d;
            return (int)Math.Floor((double)(investmentAmount * weight / 100) / priceInDollars);
        }
    }
}
