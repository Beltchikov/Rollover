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

        /// <summary>
        /// ContractFromCurrency - returns UsdIsInDenominator as a second return value
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static (Contract, bool) ContractFromCurrency(string currency)
        {
            CurrencyPair pair = BuildFor(currency);
            var contract = new Contract()
            {
                SecType = "CASH",
                Symbol = pair.UsdIsInDenominator ? currency : USD,
                Currency = pair.UsdIsInDenominator ? USD : currency,
                Exchange = IDEALPRO
            };
            return (contract, pair.UsdIsInDenominator);
        }
    }
}