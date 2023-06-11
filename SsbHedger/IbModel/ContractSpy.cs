using System.Collections.Generic;

namespace SsbHedger.IbModel
{
    public class ContractSpy : IContractSpy
    {
        public ContractSpy()
        {
            ComboLegs = new List<IComboLeg>();
            ComboLegsDescription = "";
            Currency = "USD";
            DeltaNeutralContract = null!;
            Exchange = "SMART";
            LastTradeDateOrContractMonth = "";
            LocalSymbol = "";
            Multiplier = "";
            PrimaryExch = "";
            Right = "";
            SecId = "";
            SecIdType = "";
            SecType = "STK";
            Symbol = "SPY";
            TradingClass = "";
        }

        public List<IComboLeg> ComboLegs { get; set; }
        public string ComboLegsDescription { get; set; }
        public int ConId { get; set; }
        public string Currency { get; set; }
        public IDeltaNeutralContract DeltaNeutralContract { get; set; }
        public string Exchange { get; set; }
        public bool IncludeExpired { get; set; }
        public string LastTradeDateOrContractMonth { get; set; }
        public string LocalSymbol { get; set; }
        public string Multiplier { get; set; }
        public string PrimaryExch { get; set; }
        public string Right { get; set; }
        public string SecId { get; set; }
        public string SecIdType { get; set; }
        public string SecType { get; set; }
        public double Strike { get; set; }
        public string Symbol { get; set; }
        public string TradingClass { get; set; }

        public override string ToString()
        {
            return "";
        }
    }
}
