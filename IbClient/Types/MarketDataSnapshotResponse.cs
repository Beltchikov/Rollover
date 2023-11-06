using IbClient.messages;
using System.Collections.Generic;

namespace IbClient.Types
{
    public class MarketDataSnapshotResponse
    {
        public MarketDataSnapshotResponse()
        {
            TickPriceMessages = new List<TickPriceMessage>();
            TickSizeMessages = new List<TickSizeMessage>();
        }

        public List<TickPriceMessage> TickPriceMessages { get; set; }
        public List<TickSizeMessage> TickSizeMessages { get; set; }

        public bool EndOfSnapshot { get; set; }
    }
}
