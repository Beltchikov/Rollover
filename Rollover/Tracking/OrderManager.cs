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
                var priceUnderlying = _repository.LastPrice(trackedSymbol.ConId, trackedSymbol.Exchange);
                if (!priceUnderlying.Item1)
                {
                    throw new NoMarketDataException($"Was not able to get a price for {trackedSymbol.ConId} and {trackedSymbol.Exchange}");
                }

                var currentStrike = trackedSymbol.Strike(_repository);
                var nextStrike = trackedSymbol.NextStrike(_repository, currentStrike.Value);
                if (priceUnderlying.Item2 > nextStrike && nextStrike > currentStrike)
                {
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
