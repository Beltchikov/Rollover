using IBApi;
using System.Collections.Generic;

namespace SsbHedger.IbModel
{
    public interface IContractSpy
    {
        List<IComboLeg> ComboLegs { get; set; }
        string ComboLegsDescription { get; set; }
        int ConId { get; set; }
        string Currency { get; set; }
        IDeltaNeutralContract DeltaNeutralContract { get; set; }
        string Exchange { get; set; }
        bool IncludeExpired { get; set; }
        string LastTradeDateOrContractMonth { get; set; }
        string LocalSymbol { get; set; }
        string Multiplier { get; set; }
        string PrimaryExch { get; set; }
        string Right { get; set; }
        string SecId { get; set; }
        string SecIdType { get; set; }
        string SecType { get; set; }
        double Strike { get; set; }
        string Symbol { get; set; }
        string TradingClass { get; set; }

        string ToString();
    }
}
