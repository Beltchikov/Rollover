﻿using System.Collections.Generic;

namespace Dsmn.Ib
{
    public interface IIbConsumer
    {
        public bool ConnectedToTws { get; set; }
        public List<string>? Messages { get; internal set; }
    }
}
