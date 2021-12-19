using Rollover.Tracking;
using System.Collections.Generic;

namespace Rollover.Input
{
    public interface IInputProcessor
    {
        List<string> Convert(string input, string state, IPortfolio _portfolio, ITrackedSymbols _trackedSymbols);
    }
}