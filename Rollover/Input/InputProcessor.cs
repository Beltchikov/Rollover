using IBApi;
using IBSampleApp.messages;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public InputProcessor(
            IReducer reducer,
            IPortfolio portfolio,
            ITrackedSymbols trackedSymbols,
            IRepository repository, ITrackedSymbolFactory trackedSymbolFactory)
        {
            _reducer = reducer;
            _portfolio = portfolio;
            _trackedSymbols = trackedSymbols;
            _repository = repository;
        }

        public List<string> Convert(string input)
        {
            if (input == null)
            {
                return new List<string>();
            }

            if (string.IsNullOrWhiteSpace(State))
            {
                State = "WaitingForSymbol";
            }

            switch (State)
            {
                case "WaitingForSymbol":
                case "Active":
                    if (_trackedSymbols.Any())
                    {
                        State = "Active";
                    }

                    if(State == "Active")
                    {
                        //_orderManager.RolloverIfNextStrike(_trackedSymbols);
                    }
                    
                    if (input.Contains("errorCode"))
                    {
                        return input.Contains("id=-1")
                            ? new List<string>()
                            : new List<string> { input };
                    }

                    var position = _portfolio.PositionBySymbol(input);
                    var symbol = position?.Contract?.Symbol;
                    if (position == null || symbol == null)
                    {
                        return new List<string> { "Symbol is not valid." };
                    }

                    List<string> messages = AddTrackedSymbol(position?.Contract, _trackedSymbols);
                    return messages;

                default:
                    throw new NotImplementedException();
            }
        }

        private List<string> AddTrackedSymbol(Contract contract, ITrackedSymbols trackedSymbols)
        {
            ITrackedSymbol trackedSymbol = null;
            try
            {
                trackedSymbol = _repository.GetTrackedSymbol(contract);
            }
            catch (Exception ex)
            {
                return new List<string> { ex.ToString(), "Please try again." };
            }

            if (trackedSymbol != null)
            {
                if (!_trackedSymbols.Add(trackedSymbol))
                {
                    return new List<string> { "Symbol is tracked already." };
                }
                return _trackedSymbols.Summary();
            }
            else
            {
                return new List<string> { "Symbol details could not be queried. trackedSymbol is null." };
            }
        }
    }
}
