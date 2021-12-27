using IBApi;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public class UnderlyingConverter : IUnderlyingConverter
    {
        public Contract GetUnderlying(Contract derivative)
        {
            if (derivative.Symbol == "MNQ" && derivative.SecType == "FOP")
            {
                return new Contract
                {
                    SecType = "IND",
                    Symbol = derivative.Symbol,
                    Currency = derivative.Currency,
                    Exchange = "GLOBEX"
                };
            }

            return derivative;
        }
    }
}