using IBSampleApp.messages;
using System;
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
            if (positionMessage == null)
            {
                return;
            }

            _posisionList.Add(positionMessage);
        }

        public bool SymbolExists(string symbol)
        {
            return _posisionList.Any()
                ? _posisionList.Any(p => p.Contract.LocalSymbol == symbol)
                : false;
        }

        public PositionMessage PositionBySymbol(string symbol)
        {
            return _posisionList.FirstOrDefault(p => p.Contract.LocalSymbol == symbol);
        }

        public List<string> Summary()
        {
            var summary = new List<string> { String.Empty };
            summary.Add(Constants.POSITIONS);
            summary.AddRange(_posisionList.Select(p => p.Contract.LocalSymbol));
            summary.Add("");

            return summary;
        }
    }
}
