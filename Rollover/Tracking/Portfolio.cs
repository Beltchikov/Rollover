using IBSampleApp.messages;
using System.Collections.Generic;

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
    }
}
