using System;

namespace IbClient
{
    public class CurrencyPair
    {
        private CurrencyPair()
        {
        }

        private CurrencyPair(string value, bool usdIsInDenominator)
        {
            Value = value;
            UsdIsInDenominator = usdIsInDenominator;
        }

        public static string Value { get; private set; }
        public static bool UsdIsInDenominator { get; private set; }

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
    }
}