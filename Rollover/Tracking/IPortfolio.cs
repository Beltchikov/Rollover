using IBSampleApp.messages;
using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface IPortfolio
    {
        void Add(PositionMessage positionMessage);
        bool SymbolExists(string symbol);
        PositionMessage PositionBySymbol(string symbol);
        List<string> Summary();
    }
}