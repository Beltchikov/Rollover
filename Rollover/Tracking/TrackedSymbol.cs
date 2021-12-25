namespace Rollover.Tracking
{
    public class TrackedSymbol : ITrackedSymbol
    {
        public string Name { get; set; }

        // Three last figures are behind the comma figures
        public int NextStrike { get; set; }

        // Three last figures are behind the comma figures
        public int OverNextStrike { get; set; }

        //// obj.RequestId  0=> ReqIdContractDetails
        //var msg = $"ConId={obj.ContractDetails.Contract.ConId} " +
        //    $"SecType={obj.ContractDetails.Contract.SecType} " +
        //    $"Symbol={obj.ContractDetails.Contract.Symbol} " +
        //    $"Currency={obj.ContractDetails.Contract.Currency} " +
        //    $"Exchange={obj.ContractDetails.Contract.Exchange} " +
        //    $"Strike={obj.ContractDetails.Contract.Strike } ";

        public override string ToString()
        {
            return $"{Name} {NextStrike} {OverNextStrike}";
        }
    }
}
