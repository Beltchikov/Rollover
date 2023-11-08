using IbClient.messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IbClient.Types
{
    public class MarketDataSnapshotResponse
    {
        private int _reqId;
        private bool _endOfSnapshot;
        private List<TickPriceMessage> _tickPriceMessages { get; set; }
        private List<TickSizeMessage> _tickSizeMessages { get; set; }

        private MarketDataSnapshotResponse() { }

        public MarketDataSnapshotResponse(int reqId)
        {
            _reqId = reqId;
            _tickPriceMessages = new List<TickPriceMessage>();
            _tickSizeMessages = new List<TickSizeMessage>();
        }

        public int GetId()
        {
            return _reqId;
        }

        public void AddTickPriceMessage(TickPriceMessage tickPriceMessage)
        {
            _tickPriceMessages.Add(tickPriceMessage);
        }

        public void SetEndOfSnapshot()
        {
            _endOfSnapshot = true;
        }

        public bool EndOfSnapshot()
        {
            return _endOfSnapshot;
        }
        public double? GetPrice(TickType tickType)
        {
            var message = _tickPriceMessages.Single(m => m.Field == (int)tickType);
            return message.Price;
        }
    }
}
