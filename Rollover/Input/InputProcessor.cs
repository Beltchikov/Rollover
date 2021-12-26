using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Rollover.Input
{
    public class InputProcessor : IInputProcessor
    {
        private readonly IReducer _reducer;
        private readonly IPortfolio _portfolio;
        private readonly ITrackedSymbols _trackedSymbols;
        private readonly IRepository _repository;
        public string State { get; private set; }

        public InputProcessor(IReducer reducer, IPortfolio portfolio, ITrackedSymbols trackedSymbols, IRepository repository)
        {
            _reducer = reducer;
            _portfolio = portfolio;
            _trackedSymbols = trackedSymbols;
            _repository = repository;
        }

        public List<string> Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(State))
            {
                State = "WaitingForSymbol";
            }

            switch (State)
            {
                case "WaitingForSymbol":
                    var position = _portfolio.PositionBySymbol(input);
                    var symbol = position?.Contract?.Symbol;
                    if (position == null || symbol == null)
                    {
                        return new List<string> { "Symbol is not valid." };
                    }

                    if (!_trackedSymbols.SymbolExists(symbol))
                    {
                        var trackedSymbol = _repository.GetTrackedSymbol(position.Contract);
                        if (trackedSymbol != null)
                        {
                            if(!_trackedSymbols.Add(trackedSymbol))
                            {
                                return new List<string> { "Symbol is tracked already." };
                            }
                            var serialized = JsonSerializer.Serialize(trackedSymbol);
                            return new List<string> { serialized };
                        }
                        else
                        {
                            return new List<string> { "Symbol details could not be queried." };
                        }
                    }

                    return new List<string> { "Symbol is tracked already." };
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
