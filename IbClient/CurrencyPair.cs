using IBApi;
using System;

namespace IbClient
{
    public class CurrencyPair
    {
        private static readonly string USD = "USD";
        private static readonly string IDEALPRO = "IDEALPRO";

        private CurrencyPair()
        {
        }

        private CurrencyPair(string value, bool usdIsInDenominator)
        {
            Value = value;
            UsdIsInDenominator = usdIsInDenominator;
        }

        public string Value { get; private set; }
        public bool UsdIsInDenominator { get; private set; }

        public static CurrencyPair BuildFor(string currency)
        {
            switch (currency)
            {
                case "EUR": return new CurrencyPair("EUR.USD", true);
                case "GPB": return new CurrencyPair("GBP.USD", true);
                case "NOK": return new CurrencyPair("USD.NOK", false);
                default: throw new NotImplementedException($"Not implemented for {currency}");
            }

            throw new NotImplementedException();
        }

        public static Contract ContractFromCurrency(string currency)
        {
            CurrencyPair pair = BuildFor(currency);
            return new Contract()
            {
                SecType = "CASH",
                Symbol = pair.UsdIsInDenominator ? currency : USD,
                Currency = pair.UsdIsInDenominator ? USD : currency,
                Exchange = IDEALPRO
            };
        }
    }
}