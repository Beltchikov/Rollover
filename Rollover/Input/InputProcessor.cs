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

        private static List<PositionMessage> _positionMessageList = new List<PositionMessage>();
        public string State { get; private set; }

        public InputProcessor(
            IReducer reducer, 
            IPortfolio portfolio, 
            ITrackedSymbols trackedSymbols, 
            IRepository repository)
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

                    var trackedSymbol = _repository.GetTrackedSymbol(position.Contract);
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

        public List<string> ConvertMessage(object obj)
        {
            if(obj is string)
            {
                return ConvertMessageString(obj as string);
            }
            else if (obj is ConnectionStatusMessage)
            {
                return (obj as ConnectionStatusMessage).IsConnected
                    ? new List<string> { "Connected." }
                    : new List<string> { "Disconnected." };
            }
            else if (obj is ManagedAccountsMessage)
            {
                if (!(obj as ManagedAccountsMessage).ManagedAccounts.Any())
                {
                    throw new Exception("Unexpected: no positions.");
                }

                string msg = Environment.NewLine + "Accounts found: " 
                    + (obj as ManagedAccountsMessage).ManagedAccounts.Aggregate((r, n) => r + ", " + n);
                return new List<string> { msg };
            }
            else if (obj is PositionMessage)
            {
                if ((obj as PositionMessage).Position > 0)
                {
                    _positionMessageList.Add((obj as PositionMessage));
                }
                                
                return new List<string>();
            }

            throw new NotImplementedException();
        }

        private List<string> ConvertMessageString(string obj)
        {
            switch(obj)
            {
                case Constants.ON_POSITION_END:
                    List<string> resultList = _positionMessageList.Select(x => x.Contract.LocalSymbol)
                        .OrderBy(x => x).ToList();
                    resultList.Add(Constants.ENTER_SYMBOL_TO_TRACK);
                    _positionMessageList = new List<PositionMessage>();
                    return resultList;
                default:
                    return new List<string> { obj as string };
            }
        }
    }
}
