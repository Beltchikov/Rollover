using Rollover.Ib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Tracking
{
    public class TrackedSymbolFactory : ITrackedSymbolFactory
    {
        private readonly IRepository _requestSender;

        public TrackedSymbolFactory(IRepository requestSender)
        {
            _requestSender = requestSender;
        }

        public TrackedSymbol Create(string symbol)
        {
            // TODO NextStrike, OverNextStrike
            //_requestSender.ReqSecDefOptParams()

            return new TrackedSymbol { Name = symbol };
        }
    }
}
