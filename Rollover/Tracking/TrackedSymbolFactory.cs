﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Tracking
{
    public class TrackedSymbolFactory : ITrackedSymbolFactory
    {
        public TrackedSymbol Create(string symbol)
        {
            //throw new NotImplementedException();
            // TODO NextStrike, OverNextStrike
            return new TrackedSymbol { Name = symbol };
        }
    }
}
