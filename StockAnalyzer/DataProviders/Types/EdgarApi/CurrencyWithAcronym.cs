namespace StockAnalyzer.DataProviders.Types.EdgarApi
{
    public class CurrencyWithAcronym
    {
        public CurrencyWithAcronym(Currency currency, string acronym)
        {
            Currency = currency;
            Acronym = acronym;
        }

        public Currency Currency { get; set; }
        public string Acronym { get; set; }
    }
}
