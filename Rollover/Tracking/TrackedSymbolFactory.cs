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
        private readonly IRequestSender _requestSender;

        public TrackedSymbolFactory(IRequestSender requestSender)
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
