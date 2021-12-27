using IBApi;

namespace Rollover.Ib
{
    public class UnderlyingConverter : IUnderlyingConverter
    {
        public Contract GetUnderlying(Contract derivative)
        {
            if (derivative.Symbol == "MNQ" && derivative.SecType == "FOP")
            {
                Contract underlying = CopyContract(derivative);
                underlying.SecType = "IND";
                underlying.Exchange = "GLOBEX";
                return underlying;
            }

            return derivative;
        }

        private Contract CopyContract(Contract contract)
        {
            var copy = new Contract();

            copy.ComboLegs = contract.ComboLegs;
            copy.ComboLegsDescription = contract.ComboLegsDescription;
            copy.ConId = contract.ConId;
            copy.Currency = contract.Currency;
            copy.DeltaNeutralContract = contract.DeltaNeutralContract;

            copy.Exchange = contract.Exchange;
            copy.IncludeExpired = contract.IncludeExpired;
            copy.LastTradeDateOrContractMonth = contract.LastTradeDateOrContractMonth;
            copy.LocalSymbol = contract.LocalSymbol;
            copy.Multiplier = contract.Multiplier;

            copy.PrimaryExch = contract.PrimaryExch;
            copy.Right = contract.Right;
            copy.SecId = contract.SecId;
            copy.SecIdType = contract.SecIdType;
            copy.SecType = contract.SecType;

            copy.Strike = contract.Strike;
            copy.Symbol = contract.Symbol;
            copy.TradingClass = contract.TradingClass;

            return copy;
        }
    }
}