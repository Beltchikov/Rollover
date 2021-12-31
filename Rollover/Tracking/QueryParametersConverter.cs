namespace Rollover.Tracking
{
    public class QueryParametersConverter : IQueryParametersConverter
    {
        public ITrackedSymbol TrackedSymbolForReqSecDefOptParams(ITrackedSymbol derivative)
        {
            if (derivative.Symbol == "MNQ" && derivative.SecType == "FOP")
            {
                ITrackedSymbol underlying = CopyTrackedSymbol(derivative);
                underlying.SecType = "IND";
                underlying.Exchange = "GLOBEX";
                return underlying;
            }

            return derivative;
        }

        private ITrackedSymbol CopyTrackedSymbol(ITrackedSymbol trackedSymbol)
        {
            var copy = new TrackedSymbol();

            copy.ConId = trackedSymbol.ConId;
            copy.Currency = trackedSymbol.Currency;
            copy.Exchange = trackedSymbol.Exchange;
            copy.LocalSymbol = trackedSymbol.LocalSymbol;
            copy.NextStrike = trackedSymbol.NextStrike;

            copy.NextButOneStrike = trackedSymbol.ConId;
            copy.ReqIdContractDetails = trackedSymbol.ReqIdContractDetails;
            copy.ReqIdSecDefOptParams = trackedSymbol.ReqIdSecDefOptParams;
            copy.SecType = trackedSymbol.SecType;
            copy.Strike = trackedSymbol.Strike;

            copy.Symbol = trackedSymbol.Symbol;

            return copy;
        }
    }
}
