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
        public string State { get; private set; }

        public InputProcessor(IReducer reducer, IPortfolio portfolio, ITrackedSymbols trackedSymbols)
        {
            _reducer = reducer;
            _portfolio = portfolio;
            _trackedSymbols = trackedSymbols;
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
                    if (input == null)
                    {
                        return new List<string>();
                    }
                    else if (input.Contains("STATE"))
                    {
                        return new List<string> { input };
                    }
                    else if (input == "Enter a symbol to track:")
                    {
                        return new List<string> { input };
                    }
                    else
                    {
                        var position = _portfolio.PositionBySymbol(input);

                        //            var position = portfolio.PositionBySymbol(input);
                        //            if (position != null)
                        //            {
                        //                var symbol = position.Contract.Symbol;

                        //                if (!trackedSymbols.SymbolExists(input))
                        //                {
                        //                    // TODO get con id
                        //                    //ibClient.ClientSocket.reqContractDetails(CONTRACT_DETAILS_ID, contract);
                        //                    //ibClient.ContractDetails += HandleContractDataMessage;



                        //                    trackedSymbols.BeginAdd(70100001, symbol, "GLOBEX", "IND", position.Contract.ConId);
                        //                    //var outputList = new List<string>();
                        //                    //outputList.Add($"Symbol {input} added");
                        //                    //outputList.Add($"Tracked symbols:");
                        //                    //outputList.AddRange(trackedSymbols.AllAsString());
                        //                    //return outputList;
                        //                    return new List<string>();
                        //                }
                        //                else
                        //                {
                        //                    return new List<string> { $"Symbol {input} is already tracked" };
                        //                }

                        return new List<string> { input };
                    }
                default:
                    throw new NotImplementedException();
            }

            // if state NullOrWhitespace - state = Connected
            //else _state = _reducer.GetState(_state, input);
            // case Connected
            // case WaitingForSymbol

            return new List<string>();

            //switch (state)
            //{
            //    case "Connecting":
            //        return new List<string> { input };
            //    case "Connected":
            //        return new List<string> { input };
            //    case "WaitingForSymbol":
            //        if (input == null)
            //        {
            //            return new List<string>();
            //        }
            //        else if (input.Contains("STATE"))
            //        {
            //            return new List<string> { input };
            //        }
            //        else if (input == "Enter a symbol to track:")
            //        {
            //            return new List<string> { input };
            //        }
            //        else
            //        {
            //            var position = portfolio.PositionBySymbol(input);
            //            if (position != null)
            //            {
            //                var symbol = position.Contract.Symbol;

            //                if (!trackedSymbols.SymbolExists(input))
            //                {
            //                    // TODO get con id
            //                    //ibClient.ClientSocket.reqContractDetails(CONTRACT_DETAILS_ID, contract);
            //                    //ibClient.ContractDetails += HandleContractDataMessage;



            //                    trackedSymbols.BeginAdd(70100001, symbol, "GLOBEX", "IND", position.Contract.ConId);
            //                    //var outputList = new List<string>();
            //                    //outputList.Add($"Symbol {input} added");
            //                    //outputList.Add($"Tracked symbols:");
            //                    //outputList.AddRange(trackedSymbols.AllAsString());
            //                    //return outputList;
            //                    return new List<string>();
            //                }
            //                else
            //                {
            //                    return new List<string> { $"Symbol {input} is already tracked" };
            //                }
            //            }
            //            else
            //            {
            //                return new List<string> { $"Unknown symbol {input}" };
            //            }
            //        }
            //    default:
            //        throw new NotImplementedException();
            //}
        }
    }
}
