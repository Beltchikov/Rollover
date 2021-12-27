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
        private readonly ISecTypeConverter _secTypeConverter;
        public string State { get; private set; }

        public InputProcessor(
            IReducer reducer, 
            IPortfolio portfolio, 
            ITrackedSymbols trackedSymbols, 
            IRepository repository, 
            ISecTypeConverter secTypeConverter)
        {
            _reducer = reducer;
            _portfolio = portfolio;
            _trackedSymbols = trackedSymbols;
            _repository = repository;
            _secTypeConverter = secTypeConverter;
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
                    if(input.Contains("errorCode"))
                    {
                        return input.Contains("id=-1")
                            ? new List<string> ()
                            : new List<string> { input };
                    }
                    
                    var position = _portfolio.PositionBySymbol(input);
                    var symbol = position?.Contract?.Symbol;
                    if (position == null || symbol == null)
                    {
                        return new List<string> { "Symbol is not valid." };
                    }


                    var underlyingContract = _secTypeConverter.GetUnderlyingSecType(position.Contract);
                    var trackedSymbol = _repository.GetTrackedSymbol(underlyingContract);
                    if (trackedSymbol != null)
                    {
                        if (!_trackedSymbols.Add(trackedSymbol))
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

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
