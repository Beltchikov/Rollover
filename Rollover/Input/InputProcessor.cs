using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Collections.Generic;

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
                        return new List<string> { input };
                    }

                    if (!_trackedSymbols.SymbolExists(symbol))
                    {
                        State = _reducer.GetState(State, input);
                        // TODO
                        // repository.GetTrackedSymbol(position.Contract)
                        _repository.ContractDetails(1, position.Contract);
                    }

                    return new List<string>();

                case "ContractInfo":
                    State = _reducer.GetState(State, input);
                    return new List<string> { input };

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
