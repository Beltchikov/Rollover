using IBApi;
using Rollover.Ib;

namespace Rollover.Tracking
{
    public class OrderManager : IOrderManager
    {
        private IRepository _repository;

        public OrderManager(IRepository repository)
        {
            _repository = repository;
        }

        public void RolloverIfNextStrike(ITrackedSymbols trackedSymbols)
        {
            foreach (var trackedSymbol in trackedSymbols)
            {
                var priceUnderlying = trackedSymbol.LastUnderlyingPrice(_repository);
                if(priceUnderlying == 0) // Market closed
                {
                    continue;
                }

                var currentStrike = trackedSymbol.Strike(_repository);
                var nextStrike = trackedSymbol.NextStrike(_repository, currentStrike.Value);
                if (priceUnderlying > nextStrike && nextStrike > currentStrike)
                {
                    // TODO
                    var contract = new Contract()
                    {
                        Symbol = trackedSymbol.Symbol(_repository),
                        SecType = trackedSymbol.SecType(_repository),
                        Currency = trackedSymbol.Currency(_repository),
                        Exchange = trackedSymbol.Exchange
                    };

                    _repository.PlaceBearSpread(trackedSymbol);
                }
            }
        }
    }
}
