using Rollover.Tracking;
using System;
using System.Collections.Generic;

namespace Rollover.Input
{
    public class InputProcessor : IInputProcessor
    {
        public List<string> Convert(string input, string state, IPortfolio portfolio, ITrackedSymbols trackedSymbols)
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
                    else if (input == "Enter a symbol to track:")
                    {
                        return new List<string> { input };
                    }
                    else
                    {
                        if (portfolio.SymbolExists(input))
                        {
                            if (!trackedSymbols.SymbolExists(input))
                            {
                                trackedSymbols.Add(input);
                                var outputList = new List<string>();
                                outputList.Add($"Symbol {input} added");
                                outputList.Add($"Tracked symbols:");
                                outputList.AddRange(trackedSymbols.AllAsString());
                                return outputList;
                            }
                            else
                            {
                                return new List<string> { $"Symbol {input} is already tracked" };
                            }
                        }
                        else
                        {
                            return new List<string> { $"Unknown symbol {input}" };
                        }
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
