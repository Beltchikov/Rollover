﻿using Rollover.Tracking;
using System;
using System.Collections.Generic;

namespace Rollover.Input
{
    public class InputProcessor : IInputProcessor
    {
        private string _state;

        public List<string> Convert(string input, IPortfolio portfolio, ITrackedSymbols trackedSymbols)
        {
            // if state NullOrWhitespace - state = Connected
            //else _state = _reducer.GetState(_state, input);
            // case Connected
            // case WaitingForSymbol

            throw new NotImplementedException();

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
            //            if (portfolio.SymbolExists(input))
            //            {
            //                if (!trackedSymbols.SymbolExists(input))
            //                {
            //                    trackedSymbols.BeginAdd(input);
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
