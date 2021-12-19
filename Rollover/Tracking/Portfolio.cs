using IBSampleApp.messages;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Tracking
{
    public class Portfolio : IPortfolio
    {
        private readonly List<PositionMessage> _posisionList;

        public Portfolio()
        {
            _posisionList = new List<PositionMessage>();
        }
        
        public void Add(PositionMessage positionMessage)
        {
            _posisionList.Add(positionMessage);
        }

        public bool SymbolExists(string symbol)
        {
            return _posisionList.Any() 
                ? _posisionList.Any(p => p.Contract.LocalSymbol == symbol)
                : false;    
        }
    }
}
