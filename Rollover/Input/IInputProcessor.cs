using Rollover.Tracking;
using System.Collections.Generic;

namespace Rollover.Input
{
    public interface IInputProcessor
    {
        List<string> Convert(string input, string state, IPortfolio portfolio, ITrackedSymbols trackedSymbols);
    }
}