using Rollover.Tracking;
using System;
using System.Collections.Generic;

namespace Rollover.Input
{
    public class InputProcessor : IInputProcessor
    {
        public List<string> Convert(string input, string state, IPortfolio _portfolio, ITrackedSymbols _trackedSymbols)
        {
            switch (state)
            {
                case "Connecting":
                    return new List<string> { input };
                case "Connected":
                    return new List<string> { input };
                case "WaitingForSymbol":
                    if (input == null)
                    {
                        return new List<string>();
                    }
                    else if (input.Contains("STATE"))
                    {
                        return new List<string> { input };
                    }
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }


            //switch (state)
            //{
            //    case "Connecting":
            //        return new List<string> { input };
            //    case "Connected":
            //        return new List<string> { input };
            //    case "WaitingForSymbol":
            //        if(input == null)
            //        {
            //            return new List<string>();
            //        }
            //        else if(input.Contains("STATE"))
            //        {
            //            return new List<string> { input };
            //        }
            //        else if (input == "Enter a symbol to track:")
            //        {
            //            return new List<string> { input };
            //        }
            //        else
            //        {
            //            if (_portfolio.SymbolExists(input))
            //            {
            //                if (!_trackedSymbols.SymbolExists(input))
            //                {
            //                    _trackedSymbols.Add(input);
            //                    return new List<string> { $"Symbol {input} added" };
            //                }
            //                else
            //                {
            //                    return new List<string> { $"Symbol {input} is allready tracked" };
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
